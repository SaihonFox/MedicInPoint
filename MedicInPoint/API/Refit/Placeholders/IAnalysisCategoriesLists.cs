using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IAnalysisCategoriesLists
{
	[Post("/analysis_categories_lists")]
	Task<IApiResponse> UpdateCategories4Analysis([Query] int analysisId, [Body] int[] categoryIds);
}