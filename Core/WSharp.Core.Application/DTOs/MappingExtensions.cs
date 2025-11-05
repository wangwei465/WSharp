using System.Reflection;
using AutoMapper;

namespace WSharp.Core.Application.DTOs;

/// <summary>
/// 映射扩展方法
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// 从程序集中应用所有映射配置
    /// </summary>
    /// <param name="profile">映射配置</param>
    /// <param name="assembly">程序集</param>
    public static void ApplyMappingsFromAssembly(this Profile profile, Assembly assembly)
    {
        var mapFromType = typeof(IMapFrom<>);
        var mapToType = typeof(IMapTo<>);

        var types = assembly.GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToList();

        foreach (var type in types)
        {
            // IMapFrom<T>
            var mapFromInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == mapFromType)
                .ToList();

            foreach (var mapFromInterface in mapFromInterfaces)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = mapFromInterface.GetMethod("Mapping");
                methodInfo?.Invoke(instance, new object[] { profile });
            }

            // IMapTo<T>
            var mapToInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == mapToType)
                .ToList();

            foreach (var mapToInterface in mapToInterfaces)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = mapToInterface.GetMethod("Mapping");
                methodInfo?.Invoke(instance, new object[] { profile });
            }
        }
    }
}
