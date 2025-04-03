using Avalonia.SimpleRouter.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;

namespace MedicInPoint.ViewModels;

public abstract partial class ViewModelBase : ObservableObject, ISimpleRoute<ViewModelBase>
{
	[ObservableProperty]
	private ViewModelBase? _content;

	[ObservableProperty]
	private string _title;
}