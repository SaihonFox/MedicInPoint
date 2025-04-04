using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IMessage
{
	[Get("/messages")]
	Task<IApiResponse<IList<Message>>> GetMessages();

	[Get("/messages/{id}")]
	Task<IApiResponse<Message?>> GetMessage(int id);

	[Post("/messages")]
	Task<IApiResponse<Message>> AddMessage(Message analysis);
}