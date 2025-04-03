using MedicInPoint.Models;

using Refit;

namespace MedicInPoint.API.Refit.Placeholders;

public interface IUsers
{
	[Get("/users/login")]
	Task<User?> Login([Query]string login, [Query]string password);
}