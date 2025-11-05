using Newtonsoft.Json;
using System.Text;

namespace WSharp.Infrastructure.Caching.Redis;

/// <summary>
/// Redis 序列化辅助类
/// </summary>
public static class RedisSerializationHelper
{
    private static readonly JsonSerializerSettings DefaultSettings = new()
    {
        TypeNameHandling = TypeNameHandling.None,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
        DateTimeZoneHandling = DateTimeZoneHandling.Utc
    };

    /// <summary>
    /// 序列化对象为 JSON 字符串
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="value">要序列化的对象</param>
    /// <param name="settings">自定义序列化设置（可选）</param>
    /// <returns>JSON 字符串</returns>
    public static string Serialize<T>(T value, JsonSerializerSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        return JsonConvert.SerializeObject(value, settings ?? DefaultSettings);
    }

    /// <summary>
    /// 反序列化 JSON 字符串为对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="json">JSON 字符串</param>
    /// <param name="settings">自定义序列化设置（可选）</param>
    /// <returns>反序列化的对象</returns>
    public static T? Deserialize<T>(string json, JsonSerializerSettings? settings = null)
    {
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(json, settings ?? DefaultSettings);
    }

    /// <summary>
    /// 序列化对象为字节数组
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="value">要序列化的对象</param>
    /// <param name="settings">自定义序列化设置（可选）</param>
    /// <returns>字节数组</returns>
    public static byte[] SerializeToBytes<T>(T value, JsonSerializerSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        var json = Serialize(value, settings);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// 反序列化字节数组为对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="bytes">字节数组</param>
    /// <param name="settings">自定义序列化设置（可选）</param>
    /// <returns>反序列化的对象</returns>
    public static T? DeserializeFromBytes<T>(byte[] bytes, JsonSerializerSettings? settings = null)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return default;
        }

        var json = Encoding.UTF8.GetString(bytes);
        return Deserialize<T>(json, settings);
    }

    /// <summary>
    /// 尝试序列化对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="value">要序列化的对象</param>
    /// <param name="result">序列化结果</param>
    /// <param name="settings">自定义序列化设置（可选）</param>
    /// <returns>是否序列化成功</returns>
    public static bool TrySerialize<T>(T value, out string? result, JsonSerializerSettings? settings = null)
    {
        try
        {
            result = Serialize(value, settings);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// 尝试反序列化对象
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="json">JSON 字符串</param>
    /// <param name="result">反序列化结果</param>
    /// <param name="settings">自定义序列化设置（可选）</param>
    /// <returns>是否反序列化成功</returns>
    public static bool TryDeserialize<T>(string json, out T? result, JsonSerializerSettings? settings = null)
    {
        try
        {
            result = Deserialize<T>(json, settings);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// 压缩序列化（适用于大对象）
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="value">要序列化的对象</param>
    /// <param name="settings">自定义序列化设置（可选）</param>
    /// <returns>压缩后的字节数组</returns>
    public static byte[] SerializeWithCompression<T>(T value, JsonSerializerSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        var json = Serialize(value, settings);
        var bytes = Encoding.UTF8.GetBytes(json);

        using var outputStream = new MemoryStream();
        using (var gzipStream = new System.IO.Compression.GZipStream(outputStream, System.IO.Compression.CompressionLevel.Optimal))
        {
            gzipStream.Write(bytes, 0, bytes.Length);
        }

        return outputStream.ToArray();
    }

    /// <summary>
    /// 解压缩反序列化
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="compressedBytes">压缩后的字节数组</param>
    /// <param name="settings">自定义序列化设置（可选）</param>
    /// <returns>反序列化的对象</returns>
    public static T? DeserializeWithDecompression<T>(byte[] compressedBytes, JsonSerializerSettings? settings = null)
    {
        if (compressedBytes == null || compressedBytes.Length == 0)
        {
            return default;
        }

        using var inputStream = new MemoryStream(compressedBytes);
        using var gzipStream = new System.IO.Compression.GZipStream(inputStream, System.IO.Compression.CompressionMode.Decompress);
        using var outputStream = new MemoryStream();

        gzipStream.CopyTo(outputStream);
        var bytes = outputStream.ToArray();
        var json = Encoding.UTF8.GetString(bytes);

        return Deserialize<T>(json, settings);
    }

    /// <summary>
    /// 计算对象序列化后的大小（字节）
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="value">要序列化的对象</param>
    /// <param name="settings">自定义序列化设置（可选）</param>
    /// <returns>字节大小</returns>
    public static int GetSerializedSize<T>(T value, JsonSerializerSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        var json = Serialize(value, settings);
        return Encoding.UTF8.GetByteCount(json);
    }

    /// <summary>
    /// 克隆对象（通过序列化/反序列化）
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="value">要克隆的对象</param>
    /// <param name="settings">自定义序列化设置（可选）</param>
    /// <returns>克隆的对象</returns>
    public static T? Clone<T>(T value, JsonSerializerSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        var json = Serialize(value, settings);
        return Deserialize<T>(json, settings);
    }
}
