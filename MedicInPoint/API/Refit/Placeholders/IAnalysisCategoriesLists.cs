using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IAnalysisCategoriesLists
{
	[Post("/analysis_categories")]
	Task<IApiResponse> UpdateCategories4Analysis([Query]int analysisId, [Body]List<int> categoryIds);
}