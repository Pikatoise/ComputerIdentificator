using System;
using System.Collections.Generic;

namespace ComputersCourseWork;

public partial class Devicetype
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Computer> Computers { get; set; } = new List<Computer>();
}
