using Microsoft.Extensions.Logging;

namespace WSharp.Distributed.Transaction;

/// <summary>
/// Saga orchestrator implementation
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
            _logger.LogInformation("Starting saga {SagaName} with ID {SagaId}", context.SagaName, context.SagaId);

            context.State = SagaState.Running;
            await _stateRepository.SaveAsync(context, cancellationToken);

            // Execute each step
            for (int i = context.CurrentStepIndex + 1; i < stepList.Count; i++)
            {
                context.CurrentStepIndex = i;
                var step = stepList[i];

                _logger.LogInformation("Executing step {StepIndex}/{TotalSteps}: {StepName} for saga {SagaId}",
                    i + 1, stepList.Count, step.StepName, context.SagaId);

                await step.ExecuteAsync(context, cancellationToken);
                await _stateRepository.UpdateAsync(context, cancellationToken);

                _logger.LogInformation("Step {StepName} completed successfully for saga {SagaId}",
                    step.StepName, context.SagaId);
            }

            context.State = SagaState.Completed;
            context.CompletedAt = DateTime.UtcNow;
            await _stateRepository.UpdateAsync(context, cancellationToken);

            _logger.LogInformation("Saga {SagaId} completed successfully", context.SagaId);

            return context;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saga {SagaId} failed at step {StepIndex}. Starting compensation...",
                context.SagaId, context.CurrentStepIndex);

            context.ErrorMessage = ex.Message;
            context.ErrorStackTrace = ex.StackTrace;
            await _stateRepository.UpdateAsync(context, cancellationToken);

            // Compensate
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
            throw new InvalidOperationException($"Saga {sagaId} not found");
        }

        if (context.State == SagaState.Completed || context.State == SagaState.Compensated)
        {
            _logger.LogWarning("Saga {SagaId} is already in terminal state {State}", sagaId, context.State);
            return context;
        }

        _logger.LogInformation("Resuming saga {SagaId} from step {StepIndex}", sagaId, context.CurrentStepIndex);

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

            _logger.LogInformation("Starting compensation for saga {SagaId}", context.SagaId);

            // Compensate steps in reverse order
            for (int i = context.CurrentStepIndex; i >= 0; i--)
            {
                var step = stepList[i];

                _logger.LogInformation("Compensating step {StepName} for saga {SagaId}",
                    step.StepName, context.SagaId);

                try
                {
                    await step.CompensateAsync(context, cancellationToken);
                    _logger.LogInformation("Step {StepName} compensated successfully for saga {SagaId}",
                        step.StepName, context.SagaId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to compensate step {StepName} for saga {SagaId}",
                        step.StepName, context.SagaId);
                    // Continue with other compensations
                }
            }

            context.State = SagaState.Compensated;
            context.CompletedAt = DateTime.UtcNow;
            await _stateRepository.UpdateAsync(context, cancellationToken);

            _logger.LogInformation("Saga {SagaId} compensated successfully", context.SagaId);

            return context;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Compensation failed for saga {SagaId}", context.SagaId);

            context.State = SagaState.CompensationFailed;
            context.ErrorMessage = ex.Message;
            context.ErrorStackTrace = ex.StackTrace;
            await _stateRepository.UpdateAsync(context, cancellationToken);

            throw;
        }
    }
}
