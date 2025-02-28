using CommunityToolkit.Mvvm.ComponentModel;

using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.UserControls.Drawers;

public partial class UserDrawerViewModel : ViewModelBase
{
	[ObservableProperty]
	private User? _user;
}