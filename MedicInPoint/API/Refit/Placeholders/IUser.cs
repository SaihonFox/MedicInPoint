using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IUser
{
	[Get("/users/login")]
	Task<IApiResponse<User>> Login([Query]string login, [Query]string password, CancellationToken? token = null);
}