namespace MIP.Base.Models;

public partial class PatientsDataList
{
	public int Id { get; set; }

	public DateTime ChangeDt { get; set; }

	public int PatientId { get; set; }

	public string Method { get; set; } = null!;

	public string Surname { get; set; } = null!;

	public string Name { get; set; } = null!;

	public string? Patronym { get; set; }

	public string? Passport { get; set; }

	public string? Phone { get; set; }

	public string? Email { get; set; }

	public string Login { get; set; } = null!;

	public double? Weight { get; set; }

	public double? Height { get; set; }

	public virtual Patient Patient { get; set; } = null!;
}