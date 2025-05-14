using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IPatientAnalysisAddress
{
    [Post("/patient_analysis_address")]
    Task<IApiResponse<PatientAnalysisAddress>> Post([Body] PatientAnalysisAddress address);
}