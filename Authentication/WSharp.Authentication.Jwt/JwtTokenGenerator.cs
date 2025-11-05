using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace WSharp.Authentication.Jwt;

/// <summary>
/// JWT Token 生成器
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _tokenValidationParameters;

    /// <summary>
    /// 初始化 JWT Token 生成器
    /// </summary>
    /// <param name="options">JWT 配置选项</param>
    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        this._options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        this._tokenHandler = new JwtSecurityTokenHandler();

        // 创建签名凭证
        var key = Encoding.UTF8.GetBytes(this._options.Secret);
        var securityKey = new SymmetricSecurityKey(key);
        this._signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 创建令牌验证参数
        this._tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = this._options.ValidateIssuer,
            ValidIssuer = this._options.Issuer,
            ValidateAudience = this._options.ValidateAudience,
            ValidAudience = this._options.Audience,
            ValidateLifetime = this._options.ValidateLifetime,
            ValidateIssuerSigningKey = this._options.ValidateIssuerSigningKey,
            IssuerSigningKey = securityKey,
            ClockSkew = TimeSpan.FromSeconds(this._options.ClockSkewSeconds)
        };
    }

    /// <summary>
    /// 生成访问令牌
    /// </summary>
    public string GenerateAccessToken(
        string userId,
        string userName,
        IEnumerable<string>? roles = null,
        Dictionary<string, string>? additionalClaims = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, userName),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // 添加角色
        if (roles != null)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        // 添加附加声明
        if (additionalClaims != null)
        {
            claims.AddRange(additionalClaims.Select(kvp => new Claim(kvp.Key, kvp.Value)));
        }

        return this.GenerateAccessToken(claims);
    }

    /// <summary>
    /// 生成访问令牌
    /// </summary>
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(this._options.ExpirationMinutes),
            Issuer = this._options.Issuer,
            Audience = this._options.Audience,
            SigningCredentials = this._signingCredentials
        };

        var token = this._tokenHandler.CreateToken(tokenDescriptor);
        return this._tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// 从令牌获取主体
    /// </summary>
    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        try
        {
            var principal = this._tokenHandler.ValidateToken(token, this._tokenValidationParameters, out var validatedToken);

            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 从令牌获取主体（不验证生命周期）
    /// </summary>
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        try
        {
            var tokenValidationParameters = this._tokenValidationParameters.Clone();
            tokenValidationParameters.ValidateLifetime = false;

            var principal = this._tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 验证令牌
    /// </summary>
    public bool ValidateToken(string token)
    {
        try
        {
            this._tokenHandler.ValidateToken(token, this._tokenValidationParameters, out var validatedToken);
            return IsJwtWithValidSecurityAlgorithm(validatedToken);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 验证是否是有效的 JWT 令牌
    /// </summary>
    private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return validatedToken is JwtSecurityToken jwtSecurityToken &&
               jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.Ordinal);
    }
}
