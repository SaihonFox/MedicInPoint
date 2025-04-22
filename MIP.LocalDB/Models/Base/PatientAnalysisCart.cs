namespace MIP.Base.Models;

public partial class PatientAnalysisCart
{
	public int Id { get; set; }

	public int PatientId { get; set; }

	public virtual ICollection<AnalysisOrder> AnalysisOrders { get; set; } = [];

	public virtual Patient Patient { get; set; } = null!;

	public virtual ICollection<PatientAnalysisCartItem> PatientAnalysisCartItems { get; set; } = [];
}