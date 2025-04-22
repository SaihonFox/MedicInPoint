namespace MIP.Base.Models;

public partial class PatientAnalysisCartItem
{
	public int Id { get; set; }

	public int AnalysisId { get; set; }

	public int PatientAnalysisCartId { get; set; }

	public virtual Analysis Analysis { get; set; } = null!;

	public virtual PatientAnalysisCart PatientAnalysisCart { get; set; } = null!;
}