using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.UserControls.Items;

public partial class UserItem_UserControl_ViewModel : ViewModelBase
{
	[ObservableProperty]
	private User? _user;

	[ObservableProperty]
	private bool _isSelected = false;
}