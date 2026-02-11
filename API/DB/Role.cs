using System;
using System.Collections.Generic;

namespace API.DB;

public partial class Role
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Credential> Credentials { get; set; } = new List<Credential>();
}
