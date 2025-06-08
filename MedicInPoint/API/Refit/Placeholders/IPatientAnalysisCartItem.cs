using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IPatientAnalysisCartItem
{
	[Post("/patient_analysis_cart_item")]
	Task<IApiResponse<PatientAnalysisCartItem>> Post([Body] PatientAnalysisCartItem item);

	[Put("/patient_analysis_cart_item")]
	Task<IApiResponse<PatientAnalysisCartItem>> Update([Body] PatientAnalysisCartItem item);
}