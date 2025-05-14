using MedicInPoint.Models;

using Refit;

namespace Medic.API.Refit.Placeholders;

public interface IAnalysisOrder
{
	[Get("/analysis_orders")]
	Task<IApiResponse<List<AnalysisOrder>>> GetAnalysisOrders();

	[Get("/analysis_orders/user")]
	Task<IApiResponse<List<AnalysisOrder>>> GetAnalysisOrders4User([Query] int id);

	[Post("/analysis_orders")]
	Task<IApiResponse<User>> Post([Body] AnalysisOrder analysisOrder);

	[Post("/analysis_orders/new_order")]
	Task<IApiResponse<User>> NewOrder([Body](AnalysisOrder analysisOrder, PatientAnalysisAddress address, List<Analysis> analyses) body);
}