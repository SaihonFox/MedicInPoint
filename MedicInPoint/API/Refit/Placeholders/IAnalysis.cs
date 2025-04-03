using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IAnalysis
{
	[Get("/analyses")]
	Task<IList<Analysis>> GetAnalyses();

	[Get("/analyses/{id}")]
	Task<Analysis?> GetAnalysis(int id);

	[Post("/analyses")]
	Task AddAnalysis(Analysis analysis);

	[Put("/analyses")]
	Task UpdateAnalysis(Analysis analysis);
	
	[Delete("/analyses/{id}")]
	Task DeleteAnalysis(int id);
}