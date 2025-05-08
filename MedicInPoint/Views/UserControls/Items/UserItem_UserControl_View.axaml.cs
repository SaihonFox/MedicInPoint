using Avalonia.Controls;

using MedicInPoint.ViewModels.UserControls.Items;

namespace MedicInPoint;

public partial class UserItem_UserControl_View : UserControl
{
	public UserItem_UserControl_ViewModel ViewModel => DataContext as UserItem_UserControl_ViewModel;

	public Action<UserItem_UserControl_View, bool> OnToggle { get; set; } = null!;

	public UserItem_UserControl_View()
	{
		InitializeComponent();

		select_tb.IsCheckedChanged += (_, _) =>
		{
			OnToggle?.Invoke(this, select_tb.IsChecked!.Value);
		};
	}
}