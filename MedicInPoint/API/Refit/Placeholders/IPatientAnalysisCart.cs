using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IPatientAnalysisCart
{
    [Post("/patient_analysis_cart")]
    Task<IApiResponse<PatientAnalysisCart>> Post([Body] PatientAnalysisCart patientAnalysisCart);
}