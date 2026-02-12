using MVVM.Models.Employees;

namespace MVVM.Models.Shifts;

public class ShiftEmployeeDto
{
    public EmployeeDto EmployeeD { get; set; }
    
    public ShiftDto ShiftD { get; set; }
}