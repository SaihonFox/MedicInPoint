using MedicInPoint.Models;

namespace MedicInPoint.Services;

public interface IAppStateService : IDisposable
{
	public User? CurrentUser { get; set; }
}