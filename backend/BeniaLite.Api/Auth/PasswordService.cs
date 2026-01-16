using BeniaLite.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace BeniaLite.Api.Auth;

public interface IPasswordService
{
    string Hash(User user, string password);
    bool Verify(User user, string password);
}

public sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(User user, string password)
        => _hasher.HashPassword(user, password);

    public bool Verify(User user, string password)
        => _hasher.VerifyHashedPassword(user, user.PasswordHash, password)
           is PasswordVerificationResult.Success
           or PasswordVerificationResult.SuccessRehashNeeded;
}
