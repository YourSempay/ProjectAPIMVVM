using API.DB;
using API.DTO.DBDto;
using API.DTO.DoubleDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Route("api/[controller]")]
public class EmployeesController : Controller
{
    public _1135AlexandroContext db { get; set; }

    public EmployeesController(_1135AlexandroContext db)
    {
        this.db = db;
    }
    
     [HttpGet("All")]
    public async Task<ActionResult<List<EmloyeeRoleDto>>> Employees()
    {
        var employees = await db.Employees.Include(s => s.Credentials).ThenInclude(s => s.Role).ToListAsync();

        List<EmloyeeRoleDto> list = new List<EmloyeeRoleDto>();
        foreach (Employee employee in employees)
        {
            list.Add(new EmloyeeRoleDto()
            {
                EmployeeD = new EmployeeDto()
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Position =  employee.Position,
                    HireDate =  employee.HireDate,
                    IsActive =  employee.IsActive,
                },
                RoleD = new RoleDto()
                {
                    Title = employee.Credentials.Last().Role.Title
                }
            });
        }
        return Ok(list);
    }
    
      [HttpGet("Search/{id}")]
    public async Task<ActionResult<EmloyeeRoleDto>> EmployeeOnId(int id)
    {
        Employee? employee = await db.Employees.Include(s=>s.Credentials).ThenInclude(s=>s.Role).FirstOrDefaultAsync(x => x.Id == id);
        return Ok(new EmloyeeRoleDto()
        {
            EmployeeD = new EmployeeDto()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Position =  employee.Position,
                HireDate =  employee.HireDate,
                IsActive =  employee.IsActive,
            },
            RoleD = new RoleDto()
            {
                Title = employee.Credentials.Last().Role.Title
            }
        });
    }
    
    [HttpPost("Add")]
    public  async Task<ActionResult> AddEmployee(CredentialEmloyeeDto employeeCred)
    {
        
        if (await db.Credentials.FirstOrDefaultAsync(x => x.Username == employeeCred.CredentialD.Username) != null)
            return BadRequest("");
        
        Employee employee = new Employee
        {
            FirstName = employeeCred.EmployeeD.FirstName,
            LastName = employeeCred.EmployeeD.LastName,
            Position = employeeCred.EmployeeD.Position,
            HireDate = employeeCred.EmployeeD.HireDate,
            IsActive = employeeCred.EmployeeD.IsActive,
        }; 
        db.Employees.Add(employee);
        await db.SaveChangesAsync();
        
        Credential credential = new Credential
        {
            Username = employeeCred.CredentialD.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(employeeCred.CredentialD.PasswordHash),
            RoleId = employeeCred.CredentialD.RoleId,
            EmployeeId = employee.Id,
        };
        db.Credentials.Add(credential);
        await db.SaveChangesAsync();
        
        CredentialDto credentialDto = new CredentialDto()
        {
            Id =  credential.Id,
            Username = credential.Username,
            PasswordHash = credential.PasswordHash,
            RoleId = credential.RoleId,
            EmployeeId = credential.EmployeeId,
        };
        
        return Created($"", credentialDto);
    }
    
    [HttpPut("Update/{id}")]
    public  async Task<ActionResult> UpdateEmployee(int id,  [FromBody]EmployeeDto employeeDto)
    {
        Employee? employee = await db.Employees.FirstOrDefaultAsync(x => x.Id == id);

        if (employee != null)
        {
            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.Position = employeeDto.Position;
            employee.HireDate = employeeDto.HireDate;
            employee.IsActive = employeeDto.IsActive;
        }

        await db.SaveChangesAsync();
        return Ok();
    } 
    
    
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult> DeleteEmployee(int id)
    {
        try
        {
            Employee? employee = await db.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employee != null) 
                db.Employees.Remove(employee);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        await db.SaveChangesAsync();
        return NoContent();
    }
}