using AutoMapper;

namespace WSharp.Core.Application.DTOs;

/// <summary>
/// 从源类型映射的接口
/// </summary>
/// <typeparam name="T">源类型</typeparam>
public interface IMapFrom<T>
{
    /// <summary>
    /// 配置映射规则
    /// </summary>
    /// <param name="profile">映射配置</param>
    void Mapping(Profile profile)
    {
        profile.CreateMap(typeof(T), this.GetType());
    }
}
