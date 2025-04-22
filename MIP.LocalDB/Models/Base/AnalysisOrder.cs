namespace MIP.Base.Models;

public partial class AnalysisOrder
{
	public int Id { get; set; }

	public int UserId { get; set; }

	public int PatientId { get; set; }

	public DateTime RegistrationDate { get; set; }

	public int? PatientAnalysisCartId { get; set; }

	public DateTime AnalysisDatetime { get; set; }

	public string? Comment { get; set; }

	public int PatientAnalysisAddressId { get; set; }

	public virtual Patient Patient { get; set; } = null!;

	public virtual PatientAnalysisAddress PatientAnalysisAddress { get; set; } = null!;

	public virtual PatientAnalysisCart? PatientAnalysisCart { get; set; }

	public virtual User User { get; set; } = null!;
}