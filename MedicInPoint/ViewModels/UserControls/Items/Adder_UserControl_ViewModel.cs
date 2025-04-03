using CommunityToolkit.Mvvm.Input;

namespace MedicInPoint.ViewModels.UserControls.Items;

public partial class Adder_UserControl_ViewModel() : ViewModelBase
{
	public required Action OnClickAction { get; set; } = () => {};

	[RelayCommand]
	public void Press()
	{
		OnClickAction.Invoke();
	}
}