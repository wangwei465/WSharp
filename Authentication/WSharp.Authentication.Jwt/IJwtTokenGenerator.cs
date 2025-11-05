using System.Security.Claims;

namespace WSharp.Authentication.Jwt;

/// <summary>
/// JWT Token 生成器接口
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// 生成访问令牌
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="userName">用户名</param>
    /// <param name="roles">角色列表</param>
    /// <param name="additionalClaims">附加声明</param>
    /// <returns>访问令牌</returns>
    string GenerateAccessToken(
        string userId,
        string userName,
        IEnumerable<string>? roles = null,
        Dictionary<string, string>? additionalClaims = null);

    /// <summary>
    /// 生成访问令牌
    /// </summary>
    /// <param name="claims">声明列表</param>
    /// <returns>访问令牌</returns>
    string GenerateAccessToken(IEnumerable<Claim> claims);

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// 从令牌获取主体
    /// </summary>
    /// <param name="token">令牌</param>
    /// <returns>声明主体</returns>
    ClaimsPrincipal? GetPrincipalFromToken(string token);

    /// <summary>
    /// 从令牌获取主体（不验证生命周期）
    /// </summary>
    /// <param name="token">令牌</param>
    /// <returns>声明主体</returns>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    /// <summary>
    /// 验证令牌
    /// </summary>
    /// <param name="token">令牌</param>
    /// <returns>是否有效</returns>
    bool ValidateToken(string token);
}
