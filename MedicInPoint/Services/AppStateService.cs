using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;

namespace MedicInPoint.Services;

public partial class AppStateService : ObservableObject, IAppStateService
{
	[ObservableProperty]
	public User? _currentUser = null;
}