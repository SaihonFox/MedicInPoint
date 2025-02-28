using System;

namespace MedicInPoint.Models;

public partial class AnalysisOrder
{
	public int Id { get; set; }

	public int? UserId { get; set; }

	public int? PatientId { get; set; }

	public DateTime RegistrationDate { get; set; }

	public int? PatientAnalysisCartId { get; set; }

	public DateTime AnalysisDatetime { get; set; }

	public string? Comment { get; set; }

	public int PatientAnalysisAddressId { get; set; }

	public Patient? Patient { get; set; }

	public PatientAnalysisAddress PatientAnalysisAddress { get; set; } = null!;

	public PatientAnalysisCart? PatientAnalysisCart { get; set; }

	public User? User { get; set; }
}