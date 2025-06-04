using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class Patient
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronym { get; set; }

    public DateOnly Birthday { get; set; }

    public string? Passport { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string Sex { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Image { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<AnalysisOrder> AnalysisOrders { get; set; } = new List<AnalysisOrder>();

    public virtual ICollection<PatientAnalysisCart> PatientAnalysisCarts { get; set; } = new List<PatientAnalysisCart>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
