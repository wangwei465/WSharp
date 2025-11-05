namespace WSharp.Core.Shared.Extensions;

/// <summary>
/// 日期时间扩展方法
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// 判断日期是否为今天
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>如果是今天，则为 true；否则为 false</returns>
    public static bool IsToday(this DateTime date)
    {
        return date.Date == DateTime.Today;
    }

    /// <summary>
    /// 判断日期是否为过去
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>如果是过去，则为 true；否则为 false</returns>
    public static bool IsPast(this DateTime date)
    {
        return date < DateTime.Now;
    }

    /// <summary>
    /// 判断日期是否为将来
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>如果是将来，则为 true；否则为 false</returns>
    public static bool IsFuture(this DateTime date)
    {
        return date > DateTime.Now;
    }

    /// <summary>
    /// 获取日期所在周的第一天（周一）
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>周一的日期</returns>
    public static DateTime StartOfWeek(this DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// 获取日期所在周的最后一天（周日）
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>周日的日期</returns>
    public static DateTime EndOfWeek(this DateTime date)
    {
        return date.StartOfWeek().AddDays(6).Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// 获取日期所在月的第一天
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>月初的日期</returns>
    public static DateTime StartOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// 获取日期所在月的最后一天
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>月末的日期</returns>
    public static DateTime EndOfMonth(this DateTime date)
    {
        return date.StartOfMonth().AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// 获取日期所在年的第一天
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>年初的日期</returns>
    public static DateTime StartOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 1, 1);
    }

    /// <summary>
    /// 获取日期所在年的最后一天
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>年末的日期</returns>
    public static DateTime EndOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 12, 31, 23, 59, 59, 999);
    }

    /// <summary>
    /// 获取日期的开始时间（00:00:00）
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>当天的开始时间</returns>
    public static DateTime StartOfDay(this DateTime date)
    {
        return date.Date;
    }

    /// <summary>
    /// 获取日期的结束时间（23:59:59.999）
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>当天的结束时间</returns>
    public static DateTime EndOfDay(this DateTime date)
    {
        return date.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// 计算两个日期之间的年龄
    /// </summary>
    /// <param name="birthDate">出生日期</param>
    /// <param name="currentDate">当前日期（默认为今天）</param>
    /// <returns>年龄</returns>
    public static int GetAge(this DateTime birthDate, DateTime? currentDate = null)
    {
        var now = currentDate ?? DateTime.Today;
        var age = now.Year - birthDate.Year;

        if (now < birthDate.AddYears(age))
        {
            age--;
        }

        return age;
    }

    /// <summary>
    /// 判断日期是否在指定范围内
    /// </summary>
    /// <param name="date">日期</param>
    /// <param name="start">开始日期</param>
    /// <param name="end">结束日期</param>
    /// <returns>如果在范围内，则为 true；否则为 false</returns>
    public static bool IsBetween(this DateTime date, DateTime start, DateTime end)
    {
        return date >= start && date <= end;
    }

    /// <summary>
    /// 转换为 Unix 时间戳（秒）
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>Unix 时间戳</returns>
    public static long ToUnixTimeSeconds(this DateTime date)
    {
        return new DateTimeOffset(date).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 转换为 Unix 时间戳（毫秒）
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>Unix 时间戳</returns>
    public static long ToUnixTimeMilliseconds(this DateTime date)
    {
        return new DateTimeOffset(date).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 从 Unix 时间戳（秒）转换为日期
    /// </summary>
    /// <param name="unixTime">Unix 时间戳</param>
    /// <returns>日期</returns>
    public static DateTime FromUnixTimeSeconds(long unixTime)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
    }

    /// <summary>
    /// 从 Unix 时间戳（毫秒）转换为日期
    /// </summary>
    /// <param name="unixTime">Unix 时间戳</param>
    /// <returns>日期</returns>
    public static DateTime FromUnixTimeMilliseconds(long unixTime)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(unixTime).DateTime;
    }

    /// <summary>
    /// 判断是否为工作日（周一到周五）
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>如果是工作日，则为 true；否则为 false</returns>
    public static bool IsWeekday(this DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
    }

    /// <summary>
    /// 判断是否为周末（周六或周日）
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>如果是周末，则为 true；否则为 false</returns>
    public static bool IsWeekend(this DateTime date)
    {
        return !date.IsWeekday();
    }

    /// <summary>
    /// 添加工作日（跳过周末）
    /// </summary>
    /// <param name="date">日期</param>
    /// <param name="days">工作日数量</param>
    /// <returns>添加工作日后的日期</returns>
    public static DateTime AddWorkDays(this DateTime date, int days)
    {
        var result = date;
        var daysToAdd = Math.Abs(days);
        var increment = days > 0 ? 1 : -1;

        while (daysToAdd > 0)
        {
            result = result.AddDays(increment);
            if (result.IsWeekday())
            {
                daysToAdd--;
            }
        }

        return result;
    }

    /// <summary>
    /// 格式化为友好的时间描述（如"刚刚"、"5分钟前"等）
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>友好的时间描述</returns>
    public static string ToFriendlyString(this DateTime date)
    {
        var now = DateTime.Now;
        var span = now - date;

        if (span.TotalSeconds < 60)
        {
            return "刚刚";
        }

        if (span.TotalMinutes < 60)
        {
            return $"{(int)span.TotalMinutes} 分钟前";
        }

        if (span.TotalHours < 24)
        {
            return $"{(int)span.TotalHours} 小时前";
        }

        if (span.TotalDays < 7)
        {
            return $"{(int)span.TotalDays} 天前";
        }

        if (span.TotalDays < 30)
        {
            return $"{(int)(span.TotalDays / 7)} 周前";
        }

        if (span.TotalDays < 365)
        {
            return $"{(int)(span.TotalDays / 30)} 个月前";
        }

        return $"{(int)(span.TotalDays / 365)} 年前";
    }
}
