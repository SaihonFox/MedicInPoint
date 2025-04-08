namespace MedicInPoint.Models;

public partial class RequestAnalysis
{
	public int Id { get; set; }

	public int RequestId { get; set; }

	public int AnalysisId { get; set; }

	public virtual Analysis Analysis { get; set; } = null!;

	public virtual Request Request { get; set; } = null!;
}