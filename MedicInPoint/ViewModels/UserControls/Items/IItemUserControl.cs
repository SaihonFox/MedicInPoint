using CommunityToolkit.Mvvm.ComponentModel;

namespace MedicInPoint.ViewModels.UserControls.Items;

public abstract partial class IItemUserControl : ObservableObject
{
	[ObservableProperty]
	private bool _isSelected = false;
}