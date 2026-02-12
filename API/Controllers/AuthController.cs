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
        // ищем пользователя по логину, подгружаем роль
        var credential = await db.Credentials.Include(x=> x.Role).FirstOrDefaultAsync(c=> c.Username == request.UsernameD);
        if (credential == null)
            return Unauthorized(); // 401, если нет такого логина
        
        // проверка пароля через BCrypt(BCrypt Nuget для хэширования пароля (Команда  HashPassword)
        bool isValidPassword = BCrypt.Net.BCrypt.Verify(
            request.PasswordD,
            credential.PasswordHash 
        );

        if (!isValidPassword)
            return Unauthorized(); // 401, если пароль неверный

        
        // создаём claims — данные пользователя для токена
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, credential.Username),
            new Claim(ClaimTypes.Role, credential.Role.Title),
            new Claim("EmployeeId", credential.EmployeeId.ToString()),
        };
        // ключ для подписи токена
        var key = JwtSettings.GetSymmetricSecurityKey();
        // SigningCredentials — это объект, который говорит серверу, как подписывать JWT, чтобы клиент не мог подделать токен.
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // срок действия токена 1 час
        var expirensIn = 3600;

        // создаём JWT
        var toker = new JwtSecurityToken(
            issuer: JwtSettings.ISSUER,
            audience: JwtSettings.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(expirensIn),
            signingCredentials: creds
        );
        
        // возвращаем клиенту токен
        return Ok(new TokenDto()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(toker),
            ExpiresIn = expirensIn
        });
    }
  
    
    [HttpPost("profile")]
    public async Task<ActionResult<EmloyeeRoleDto>> Profile()
    {
        // извлекаем EmployeeId из токена (claims)
        var employeeId = int.Parse(User.FindFirst("EmployeeId")!.Value);
        // получаем сотрудника из БД, подгружаем роль
        var employee = await db.Employees.Include(e => e.Credentials).ThenInclude(c => c.Role).FirstOrDefaultAsync(e => e.Id == employeeId);

        if (employee == null)
            return NotFound(); // 404 если не найден

        // возвращаем DTO клиенту
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