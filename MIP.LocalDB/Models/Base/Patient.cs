using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MIP.Base.Models;

public partial class Patient
{
	[Key]
	public int Id { get; set; }

	public string Surname { get; set; } = null!;

	public string Name { get; set; } = null!;

	public string? Patronym { get; set; }

	public DateOnly Birthday { get; set; }

	public string? Passport { get; set; }

	public string? Phone { get; set; }

	public string? Email { get; set; }

	public string Sex { get; set; } = null!;

	public string Login { get; set; } = null!;

	[DefaultValue("123")]
	public string Password { get; set; } = null!;

	public byte[]? Image { get; set; }

	public virtual ICollection<AnalysisOrder> AnalysisOrders { get; set; } = [];

	public virtual ICollection<Message> Messages { get; set; } = [];

	public virtual ICollection<MessagesMessage> MessagesMessages { get; set; } = [];

	public virtual ICollection<PatientAnalysisCart> PatientAnalysisCarts { get; set; } = [];

	public virtual ICollection<PatientDatum> PatientData { get; set; } = [];

	public virtual ICollection<PatientsDataList> PatientsDataLists { get; set; } = [];

	public virtual ICollection<Request> Requests { get; set; } = [];
}