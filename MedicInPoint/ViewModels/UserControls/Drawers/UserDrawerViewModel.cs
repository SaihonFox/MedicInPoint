using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;
using MedicInPoint.Services;

using Microsoft.Extensions.DependencyInjection;

namespace MedicInPoint.ViewModels.UserControls.Drawers;

public partial class UserDrawerViewModel : ViewModelBase
{
	[ObservableProperty]
	private User? _user;

	public bool IsFireReturnVisible => App.services.GetRequiredService<IAppStateService>().CurrentUser!.Id != User?.Id;
}