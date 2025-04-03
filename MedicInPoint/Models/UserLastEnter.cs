using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class UserLastEnter
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateOnly LastEnterDate { get; set; }

    public User User { get; set; } = null!;
}
