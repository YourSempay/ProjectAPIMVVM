namespace API.DTO.DBDto;

public class EmployeeDto
{
    public int Id { get; set; }
    
    public string? FirstName { get; set; } = null!;

    public string? LastName { get; set; } = null!;

    public string? Position { get; set; } = null!;

    public DateTime? HireDate { get; set; }
    
    public bool? IsActive { get; set; }
}