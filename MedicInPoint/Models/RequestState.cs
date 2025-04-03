using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class RequestState
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<Request> Requests { get; set; } = [];
}
