using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IAnalysisCategory
{
	[Get("/analysis_categories")]
	Task<IApiResponse<IList<AnalysisCategory>>> GetAnalysisCategories();

	[Get("/analysis_categories/{id}")]
	Task<IApiResponse<AnalysisCategory>> GetAnalysisCategory(int id);

	[Post("/analysis_categories")]
	Task<IApiResponse<AnalysisCategory>> AddAnalysisCategory(AnalysisCategory analysis, CancellationToken? token = null);

	[Put("/analysis_categories")]
	Task<IApiResponse<AnalysisCategory>> UpdateAnalysisCategory(AnalysisCategory analysis, CancellationToken? token = null);

	[Delete("/analysis_categories/{id}")]
	Task<IApiResponse<AnalysisCategory>> DeleteAnalysisCategory(int id);
}