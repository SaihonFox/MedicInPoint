using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IMessagesMessages
{
	[Get("/messages_messages")]
	Task<IApiResponse<IList<MessagesMessage>>> GetMessagesMessages();

	[Get("/messages_messages/{id}")]
	Task<IApiResponse<MessagesMessage>> GetMessagesMessage(int id);

	[Post("/messages_messages")]
	Task<IApiResponse<MessagesMessage>> AddMessage(MessagesMessage analysis);
}