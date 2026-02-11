using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DTO.DBDto;
using API.DTO.DoubleDto;
using API.DTO.GagDto;
using API.Tools;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;
[Route("api/[controller]")]
public class AuthController : Controller
{
    public _1135AlexandroContext db { get; set; }

    public AuthController(_1135AlexandroContext db)
    {
        this.db = db;
    }
       [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login([FromBody]LoginDto request)
    {
        var credential = await db.Credentials.Include(x=> x.Role).FirstOrDefaultAsync(c=> c.Username == request.UsernameD);
        if (credential == null)
            return Unauthorized();
        
        bool isValidPassword = BCrypt.Net.BCrypt.Verify(
            request.PasswordD,
            credential.PasswordHash
        );

        if (!isValidPassword)
            return Unauthorized();

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, credential.Username),
            new Claim(ClaimTypes.Role, credential.Role.Title),
            new Claim("EmployeeId", credential.EmployeeId.ToString()),
        };

        var key = JwtSettings.GetSymmetricSecurityKey();
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expirensIn = 3600;

        var toker = new JwtSecurityToken(
            issuer: JwtSettings.ISSUER,
            audience: JwtSettings.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(expirensIn),
            signingCredentials: creds
        );
        return Ok(new TokenDto()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(toker),
            ExpiresIn = expirensIn
        });
    }
  
    
    [HttpPost("profile")]
    public async Task<ActionResult<EmloyeeRoleDto>> Profile()
    {
        var employeeId = int.Parse(User.FindFirst("EmployeeId")!.Value);

        var employee = await db.Employees.Include(e => e.Credentials).ThenInclude(c => c.Role).FirstOrDefaultAsync(e => e.Id == employeeId);

        if (employee == null)
            return NotFound();

        return Ok(new EmloyeeRoleDto
        {
            EmployeeD = new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Position = employee.Position,
            },
            RoleD = new RoleDto
            {
                Title = employee.Credentials.Last().Role.Title
            },
        });
    }
}