using System.Collections.ObjectModel;

using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Threading;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Models;
using MedicInPoint.Services;
using MedicInPoint.ViewModels.UserControls.Drawers;

using Microsoft.Extensions.DependencyInjection;

namespace MedicInPoint.Views.UserControls.Drawers;

public partial class AnalysisDrawerView : UserControl
{
	public static ObservableCollection<AnalysisCategory> AllCategories = [];

	public AnalysisDrawerViewModel ViewModel => (DataContext as AnalysisDrawerViewModel)!;

	public readonly INotificationService notification = App.services.GetRequiredService<INotificationService>();

	public Action<AnalysisDrawerView> OnAdding = null!;
	public Action<AnalysisDrawerView> OnEditing = null!;
	public Action<AnalysisDrawerView> OnDeleting = null!;

	public Action<AnalysisDrawerView?>? ActionOnSelect = null;
	public Action<AnalysisDrawerView>? ActionOnDelete = null;
	public Action<AnalysisDrawerView>? ActionOnPreDelete = null;

	public bool IsAdding { get; set; } = false;
	public bool IsEditing { get; set; } = false;
	public bool IsDeleting { get; set; } = false;

	public List<AnalysisCategory> CategoriesInAnalysis = [];

	public AnalysisDrawerView()
	{
		InitializeComponent();

		FillCategories();

		delete_btn.Click += Delete_btn_Click;
		apply_btn.Click += Apply_btn_Click;
		reject_btn.Click += Reject_btn_Click;

		edit_btn.Click += Edit_btn_Click;

		if(!Design.IsDesignMode)
		Loaded += (_, _) =>
		{
			CategoriesInAnalysis.AddRange(ViewModel.AnalysisCategories);
		};
	}

	async void FillCategories()
	{
		using var response = await APIService.For<IAnalysisCategory>().GetAnalysisCategories().ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;

		Dispatcher.UIThread.Invoke(() =>
		{
			foreach (var category in response.Content)
			{
				AllCategories.Add(category);
				all_categories_cb.Items.Add(category);
			}
			all_categories_cb.SelectedIndex = 0;
		});
	}

	void Add_analysis_Click(object? sender, RoutedEventArgs e)
	{
		OnAdding?.Invoke(this);
	}

	void Edit_btn_Click(object? sender, RoutedEventArgs e)
	{
		OnEditing?.Invoke(this);
	}

	void Delete_btn_Click(object? sender, RoutedEventArgs e)
	{
		IsDeleting = true;

		edit_btn.IsVisible = false;
		delete_btn.IsVisible = false;
		apply_btn.IsVisible = true;
		reject_btn.IsVisible = true;
	}

	void Reject_btn_Click(object? sender, RoutedEventArgs e)
	{
		IsDeleting = false;

		edit_btn.IsVisible = true;
		delete_btn.IsVisible = true;
		apply_btn.IsVisible = false;
		reject_btn.IsVisible = false;
	}

	async void Apply_btn_Click(object? sender, RoutedEventArgs e)
	{
		using var response = await APIService.For<IAnalysis>().DeleteAnalysis(ViewModel!.Analysis!.Id);
		if (!response.IsSuccessful)
			notification.Show("Ошибка!", $"Не удалось удалить: {response.StatusCode}", NotificationType.Error);
		else
		{
			notification.Show("Успех!", "Успешно удалено", NotificationType.Success);

			IsDeleting = false;

			edit_btn.IsVisible = true;
			delete_btn.IsVisible = true;
			apply_btn.IsVisible = false;
			reject_btn.IsVisible = false;
		}
	}
}