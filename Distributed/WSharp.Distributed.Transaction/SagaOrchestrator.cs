using Microsoft.Extensions.Logging;

namespace WSharp.Distributed.Transaction;

/// <summary>
/// Saga 编排器实现
/// </summary>
public class SagaOrchestrator : ISagaOrchestrator
{
    private readonly ISagaStateRepository _stateRepository;
    private readonly ILogger<SagaOrchestrator> _logger;

    public SagaOrchestrator(
        ISagaStateRepository stateRepository,
        ILogger<SagaOrchestrator> logger)
    {
        _stateRepository = stateRepository;
        _logger = logger;
    }

    public async Task<SagaContext> ExecuteAsync<TInput>(
        string sagaName,
        TInput input,
        IEnumerable<SagaStep> steps,
        CancellationToken cancellationToken = default) where TInput : class
    {
        var context = new SagaContext
        {
            SagaName = sagaName,
            StartedAt = DateTime.UtcNow
        };

        context.SetData("Input", input);

        return await ExecuteAsync(context, steps, cancellationToken);
    }

    public async Task<SagaContext> ExecuteAsync(
        SagaContext context,
        IEnumerable<SagaStep> steps,
        CancellationToken cancellationToken = default)
    {
        var stepList = steps.ToList();

        try
        {
            _logger.LogInformation("开始执行 Saga {SagaName}，ID 为 {SagaId}", context.SagaName, context.SagaId);

            context.State = SagaState.Running;
            await _stateRepository.SaveAsync(context, cancellationToken);

            // 执行每个步骤
            for (int i = context.CurrentStepIndex + 1; i < stepList.Count; i++)
            {
                context.CurrentStepIndex = i;
                var step = stepList[i];

                _logger.LogInformation("执行步骤 {StepIndex}/{TotalSteps}: {StepName}，Saga ID {SagaId}",
                    i + 1, stepList.Count, step.StepName, context.SagaId);

                await step.ExecuteAsync(context, cancellationToken);
                await _stateRepository.UpdateAsync(context, cancellationToken);

                _logger.LogInformation("步骤 {StepName} 成功完成，Saga ID {SagaId}",
                    step.StepName, context.SagaId);
            }

            context.State = SagaState.Completed;
            context.CompletedAt = DateTime.UtcNow;
            await _stateRepository.UpdateAsync(context, cancellationToken);

            _logger.LogInformation("Saga {SagaId} 成功完成", context.SagaId);

            return context;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saga {SagaId} 在步骤 {StepIndex} 失败，开始补偿...",
                context.SagaId, context.CurrentStepIndex);

            context.ErrorMessage = ex.Message;
            context.ErrorStackTrace = ex.StackTrace;
            await _stateRepository.UpdateAsync(context, cancellationToken);

            // 执行补偿
            return await CompensateAsync(context, stepList, cancellationToken);
        }
    }

    public async Task<SagaContext> ResumeAsync(
        string sagaId,
        IEnumerable<SagaStep> steps,
        CancellationToken cancellationToken = default)
    {
        var context = await _stateRepository.GetAsync(sagaId, cancellationToken);

        if (context == null)
        {
            throw new InvalidOperationException($"未找到 Saga {sagaId}");
        }

        if (context.State == SagaState.Completed || context.State == SagaState.Compensated)
        {
            _logger.LogWarning("Saga {SagaId} 已处于最终状态 {State}", sagaId, context.State);
            return context;
        }

        _logger.LogInformation("从步骤 {StepIndex} 恢复 Saga {SagaId}", context.CurrentStepIndex, sagaId);

        return await ExecuteAsync(context, steps, cancellationToken);
    }

    public async Task<SagaContext> CompensateAsync(
        SagaContext context,
        IEnumerable<SagaStep> steps,
        CancellationToken cancellationToken = default)
    {
        var stepList = steps.ToList();

        try
        {
            context.State = SagaState.Compensating;
            await _stateRepository.UpdateAsync(context, cancellationToken);

            _logger.LogInformation("开始补偿 Saga {SagaId}", context.SagaId);

            // 按相反顺序补偿步骤
            for (int i = context.CurrentStepIndex; i >= 0; i--)
            {
                var step = stepList[i];

                _logger.LogInformation("补偿步骤 {StepName}，Saga ID {SagaId}",
                    step.StepName, context.SagaId);

                try
                {
                    await step.CompensateAsync(context, cancellationToken);
                    _logger.LogInformation("步骤 {StepName} 补偿成功，Saga ID {SagaId}",
                        step.StepName, context.SagaId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "补偿步骤 {StepName} 失败，Saga ID {SagaId}",
                        step.StepName, context.SagaId);
                    // 继续执行其他补偿
                }
            }

            context.State = SagaState.Compensated;
            context.CompletedAt = DateTime.UtcNow;
            await _stateRepository.UpdateAsync(context, cancellationToken);

            _logger.LogInformation("Saga {SagaId} 补偿成功", context.SagaId);

            return context;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saga {SagaId} 补偿失败", context.SagaId);

            context.State = SagaState.CompensationFailed;
            context.ErrorMessage = ex.Message;
            context.ErrorStackTrace = ex.StackTrace;
            await _stateRepository.UpdateAsync(context, cancellationToken);

            throw;
        }
    }
}
