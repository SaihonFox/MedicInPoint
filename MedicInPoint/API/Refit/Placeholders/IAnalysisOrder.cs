using MedicInPoint.Models;

using Refit;

namespace Medic.API.Refit.Placeholders;

public interface IAnalysisOrder
{
	[Post("/analysis_orders/new_order")]
	Task<IApiResponse<dynamic>> NewOrder([Body](AnalysisOrder analysisOrder, PatientAnalysisAddress address, List<Analysis> analyses) body);
}