using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MIP.Base.Models;

public partial class Request
{
	[Key]
	public int Id { get; set; }

	public int DoctorId { get; set; }

	public int PatientId { get; set; }

	public DateTime AnalysisDatetime { get; set; }

	public string? PatientComment { get; set; }

	public int PatientAnalysisAddressId { get; set; }

	/// <summary>
	/// 1 - request in procces
	/// 2 - request declined
	/// 3 - request accepted
	/// </summary>
	[DefaultValue(1)]
	public int RequestStateId { get; set; } = 1;

	/// <summary>
	/// Время создания запроса на запись
	/// </summary>
	public DateTime RequestSended { get; set; }

	/// <summary>
	/// Время изменения запроса на запись
	/// </summary>
	public DateTime? RequestChanged { get; set; }

	public virtual User Doctor { get; set; } = null!;

	public virtual Patient Patient { get; set; } = null!;

	public virtual PatientAnalysisAddress PatientAnalysisAddress { get; set; } = null!;

	public virtual ICollection<RequestAnalysis> RequestAnalyses { get; set; } = [];

	public virtual RequestState RequestState { get; set; } = null!;
}