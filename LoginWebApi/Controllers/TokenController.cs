using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using LoginWebApi.Database;
using LoginWebApi.Models;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly AppDbContext _context;

    public TokenController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateToken([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var privateKey = System.IO.File.ReadAllText("Keys/private.pem");
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey.ToCharArray());

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("role", "user")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = new DateTime(2025, 8, 1),
            Issuer = "LoginWebApi",
            Audience = "LoginWebApi",
            SigningCredentials = signingCredentials
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        var jwt = handler.WriteToken(token);

        return Ok(new { token = jwt });
    }

}
