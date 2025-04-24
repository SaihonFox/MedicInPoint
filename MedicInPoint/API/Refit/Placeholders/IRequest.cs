using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IRequest
{
	[Get("/requests")]
	Task<IApiResponse<List<Request>>> GetRequests();

	[Get("/requests/{id}")]
	Task<IApiResponse<Request>> GetRequest(int id);

	[Post("/requests")]
	Task<IApiResponse<Request>> PostRequest([Body]Request analysis, [Query]bool ignoreId = true);

	[Put("/requests")]
	Task<IApiResponse<Request>> PutRequest([Body]Request analysis);
}