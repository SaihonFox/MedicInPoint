using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IMessage
{
	[Get("/messages")]
	Task<IList<Message>> GetMessages();

	[Get("/messages/{id}")]
	Task<Message?> GetMessage(int id);

	[Post("/messages")]
	Task AddMessage(Message analysis);
}