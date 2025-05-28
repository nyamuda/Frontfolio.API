using Frontfolio.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
public class JwtService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    public JwtService(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }


    public async string GenerateJwtToken(int userId, double expirationInMinutes=10)
    {
        //get the details of the user the token is for
        var user = await  _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
        if (user is null) throw new KeyNotFoundException($"User with ID {userId} does not exist.");

        //get the JWT settings
        string jwtIssuer = _config.GetValue<string>("Authentication:Jwt:Issuer") ?? throw new KeyNotFoundException("Jwt issuer field not found.");
        string jwtAudience = _config.GetValue<string>("Authentication:Jwt:Audience") ?? throw new KeyNotFoundException("Jwt audience field not found.");
        string jwtKey = _config.GetValue<string>("Authentication:Jwt:Key") ?? throw new KeyNotFoundException("Jwt key field not found.");

        //encode the key and create the JWT signature
        var encodedJwtKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(encodedJwtKey, SecurityAlgorithms.HmacSha256);



        //Create claims for the token
        // Claims represent data stored in the token
        Claim[] claims = [
            new Claim(ClaimTypes.Role,user.Role.ToString()),
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Email,user.Email),
            new Claim("isVerified",user.isVerified.ToString().ToLower())
        ];

        //finally create the token
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            signingCredentials: credentials,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expirationInMinutes)
            );


        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}
