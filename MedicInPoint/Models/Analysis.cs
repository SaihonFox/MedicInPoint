using System.Collections.Generic;

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

	public int AnalysisCategoryId { get; set; }

	public ICollection<AdBlock> AdBlocks { get; set; } = [];

	public AnalysisCategory AnalysisCategory { get; set; } = null!;

	public ICollection<PatientAnalysisCartItem> PatientAnalysisCartItems { get; set; } = [];
}