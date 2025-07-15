using System;
using System.Collections.Generic;

namespace Dormitory_Management.Models;

public partial class Room
{
    public long RoomNo { get; set; }

    public string RoomStatus { get; set; } = null!;

    public string? Booked { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
