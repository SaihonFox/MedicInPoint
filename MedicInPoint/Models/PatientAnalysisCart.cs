using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class PatientAnalysisCart
{
    public int Id { get; set; }

    public int? PatientId { get; set; }

    public ICollection<AnalysisOrder> AnalysisOrders { get; set; } = [];

    public Patient? Patient { get; set; }

    public ICollection<PatientAnalysisCartItem> PatientAnalysisCartItems { get; set; } = [];

    public ICollection<Request> Requests { get; set; } = [];
}
