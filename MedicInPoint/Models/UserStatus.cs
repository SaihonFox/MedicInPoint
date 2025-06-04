using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class UserStatus
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public bool IsFired { get; set; }

    public virtual User User { get; set; } = null!;
}
