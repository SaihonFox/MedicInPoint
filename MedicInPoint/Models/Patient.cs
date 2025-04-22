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

    public ICollection<AnalysisOrder> AnalysisOrders { get; set; } = [];

    public ICollection<Message> Messages { get; set; } = [];

    public ICollection<MessagesMessage> MessagesMessages { get; set; } = [];

    public ICollection<PatientAnalysisCart> PatientAnalysisCarts { get; set; } = [];

    public ICollection<PatientDatum> PatientData { get; set; } = [];

    public ICollection<PatientsDataList> PatientsDataLists { get; set; } = [];

    public ICollection<Request> Requests { get; set; } = [];
}
