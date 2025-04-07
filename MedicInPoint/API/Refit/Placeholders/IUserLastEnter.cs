using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IUserLastEnter
{
	[Post("/user_last_enters")]
	Task<IApiResponse<UserLastEnter>> SetLastEnter([Query]int userId, [Query]DateTime enterDateTime);

	[Get("/user_last_enters/{id}")]
	Task<IApiResponse<UserLastEnter>> GetUserLastEnter(int id);
}