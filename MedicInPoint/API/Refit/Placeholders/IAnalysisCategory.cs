using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IAnalysisCategory
{
	[Get("/analysis_categories")]
	Task<IList<AnalysisCategory>> GetAnalysisCategories();

	[Get("/analysis_categories/{id}")]
	Task<AnalysisCategory?> GetAnalysisCategory(int id);

	[Post("/analysis_categories")]
	Task AddAnalysisCategory(AnalysisCategory analysis);

	[Put("/analysis_categories")]
	Task UpdateAnalysisCategory(AnalysisCategory analysis);

	[Delete("/analysis_categories/{id}")]
	Task DeleteAnalysisCategory(int id);
}