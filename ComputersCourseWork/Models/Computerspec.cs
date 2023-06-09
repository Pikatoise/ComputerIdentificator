using System;
using System.Collections.Generic;

namespace ComputersCourseWork;

public partial class Computerspec
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Value { get; set; }

    public int? Computer { get; set; }

    public bool IsNetwork { get; set; } // Custom

    public virtual Computer? ComputerNavigation { get; set; }

}
