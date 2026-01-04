namespace ProManage360.Application.Common.Interfaces;

/// <summary>
/// Password hashing service (BCrypt)
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash a plain text password
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verify password against hash
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}