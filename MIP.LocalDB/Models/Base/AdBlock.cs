namespace MIP.Base.Models;

public partial class AdBlock
{
	public int Id { get; set; }

	public string Name { get; set; } = null!;

	public string? Description { get; set; }

	public string? Sex { get; set; }

	public decimal Price { get; set; }

	public DateTime? DateStart { get; set; }

	public DateTime? DateEnd { get; set; }

	public int? AnalysisId { get; set; }

	public virtual Analysis? Analysis { get; set; }
}