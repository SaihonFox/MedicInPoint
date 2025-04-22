namespace MIP.Base.Models;

public partial class AnalysisCategoriesList
{
	public int Id { get; set; }

	public int AnalysisId { get; set; }

	public int AnalysisCategoryId { get; set; }

	public virtual Analysis Analysis { get; set; } = null!;

	public virtual AnalysisCategory AnalysisCategory { get; set; } = null!;
}