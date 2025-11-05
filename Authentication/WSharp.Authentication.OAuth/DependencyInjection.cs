using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace WSharp.Authentication.OAuth;

/// <summary>
/// OAuth 认证依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加 Google OAuth 认证
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddGoogleOAuth(
        this IServiceCollection services,
        Action<GoogleOAuthOptions> configureOptions)
    {
        var options = new GoogleOAuthOptions();
        configureOptions(options);

        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            auth.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = "Google";
        })
        .AddCookie()
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = options.ClientId;
            googleOptions.ClientSecret = options.ClientSecret;
            googleOptions.SaveTokens = options.SaveTokens;

            if (!string.IsNullOrEmpty(options.CallbackPath))
            {
                googleOptions.CallbackPath = options.CallbackPath;
            }

            foreach (var scope in options.Scopes)
            {
                googleOptions.Scope.Add(scope);
            }
        });

        return services;
    }

    /// <summary>
    /// 添加 Microsoft OAuth 认证
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMicrosoftOAuth(
        this IServiceCollection services,
        Action<MicrosoftOAuthOptions> configureOptions)
    {
        var options = new MicrosoftOAuthOptions();
        configureOptions(options);

        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            auth.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = "Microsoft";
        })
        .AddCookie()
        .AddMicrosoftAccount(microsoftOptions =>
        {
            microsoftOptions.ClientId = options.ClientId;
            microsoftOptions.ClientSecret = options.ClientSecret;
            microsoftOptions.SaveTokens = options.SaveTokens;

            if (!string.IsNullOrEmpty(options.CallbackPath))
            {
                microsoftOptions.CallbackPath = options.CallbackPath;
            }

            foreach (var scope in options.Scopes)
            {
                microsoftOptions.Scope.Add(scope);
            }
        });

        return services;
    }

    /// <summary>
    /// 添加 Facebook OAuth 认证
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddFacebookOAuth(
        this IServiceCollection services,
        Action<FacebookOAuthOptions> configureOptions)
    {
        var options = new FacebookOAuthOptions();
        configureOptions(options);

        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            auth.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = "Facebook";
        })
        .AddCookie()
        .AddFacebook(facebookOptions =>
        {
            facebookOptions.AppId = options.ClientId;
            facebookOptions.AppSecret = options.ClientSecret;
            facebookOptions.SaveTokens = options.SaveTokens;

            if (!string.IsNullOrEmpty(options.CallbackPath))
            {
                facebookOptions.CallbackPath = options.CallbackPath;
            }

            foreach (var scope in options.Scopes)
            {
                facebookOptions.Scope.Add(scope);
            }
        });

        return services;
    }

    /// <summary>
    /// 添加 GitHub OAuth 认证
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddGitHubOAuth(
        this IServiceCollection services,
        Action<GitHubOAuthOptions> configureOptions)
    {
        var options = new GitHubOAuthOptions();
        configureOptions(options);

        services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            auth.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            auth.DefaultChallengeScheme = "GitHub";
        })
        .AddCookie()
        .AddGitHub(githubOptions =>
        {
            githubOptions.ClientId = options.ClientId;
            githubOptions.ClientSecret = options.ClientSecret;
            githubOptions.SaveTokens = options.SaveTokens;

            if (!string.IsNullOrEmpty(options.CallbackPath))
            {
                githubOptions.CallbackPath = options.CallbackPath;
            }

            foreach (var scope in options.Scopes)
            {
                githubOptions.Scope.Add(scope);
            }
        });

        return services;
    }

    /// <summary>
    /// 添加多个 OAuth 提供商
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureProviders">配置提供商</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddOAuthProviders(
        this IServiceCollection services,
        Action<OAuthProviderBuilder> configureProviders)
    {
        var builder = new OAuthProviderBuilder(services);
        configureProviders(builder);
        return services;
    }
}

/// <summary>
/// OAuth 提供商构建器
/// </summary>
public class OAuthProviderBuilder
{
    private readonly IServiceCollection _services;
    private readonly AuthenticationBuilder _authBuilder;
    private bool _cookieAdded;

    /// <summary>
    /// 初始化构建器
    /// </summary>
    public OAuthProviderBuilder(IServiceCollection services)
    {
        this._services = services;
        this._authBuilder = services.AddAuthentication(auth =>
        {
            auth.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            auth.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        });
    }

    /// <summary>
    /// 添加 Google
    /// </summary>
    public OAuthProviderBuilder AddGoogle(Action<GoogleOAuthOptions> configureOptions)
    {
        this.EnsureCookie();
        var options = new GoogleOAuthOptions();
        configureOptions(options);

        this._authBuilder.AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = options.ClientId;
            googleOptions.ClientSecret = options.ClientSecret;
            googleOptions.SaveTokens = options.SaveTokens;

            if (!string.IsNullOrEmpty(options.CallbackPath))
            {
                googleOptions.CallbackPath = options.CallbackPath;
            }

            foreach (var scope in options.Scopes)
            {
                googleOptions.Scope.Add(scope);
            }
        });

        return this;
    }

    /// <summary>
    /// 添加 Microsoft
    /// </summary>
    public OAuthProviderBuilder AddMicrosoft(Action<MicrosoftOAuthOptions> configureOptions)
    {
        this.EnsureCookie();
        var options = new MicrosoftOAuthOptions();
        configureOptions(options);

        this._authBuilder.AddMicrosoftAccount(microsoftOptions =>
        {
            microsoftOptions.ClientId = options.ClientId;
            microsoftOptions.ClientSecret = options.ClientSecret;
            microsoftOptions.SaveTokens = options.SaveTokens;

            if (!string.IsNullOrEmpty(options.CallbackPath))
            {
                microsoftOptions.CallbackPath = options.CallbackPath;
            }

            foreach (var scope in options.Scopes)
            {
                microsoftOptions.Scope.Add(scope);
            }
        });

        return this;
    }

    /// <summary>
    /// 添加 Facebook
    /// </summary>
    public OAuthProviderBuilder AddFacebook(Action<FacebookOAuthOptions> configureOptions)
    {
        this.EnsureCookie();
        var options = new FacebookOAuthOptions();
        configureOptions(options);

        this._authBuilder.AddFacebook(facebookOptions =>
        {
            facebookOptions.AppId = options.ClientId;
            facebookOptions.AppSecret = options.ClientSecret;
            facebookOptions.SaveTokens = options.SaveTokens;

            if (!string.IsNullOrEmpty(options.CallbackPath))
            {
                facebookOptions.CallbackPath = options.CallbackPath;
            }

            foreach (var scope in options.Scopes)
            {
                facebookOptions.Scope.Add(scope);
            }
        });

        return this;
    }

    /// <summary>
    /// 添加 GitHub
    /// </summary>
    public OAuthProviderBuilder AddGitHub(Action<GitHubOAuthOptions> configureOptions)
    {
        this.EnsureCookie();
        var options = new GitHubOAuthOptions();
        configureOptions(options);

        this._authBuilder.AddGitHub(githubOptions =>
        {
            githubOptions.ClientId = options.ClientId;
            githubOptions.ClientSecret = options.ClientSecret;
            githubOptions.SaveTokens = options.SaveTokens;

            if (!string.IsNullOrEmpty(options.CallbackPath))
            {
                githubOptions.CallbackPath = options.CallbackPath;
            }

            foreach (var scope in options.Scopes)
            {
                githubOptions.Scope.Add(scope);
            }
        });

        return this;
    }

    private void EnsureCookie()
    {
        if (!this._cookieAdded)
        {
            this._authBuilder.AddCookie();
            this._cookieAdded = true;
        }
    }
}
