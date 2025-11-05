namespace WSharp.Authentication.OAuth;

/// <summary>
/// OAuth 提供商
/// </summary>
public enum OAuthProvider
{
    /// <summary>
    /// Google
    /// </summary>
    Google,

    /// <summary>
    /// Microsoft
    /// </summary>
    Microsoft,

    /// <summary>
    /// Facebook
    /// </summary>
    Facebook,

    /// <summary>
    /// GitHub
    /// </summary>
    GitHub
}

/// <summary>
/// OAuth 配置选项
/// </summary>
public class OAuthOptions
{
    /// <summary>
    /// 客户端 ID
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// 客户端密钥
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// 回调路径
    /// </summary>
    public string? CallbackPath { get; set; }

    /// <summary>
    /// 作用域
    /// </summary>
    public List<string> Scopes { get; set; } = new();

    /// <summary>
    /// 是否保存令牌
    /// </summary>
    public bool SaveTokens { get; set; } = true;
}

/// <summary>
/// Google OAuth 配置选项
/// </summary>
public class GoogleOAuthOptions : OAuthOptions
{
    public GoogleOAuthOptions()
    {
        this.Scopes = new List<string> { "openid", "profile", "email" };
        this.CallbackPath = "/signin-google";
    }
}

/// <summary>
/// Microsoft OAuth 配置选项
/// </summary>
public class MicrosoftOAuthOptions : OAuthOptions
{
    public MicrosoftOAuthOptions()
    {
        this.Scopes = new List<string> { "User.Read" };
        this.CallbackPath = "/signin-microsoft";
    }
}

/// <summary>
/// Facebook OAuth 配置选项
/// </summary>
public class FacebookOAuthOptions : OAuthOptions
{
    public FacebookOAuthOptions()
    {
        this.Scopes = new List<string> { "email", "public_profile" };
        this.CallbackPath = "/signin-facebook";
    }
}

/// <summary>
/// GitHub OAuth 配置选项
/// </summary>
public class GitHubOAuthOptions : OAuthOptions
{
    public GitHubOAuthOptions()
    {
        this.Scopes = new List<string> { "user:email" };
        this.CallbackPath = "/signin-github";
    }
}
