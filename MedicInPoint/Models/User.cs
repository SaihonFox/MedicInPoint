using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class User
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronym { get; set; }

    public DateOnly Birthday { get; set; }

    public string? Passport { get; set; }

    public string? Phone { get; set; }

    public bool? IsBlocked { get; set; }

    public short? Post { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Image { get; set; }

    public virtual ICollection<AnalysisOrder> AnalysisOrders { get; set; } = new List<AnalysisOrder>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<UserData> UserStatuses { get; set; } = new List<UserData>();
}
