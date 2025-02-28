using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

namespace MedicInPoint.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
	[ObservableProperty]
	private UserControl _currentPage = default!;
}