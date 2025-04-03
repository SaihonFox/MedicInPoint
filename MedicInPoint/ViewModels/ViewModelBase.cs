using Avalonia.SimpleRouter.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;

namespace MedicInPoint.ViewModels;

public abstract partial class ViewModelBase : ObservableObject, ISimpleRoute<ViewModelBase>
{
	[ObservableProperty]
	private ViewModelBase? _content;

	[ObservableProperty]
	private string _title;

	protected async void Log(string message, string tag) =>
		await File.AppendAllTextAsync(Environment.CurrentDirectory + "/log.txt", $"[{DateTime.Now:H:m:s.fffffff}]#{tag}: message\n");
}