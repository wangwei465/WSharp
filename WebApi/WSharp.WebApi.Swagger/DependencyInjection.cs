using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using WSharp.WebApi.Swagger.Filters;

namespace WSharp.WebApi.Swagger;

/// <summary>
/// Swagger 依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 WSharp Swagger
    /// </summary>
    public static IServiceCollection AddWSharpSwagger(
        this IServiceCollection services,
        Action<SwaggerOptions>? configureOptions = null)
    {
        var options = new SwaggerOptions();
        configureOptions?.Invoke(options);

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            // 基本信息
            c.SwaggerDoc(options.DocumentName, new OpenApiInfo
            {
                Title = options.Title,
                Version = options.Version,
                Description = options.Description,
                Contact = options.Contact,
                License = options.License
            });

            // 服务器列表
            if (options.Servers.Count > 0)
            {
                foreach (var server in options.Servers)
                {
                    c.AddServer(server);
                }
            }

            // JWT Bearer 认证
            if (options.EnableJwtBearer)
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            }

            // OAuth2 认证
            if (options.EnableOAuth2 && !string.IsNullOrEmpty(options.OAuth2AuthorizationUrl))
            {
                c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(options.OAuth2AuthorizationUrl),
                            TokenUrl = string.IsNullOrEmpty(options.OAuth2TokenUrl)
                                ? null
                                : new Uri(options.OAuth2TokenUrl),
                            Scopes = options.OAuth2Scopes
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "OAuth2"
                            }
                        },
                        options.OAuth2Scopes.Keys.ToArray()
                    }
                });
            }

            // XML 注释文档
            foreach (var xmlPath in options.XmlDocumentPaths)
            {
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            }

            // 使用驼峰命名
            if (options.UseCamelCase)
            {
                c.DescribeAllParametersInCamelCase();
            }

            // 枚举显示为字符串
            if (options.UseEnumsAsStrings)
            {
                c.SchemaFilter<EnumSchemaFilter>();
            }

            // 添加操作过滤器
            c.OperationFilter<ResponseExampleFilter>();
            c.OperationFilter<FileUploadOperationFilter>();
            c.OperationFilter<HeaderParameterOperationFilter>();

            // 自定义 Schema ID
            c.CustomSchemaIds(type => type.FullName);
        });

        return services;
    }

    /// <summary>
    /// 使用 WSharp Swagger
    /// </summary>
    public static IApplicationBuilder UseWSharpSwagger(
        this IApplicationBuilder app,
        Action<SwaggerOptions>? configureOptions = null)
    {
        var options = new SwaggerOptions();
        configureOptions?.Invoke(options);

        app.UseSwagger(c =>
        {
            c.RouteTemplate = $"{options.RoutePrefix}/{{documentName}}/swagger.json";
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/{options.RoutePrefix}/{options.DocumentName}/swagger.json", options.Title);
            c.RoutePrefix = options.RoutePrefix;

            // UI 配置
            c.DocExpansion(DocExpansion.None);
            c.DefaultModelsExpandDepth(-1);
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
            c.EnableFilter();
            c.ShowExtensions();
        });

        return app;
    }
}

/// <summary>
/// 枚举 Schema 过滤器
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// 应用过滤器
    /// </summary>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Type = "string";
            schema.Enum = Enum.GetNames(context.Type)
                .Select(name => new Microsoft.OpenApi.Any.OpenApiString(name))
                .ToList<Microsoft.OpenApi.Any.IOpenApiAny>();
        }
    }
}
