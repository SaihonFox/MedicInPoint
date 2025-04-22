using System;
using System.Collections.Generic;

namespace MedicInPoint.Models;

public partial class Request
{
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
    public int RequestStateId { get; set; }

    /// <summary>
    /// Время создания запроса на запись
    /// </summary>
    public DateTime RequestSended { get; set; }

    /// <summary>
    /// Время изменения запроса на запись
    /// </summary>
    public DateTime? RequestChanged { get; set; }

    public User Doctor { get; set; } = null!;

    public Patient Patient { get; set; } = null!;

    public PatientAnalysisAddress PatientAnalysisAddress { get; set; } = null!;

    public ICollection<RequestAnalysis> RequestAnalyses { get; set; } = [];

    public RequestState RequestState { get; set; } = null!;
}
