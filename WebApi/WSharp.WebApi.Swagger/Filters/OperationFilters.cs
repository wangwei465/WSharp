using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WSharp.WebApi.Swagger.Filters;

/// <summary>
/// 添加通用响应示例过滤器
/// </summary>
public class ResponseExampleFilter : IOperationFilter
{
    /// <summary>
    /// 应用过滤器
    /// </summary>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // 为所有操作添加 400 和 500 响应
        if (!operation.Responses.ContainsKey("400"))
        {
            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "请求参数错误",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ApiResponse), context.SchemaRepository)
                    }
                }
            });
        }

        if (!operation.Responses.ContainsKey("500"))
        {
            operation.Responses.Add("500", new OpenApiResponse
            {
                Description = "服务器内部错误",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ApiResponse), context.SchemaRepository)
                    }
                }
            });
        }
    }
}

/// <summary>
/// 添加文件上传参数过滤器
/// </summary>
public class FileUploadOperationFilter : IOperationFilter
{
    /// <summary>
    /// 应用过滤器
    /// </summary>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParameters = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile) ||
                       p.ParameterType == typeof(IFormFileCollection) ||
                       p.ParameterType == typeof(IEnumerable<IFormFile>));

        if (fileParameters.Any())
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = fileParameters.ToDictionary(
                                p => p.Name ?? "file",
                                p => new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                })
                        }
                    }
                }
            };
        }
    }
}

/// <summary>
/// 添加请求头参数过滤器
/// </summary>
public class HeaderParameterOperationFilter : IOperationFilter
{
    /// <summary>
    /// 应用过滤器
    /// </summary>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // 为需要认证的操作添加 Authorization 头说明
        if (operation.Security?.Any() == true)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            var hasAuthorizationHeader = operation.Parameters.Any(p =>
                p.Name.Equals("Authorization", StringComparison.OrdinalIgnoreCase) &&
                p.In == ParameterLocation.Header);

            if (!hasAuthorizationHeader)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "JWT Bearer 令牌",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Default = new OpenApiString("Bearer {token}")
                    }
                });
            }
        }
    }
}
