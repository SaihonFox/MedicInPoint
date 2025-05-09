using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;
using MedicInPoint.Services;

using Microsoft.Extensions.DependencyInjection;

namespace MedicInPoint.ViewModels.UserControls.Items;

public partial class UserItem_UserControl_ViewModel : ViewModelBase
{
	[ObservableProperty]
	private User? _user;

	[ObservableProperty]
	private bool _isSelected = false;

	partial void OnUserChanging(User? value)
	{
		App.services.GetRequiredService<INotificationService>().Show("1", $"{User?.FullName}: {value?.IsBlocked} - {User?.IsBlocked}");
	}

	partial void OnUserChanged(User? value)
	{
		App.services.GetRequiredService<INotificationService>().Show("2", $"{User?.FullName}: {value?.IsBlocked} - {User?.IsBlocked}");
	}
}