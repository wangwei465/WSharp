namespace WSharp.Core.Shared.Extensions;

/// <summary>
/// 集合扩展方法
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// 判断集合是否为 null 或空
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">集合</param>
    /// <returns>如果集合为 null 或空，则为 true；否则为 false</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source is null || !source.Any();
    }

    /// <summary>
    /// 对集合中的每个元素执行指定操作
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="action">要执行的操作</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }

    /// <summary>
    /// 对集合中的每个元素执行指定操作（带索引）
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="action">要执行的操作</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        var index = 0;
        foreach (var item in source)
        {
            action(item, index++);
        }
    }

    /// <summary>
    /// 将集合分批
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="batchSize">批次大小</param>
    /// <returns>分批后的集合</returns>
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        if (batchSize <= 0)
        {
            throw new ArgumentException("批次大小必须大于 0", nameof(batchSize));
        }

        var batch = new List<T>(batchSize);
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }

        if (batch.Count > 0)
        {
            yield return batch;
        }
    }

    /// <summary>
    /// 去除集合中的重复元素（根据指定的键）
    /// </summary>
    /// <typeparam name="TSource">元素类型</typeparam>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="keySelector">键选择器</param>
    /// <returns>去重后的集合</returns>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }

    /// <summary>
    /// 将集合转换为字典（如果键重复则跳过）
    /// </summary>
    /// <typeparam name="TSource">元素类型</typeparam>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="keySelector">键选择器</param>
    /// <returns>字典</returns>
    public static Dictionary<TKey, TSource> ToSafeDictionary<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector) where TKey : notnull
    {
        var dictionary = new Dictionary<TKey, TSource>();
        foreach (var item in source)
        {
            var key = keySelector(item);
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = item;
            }
        }

        return dictionary;
    }

    /// <summary>
    /// 随机打乱集合
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">集合</param>
    /// <returns>打乱后的集合</returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        var random = new Random();
        return source.OrderBy(x => random.Next());
    }

    /// <summary>
    /// 连接集合中的字符串元素
    /// </summary>
    /// <param name="source">字符串集合</param>
    /// <param name="separator">分隔符</param>
    /// <returns>连接后的字符串</returns>
    public static string JoinString(this IEnumerable<string> source, string separator = ",")
    {
        return string.Join(separator, source);
    }

    /// <summary>
    /// 从集合中随机选择一个元素
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">集合</param>
    /// <returns>随机选择的元素</returns>
    public static T Random<T>(this IEnumerable<T> source)
    {
        var list = source as IList<T> ?? source.ToList();
        var random = new System.Random();
        var index = random.Next(list.Count);
        return list[index];
    }

    /// <summary>
    /// 从集合中随机选择指定数量的元素
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="count">数量</param>
    /// <returns>随机选择的元素集合</returns>
    public static IEnumerable<T> RandomTake<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    /// <summary>
    /// 判断集合中的所有元素是否唯一
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">集合</param>
    /// <returns>如果所有元素唯一，则为 true；否则为 false</returns>
    public static bool AllUnique<T>(this IEnumerable<T> source)
    {
        var set = new HashSet<T>();
        foreach (var item in source)
        {
            if (!set.Add(item))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 判断集合中是否包含任何指定的元素
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="values">要检查的元素</param>
    /// <returns>如果包含任何元素，则为 true；否则为 false</returns>
    public static bool ContainsAny<T>(this IEnumerable<T> source, params T[] values)
    {
        return source.Any(item => values.Contains(item));
    }

    /// <summary>
    /// 判断集合是否包含所有指定的元素
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="values">要检查的元素</param>
    /// <returns>如果包含所有元素，则为 true；否则为 false</returns>
    public static bool ContainsAll<T>(this IEnumerable<T> source, params T[] values)
    {
        var sourceSet = new HashSet<T>(source);
        return values.All(value => sourceSet.Contains(value));
    }
}
