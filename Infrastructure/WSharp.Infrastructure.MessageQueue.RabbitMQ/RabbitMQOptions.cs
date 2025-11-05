namespace WSharp.Infrastructure.MessageQueue.RabbitMQ;

/// <summary>
/// RabbitMQ 选项
/// </summary>
public class RabbitMQOptions
{
    /// <summary>
    /// 主机地址
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// 虚拟主机
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// 是否自动重连
    /// </summary>
    public bool AutomaticRecoveryEnabled { get; set; } = true;

    /// <summary>
    /// 网络恢复间隔（秒）
    /// </summary>
    public int NetworkRecoveryInterval { get; set; } = 10;

    /// <summary>
    /// 请求的心跳间隔（秒）
    /// </summary>
    public ushort RequestedHeartbeat { get; set; } = 60;

    /// <summary>
    /// 预取数量
    /// </summary>
    public ushort PrefetchCount { get; set; } = 10;

    /// <summary>
    /// 交换机类型（direct、topic、fanout、headers）
    /// </summary>
    public string ExchangeType { get; set; } = "topic";

    /// <summary>
    /// 默认交换机名称
    /// </summary>
    public string DefaultExchange { get; set; } = string.Empty;
}
