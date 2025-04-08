using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IRequest
{
	[Get("/requests")]
	Task<IApiResponse<List<Request>>> GetRequests();

	[Get("/requests/{id}")]
	Task<IApiResponse<Request>> GetRequest(int id);

	[Put("/requests")]
	Task<IApiResponse<Request>> UpdateRequest(Request analysis);
}