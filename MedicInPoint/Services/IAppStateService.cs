using MedicInPoint.Models;

namespace MedicInPoint.Services;

public interface IAppStateService
{
	public User? CurrentUser { get; set; }
}