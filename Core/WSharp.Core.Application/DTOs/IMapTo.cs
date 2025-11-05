using AutoMapper;

namespace WSharp.Core.Application.DTOs;

/// <summary>
/// 映射到目标类型的接口
/// </summary>
/// <typeparam name="T">目标类型</typeparam>
public interface IMapTo<T>
{
    /// <summary>
    /// 配置映射规则
    /// </summary>
    /// <param name="profile">映射配置</param>
    void Mapping(Profile profile)
    {
        profile.CreateMap(this.GetType(), typeof(T));
    }
}
