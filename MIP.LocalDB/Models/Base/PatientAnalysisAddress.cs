namespace MIP.Base.Models;

public partial class PatientAnalysisAddress
{
	public int Id { get; set; }

	public string Address { get; set; } = null!;

	public int? Floor { get; set; }

	public int? Room { get; set; }

	public int? Entrance { get; set; }

	public string? Intercome { get; set; }

	public virtual ICollection<AnalysisOrder> AnalysisOrders { get; set; } = [];

	public virtual ICollection<Request> Requests { get; set; } = [];
}