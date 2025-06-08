namespace MedicInPoint.Models;

public partial class AnalysisOrder
{
    public int Id { get; set; }

    public bool AtHome { get; set; }

    public int UserId { get; set; }

    public int PatientId { get; set; }

    public DateTime RegistrationDate { get; set; }

    public int? PatientAnalysisCartId { get; set; }

    public DateTime AnalysisDatetime { get; set; }

    public string? Comment { get; set; }

    public int AnalysisOrderStateId { get; set; }

    public virtual AnalysisOrderState AnalysisOrderState { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual PatientAnalysisCart? PatientAnalysisCart { get; set; }

    public virtual User User { get; set; } = null!;
}
