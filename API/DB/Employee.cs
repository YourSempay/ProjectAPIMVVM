using System;
using System.Collections.Generic;

namespace API.DB;

public partial class Employee
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Position { get; set; }

    public DateTime? HireDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Credential> Credentials { get; set; } = new List<Credential>();

    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
