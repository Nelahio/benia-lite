using BeniaLite.Api.Auth;
using BeniaLite.Api.Auth.Contracts;
using BeniaLite.Api.Data;
using BeniaLite.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace BeniaLite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly BeniaDbContext _db;
    private readonly IPasswordService _passwords;
    private readonly IJwtTokenService _jwt;

    public AuthController(BeniaDbContext db, IPasswordService passwords, IJwtTokenService jwt)
    {
        _db = db;
        _passwords = passwords;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        var email = req.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Email and password are required.");

        var exists = await _db.Users.AnyAsync(x => x.Email == email);
        if (exists) return Conflict("Email already registered.");

        var user = new User { Email = email };
        user.PasswordHash = _passwords.Hash(user, req.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.CreateAccessToken(user);
        return Ok(new AuthResponse(token));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        var email = req.Email.Trim().ToLowerInvariant();

        var user = await _db.Users.SingleOrDefaultAsync(x => x.Email == email);
        if (user is null) return Unauthorized("Invalid credentials.");

        if (!_passwords.Verify(user, req.Password))
            return Unauthorized("Invalid credentials.");

        var token = _jwt.CreateAccessToken(user);
        return Ok(new AuthResponse(token));
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        var userId = User.FindFirstValue("uid") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");

        return Ok(new
        {
            userId,
            email
        });
    }

}
