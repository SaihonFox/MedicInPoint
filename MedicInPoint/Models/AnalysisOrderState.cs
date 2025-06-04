using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class AnalysisOrderState
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<AnalysisOrder> AnalysisOrders { get; set; } = new List<AnalysisOrder>();
}
