using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IMessagesMessages
{
	[Get("/messages_messages")]
	Task<IList<Message>> GetMessagesMessages();

	[Get("/messages_messages/{id}")]
	Task<Message?> GetMessage(int id);

	[Post("/messages_messages")]
	Task AddMessage(Message analysis);
}