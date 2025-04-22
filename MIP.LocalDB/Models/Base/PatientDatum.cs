namespace MIP.Base.Models;

public partial class PatientDatum
{
	public int Id { get; set; }

	public int PatientId { get; set; }

	public double? Weight { get; set; }

	public double? Height { get; set; }

	public virtual Patient Patient { get; set; } = null!;
}