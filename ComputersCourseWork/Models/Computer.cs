using System;
using System.Collections.Generic;

namespace ComputersCourseWork;

public partial class Computer
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public string? inventoryNum { get; set; }

    public int Devicetype { get; set; }

    public DateOnly LastUpdate { get; set; }

    public string? Windowskey { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Computerspec> Computerspecs { get; set; } = new List<Computerspec>();

    public virtual Devicetype DevicetypeNavigation { get; set; } = null!;
}
