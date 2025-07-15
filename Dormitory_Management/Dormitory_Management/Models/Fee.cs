using System;
using System.Collections.Generic;

namespace Dormitory_Management.Models;

public partial class Fee
{
    public long MobileNo { get; set; }

    public string Fmonth { get; set; } = null!;

    public long Amount { get; set; }

    public virtual Student MobileNoNavigation { get; set; } = null!;
}
