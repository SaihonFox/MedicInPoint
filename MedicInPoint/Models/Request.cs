using Newtonsoft.Json;

namespace MedicInPoint.Models;

public partial class Request
{
	public int Id { get; set; }

	public int DoctorId { get; set; }

	public int PatientId { get; set; }

	public DateTime AnalysisDatetime { get; set; }

	public string? Comment { get; set; }

	public int PatientAnalysisAddressId { get; set; }

	/// <summary>
	/// 1 - request in 
	/// false - request declined
	/// </summary>
	public int? RequestStateId { get; set; }

	public DateTime RequestSended { get; set; }

	/// <summary>
	/// Время изменения запроса на запись
	/// </summary>
	public DateTime? RequestChanged { get; set; }

	public virtual User Doctor { get; set; } = null!;

	public virtual Patient Patient { get; set; } = null!;

	public virtual PatientAnalysisAddress PatientAnalysisAddress { get; set; } = null!;

	public virtual ICollection<RequestAnalysis> RequestAnalyses { get; set; } = [];

	public virtual RequestState? RequestState { get; set; }
}