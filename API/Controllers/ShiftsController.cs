using API.DB;
using API.DTO.DBDto;
using API.DTO.DoubleDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Route("api/[controller]")]
public class ShiftsController : Controller
{
    public _1135AlexandroContext db { get; set; }

    public ShiftsController(_1135AlexandroContext db)
    {
        this.db = db;
    }
    
    [HttpGet("")]
    public async Task<ActionResult<List<ShiftDto>>> Shifts()
    {
        var shifts= await db.Shifts.Include(s => s.Employee).ToListAsync();

        List<ShiftEmployeeDto> list = new List<ShiftEmployeeDto>();
        foreach (Shift shift in shifts)
        {
            list.Add(new ShiftEmployeeDto()
            {
                ShiftD = new ShiftDto()
                {
                    Id = shift.Id,
                    EmployeeId = shift.EmployeeId,
                    StartDateTime = shift.StartDateTime,
                    EndDateTime = shift.EndDateTime,
                    Description = shift.Description,
                },
                EmployeeD = new EmployeeDto()
                {
                    Id = shift.EmployeeId,
                    FirstName = shift.Employee.FirstName,
                    LastName = shift.Employee.LastName,
                }
            });
        }
        return Ok(list);   
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ShiftDto>> ShiftOnId(int id)
    {
        Shift? shift = await db.Shifts.Include(s=>s.Employee).FirstOrDefaultAsync(x => x.Id == id);
        return Ok(new ShiftEmployeeDto()
        {
            ShiftD = new ShiftDto()
            {
                Id = shift.Id,
                EmployeeId = shift.EmployeeId,
                StartDateTime = shift.StartDateTime,
                EndDateTime = shift.EndDateTime,
                Description = shift.Description,
            },
            EmployeeD = new EmployeeDto()
            {
                Id = shift.EmployeeId,
                FirstName = shift.Employee.FirstName,
                LastName = shift.Employee.LastName,
                Position = shift.Employee.Position,
                HireDate = shift.Employee.HireDate,
                IsActive = shift.Employee.IsActive,
            }
        });
    }
    
    [HttpGet("employee/{id}")]
    public async Task<ActionResult<List<ShiftDto>>> ShiftEmployeeOnId(int id)
    {
        DateTime oldestDate = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0, 0));
        List<Shift> list = await db.Shifts.Where(x => x.StartDateTime >= oldestDate && x.EmployeeId == id).ToListAsync();
        List<ShiftDto> listDto = new List<ShiftDto>();
        foreach (Shift shift in list)
        {
            listDto.Add(new ShiftDto()
            {
                Id = shift.Id,
                EmployeeId = shift.EmployeeId,
                StartDateTime =  shift.StartDateTime,  
                EndDateTime =  shift.EndDateTime,
                Description = shift.Description,
            });
        }
        return Ok(listDto);
    }
    
    [HttpPost("")]
    public  async Task<ActionResult> AddShift(ShiftDto shift)
    {
        if (await db.Employees.FirstOrDefaultAsync(x => x.Id == shift.EmployeeId) == null)
            return BadRequest("Нет пассажира");
        if(shift.StartDateTime > shift.EndDateTime)
            return BadRequest("Ты тупой");

        db.Shifts.Add(new Shift()
        {
            EmployeeId = shift.EmployeeId,
            StartDateTime = shift.StartDateTime,
            EndDateTime = shift.EndDateTime,
            Description = shift.Description,
        });
        await db.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateShift(int id,  [FromBody]ShiftDto shift)
    {
        Shift? shiftToUpdate = await db.Shifts.FirstOrDefaultAsync(x => x.Id == id);
        
        if (shiftToUpdate != null)
        {
            shiftToUpdate.StartDateTime = shift.StartDateTime;
            shiftToUpdate.EndDateTime = shift.EndDateTime;
            shiftToUpdate.Description = shift.Description;
        }

        await db.SaveChangesAsync();
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteShift(int id)
    {
        try
        {
            Shift? shift = await db.Shifts.FirstOrDefaultAsync(x => x.Id == id);
            if (shift != null)
                db.Shifts.Remove(shift);
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