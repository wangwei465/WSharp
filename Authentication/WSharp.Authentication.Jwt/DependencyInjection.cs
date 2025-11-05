using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WSharp.Authentication.Jwt;

/// <summary>
/// JWT 认证依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 JWT 认证
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        Action<JwtOptions> configureOptions)
    {
        // 配置选项
        services.Configure(configureOptions);

        var options = new JwtOptions();
        configureOptions(options);

        // 注册 JWT Token 生成器
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // 配置 JWT 认证
        var key = Encoding.UTF8.GetBytes(options.Secret);

        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(jwt =>
        {
            jwt.SaveToken = true;
            jwt.RequireHttpsMetadata = false;
            jwt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = options.ValidateIssuer,
                ValidIssuer = options.Issuer,
                ValidateAudience = options.ValidateAudience,
                ValidAudience = options.Audience,
                ValidateLifetime = options.ValidateLifetime,
                ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.FromSeconds(options.ClockSkewSeconds)
            };

            // 配置事件
            jwt.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        error = "未授权",
                        message = context.ErrorDescription ?? "需要有效的访问令牌"
                    });
                    return context.Response.WriteAsync(result);
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        error = "禁止访问",
                        message = "您没有权限访问此资源"
                    });
                    return context.Response.WriteAsync(result);
                }
            };
        });

        return services;
    }

    /// <summary>
    /// 添加 JWT 认证（使用配置文件）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <param name="sectionName">配置节名称</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration,
        string sectionName = "Jwt")
    {
        var jwtSection = configuration.GetSection(sectionName);
        services.Configure<JwtOptions>(jwtSection);

        var options = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

        return services.AddJwtAuthentication(opt =>
        {
            opt.Secret = options.Secret;
            opt.Issuer = options.Issuer;
            opt.Audience = options.Audience;
            opt.ExpirationMinutes = options.ExpirationMinutes;
            opt.RefreshTokenExpirationDays = options.RefreshTokenExpirationDays;
            opt.ValidateIssuer = options.ValidateIssuer;
            opt.ValidateAudience = options.ValidateAudience;
            opt.ValidateLifetime = options.ValidateLifetime;
            opt.ValidateIssuerSigningKey = options.ValidateIssuerSigningKey;
            opt.ClockSkewSeconds = options.ClockSkewSeconds;
        });
    }
}

/// <summary>
/// HTTP 状态码常量
/// </summary>
internal static class StatusCodes
{
    public const int Status401Unauthorized = 401;
    public const int Status403Forbidden = 403;
}
