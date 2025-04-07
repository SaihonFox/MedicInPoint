namespace MedicInPoint.Models;

public partial class Analysis
{
	public int Id { get; set; }

	public string Name { get; set; } = null!;

	public string? Description { get; set; }

	public string? Preparation { get; set; }

	public string ResultsAfter { get; set; } = null!;

	public string Biomaterial { get; set; } = null!;

	public decimal Price { get; set; }

	public virtual ICollection<AdBlock> AdBlocks { get; set; } = [];

	public virtual ICollection<AnalysisCategoriesList> AnalysisCategoriesLists { get; set; } = [];

	public virtual ICollection<PatientAnalysisCartItem> PatientAnalysisCartItems { get; set; } = [];
}