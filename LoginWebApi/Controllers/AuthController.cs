using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using LoginWebApi.Database;
using LoginWebApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoginController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = Request.Headers["x-auth-token"].FirstOrDefault();
        if (token == null)
        {
            return Unauthorized("Missing token");
        }
            
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }
            

        if (!IsJwtTokenValid(token))
        {
            return Unauthorized("Invalid token");
        }
            
        return Ok(new { message = "Login successful" });
    }

    private bool IsJwtTokenValid(string token)
    {
        var publicKey = System.IO.File.ReadAllText("PublicKeys/public.pem");

        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKey.ToCharArray());

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidateLifetime = true
        };

        var handler = new JwtSecurityTokenHandler();
        try
        {
            handler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
