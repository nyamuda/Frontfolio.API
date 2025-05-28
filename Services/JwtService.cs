using Frontfolio.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
public class JwtService
{
    private readonly ApplicationDbContext _context;
    private string _jwtIssuer = string.Empty;
    private string _jwtAudience =string.Empty ;
    private string _jwtKey =string.Empty;
    private SigningCredentials _signingCredentials;

    public JwtService(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
       
        //get the JWT settings
        _jwtIssuer = config.GetValue<string>("Authentication:Jwt:Issuer") ?? throw new KeyNotFoundException("Jwt issuer field not found.");
        _jwtAudience = config.GetValue<string>("Authentication:Jwt:Audience") ?? throw new KeyNotFoundException("Jwt audience field not found.");
        _jwtKey = config.GetValue<string>("Authentication:Jwt:Key") ?? throw new KeyNotFoundException("Jwt key field not found.");

        //encode the key and create the signing credentials
        var encodedJwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        _signingCredentials = new SigningCredentials(encodedJwtKey, SecurityAlgorithms.HmacSha256);
    }


    public async Task<string> GenerateJwtToken(int userId, double expirationInMinutes=10)
    {
        //get the details of the user the token is for
        var user = await  _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
        if (user is null) throw new KeyNotFoundException($"User with ID {userId} does not exist.");

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
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            signingCredentials: _signingCredentials,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expirationInMinutes)
            );


        return new JwtSecurityTokenHandler().WriteToken(token);

    }


    public ClaimsPrincipal ValidateJwtToken(string token) { 

    }
}
