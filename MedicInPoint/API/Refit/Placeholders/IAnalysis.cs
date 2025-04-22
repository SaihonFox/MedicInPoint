using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IAnalysis
{
	[Get("/analyses")]
	Task<IApiResponse<IList<Analysis>>> GetAnalyses();

	[Get("/analyses/{id}")]
	Task<IApiResponse<Analysis?>> GetAnalysis(int id);

	[Post("/analyses")]
	Task<IApiResponse<Analysis>> AddAnalysis(Analysis analysis, CancellationToken? token = null);

	[Put("/analyses")]
	Task<IApiResponse<Analysis>> UpdateAnalysis(Analysis analysis, CancellationToken? token = null);
	
	[Delete("/analyses/{id}")]
	Task<IApiResponse<Analysis>> DeleteAnalysis(int id);
}