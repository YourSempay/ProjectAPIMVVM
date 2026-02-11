using System;
using System.Collections.Generic;

namespace API.DB;

public partial class Shift
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public DateTime? StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public string? Description { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
