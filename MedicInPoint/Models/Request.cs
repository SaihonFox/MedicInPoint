using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class Request
{
    public int Id { get; set; }

    public int? DoctorId { get; set; }

    public int? PatientId { get; set; }

    public int? PatientAnalysisCartId { get; set; }

    public DateTime AnalysisDatetime { get; set; }

    public string? Comment { get; set; }

    public int PatientAnalysisAddressId { get; set; }

    public int? RequestStateId { get; set; }

    public DateTime RequestSended { get; set; }

    public DateTime? RequestChanged { get; set; }

    public User? Doctor { get; set; }

    public Patient? Patient { get; set; }

    public PatientAnalysisAddress PatientAnalysisAddress { get; set; } = null!;

    public PatientAnalysisCart? PatientAnalysisCart { get; set; }

    public RequestState? RequestState { get; set; }
}
