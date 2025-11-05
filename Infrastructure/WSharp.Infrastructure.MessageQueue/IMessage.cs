namespace WSharp.Infrastructure.MessageQueue;

/// <summary>
/// 消息接口
/// </summary>
public interface IMessage
{
    /// <summary>
    /// 消息 ID
    /// </summary>
    string MessageId { get; }

    /// <summary>
    /// 消息创建时间
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// 消息类型
    /// </summary>
    string MessageType { get; }

    /// <summary>
    /// 消息元数据
    /// </summary>
    IDictionary<string, string> Metadata { get; }
}

/// <summary>
/// 消息基类
/// </summary>
public abstract class MessageBase : IMessage
{
    /// <summary>
    /// 初始化消息
    /// </summary>
    protected MessageBase()
    {
        this.MessageId = Guid.NewGuid().ToString();
        this.CreatedAt = DateTime.UtcNow;
        this.MessageType = this.GetType().Name;
        this.Metadata = new Dictionary<string, string>();
    }

    /// <summary>
    /// 消息 ID
    /// </summary>
    public string MessageId { get; set; }

    /// <summary>
    /// 消息创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public string MessageType { get; set; }

    /// <summary>
    /// 消息元数据
    /// </summary>
    public IDictionary<string, string> Metadata { get; set; }
}
