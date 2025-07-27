using System;
using System.Collections.Generic;

namespace Dormitory_Management.Models;

public partial class Student
{
    public int Id { get; set; }

    public string Mobile { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Fname { get; set; } = null!;

    public string Mname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Paddress { get; set; } = null!;

    public string Idproof { get; set; } = null!;

    public long? RoomNo { get; set; }

    public string? Living { get; set; }

    public virtual ICollection<Fee> Fees { get; set; } = new List<Fee>();

    public virtual Room? RoomNoNavigation { get; set; }
}
