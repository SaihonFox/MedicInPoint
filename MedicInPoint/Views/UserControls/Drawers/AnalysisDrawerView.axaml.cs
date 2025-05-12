using System.Collections.ObjectModel;
using System.Xml.Linq;

using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Threading;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Extensions;
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

	public Action<AnalysisDrawerView?>? ActionOnSelect = null;
	public Action<AnalysisDrawerView>? ActionOnDelete = null;
	public Action<AnalysisDrawerView>? ActionOnPreDelete = null;

	public bool IsEditing { get; set; } = false;
	public bool IsDeleting { get; set; } = false;
	private Analysis? initData = null;

	public List<AnalysisCategory> CategoriesInAnalysis = [];

	public AnalysisDrawerView()
	{
		InitializeComponent();

		FillCategories();

		edit_btn.Click += first_btn_Click;
		apply_btn.Click += first_btn_Click;
		delete_btn.Click += second_btn_Click;
		reject_btn.Click += second_btn_Click;

		add_order.Click += add_order_Click;
		add_category.Click += add_category_Click;

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

	async void first_btn_Click(object? sender, RoutedEventArgs e)
	{
		await Dispatcher.UIThread.Invoke(async() =>
		{
			if (!IsEditing)
			{
				if (IsDeleting)
				{
					try
					{
						using var response = await APIService.For<IAnalysis>().DeleteAnalysis(ViewModel!.Analysis!.Id);
						if (!response.IsSuccessful)
							notification.Show("Ошибка!", $"Не удалось удалить: {response.StatusCode}", NotificationType.Error);
						else
						{
							notification.Show("Успех!", "Успешно удалено", NotificationType.Success);
							IsDeleting = false;
							(edit_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/edit_category.svg";
							(delete_btn.Content as Avalonia.Svg.Skia.Svg)!.Path = "/Assets/SVGs/buttons/delete_category.svg";

							ActionOnDelete?.Invoke(this);

							edit_btn.IsVisible = true;
							delete_btn.IsVisible = true;
							apply_btn.IsVisible = false;
							reject_btn.IsVisible = false;
							add_order.IsVisible = true;
						}
					}
					catch (Exception ex)
					{
						notification.Show("Ошибка", ex.StackTrace!, NotificationType.Error);
					}
				}
				else
				{
					ActionOnSelect?.Invoke(this);

					initData = new() {
						Name = ViewModel.Analysis!.Name,
						Biomaterial = ViewModel.Analysis!.Biomaterial,
						ResultsAfter = ViewModel.Analysis!.ResultsAfter,
						Preparation = ViewModel.Analysis!.Preparation,
						Description = ViewModel.Analysis!.Description,
						Price = ViewModel.Analysis!.Price,
					};

					IsEditing = true;
					ViewModel.IsEditable = true;

					edit_btn.IsVisible = false;
					delete_btn.IsVisible = false;
					apply_btn.IsVisible = true;
					reject_btn.IsVisible = true;
					add_order.IsVisible = false;
				}
			}
			else
			{
				if (new[] { name.Text, results_after.Value, price.Value, biomaterial.Value }.Any(x => x.IsNullOrWhiteSpace()))
				{
					notification.Show("Внимание", "Пустые поля", NotificationType.Warning);
					return;
				}

				ActionOnSelect?.Invoke(null);

				try
				{
					using var response = await APIService.For<IAnalysis>().UpdateAnalysis(ViewModel.Analysis!);
					if (!response.IsSuccessful)
						notification.Show("Ошибка!", $"Не удалось удалить: {response.StatusCode}", NotificationType.Error);
					else
					{
						IsEditing = false;
						ViewModel.IsEditable = false;

						edit_btn.IsVisible = true;
						delete_btn.IsVisible = true;
						apply_btn.IsVisible = false;
						reject_btn.IsVisible = false;
						add_order.IsVisible = true;
					}
				}
				catch (Exception ex)
				{
					notification.Show("Ошибка", ex.StackTrace!, NotificationType.Error);
				}
			}
		});
	}

	async void second_btn_Click(object? sender, RoutedEventArgs e)
	{
		await Dispatcher.UIThread.Invoke(async () =>
		{
			if (IsEditing)
			{
				IsEditing = false;
				ViewModel.IsEditable = false;

				edit_btn.IsVisible = true;
				delete_btn.IsVisible = true;
				apply_btn.IsVisible = false;
				reject_btn.IsVisible = false;
				add_order.IsVisible = true;

				ViewModel.Analysis = initData;
			}
			else if (!IsDeleting)
			{
				IsDeleting = true;

				edit_btn.IsVisible = false;
				delete_btn.IsVisible = false;
				apply_btn.IsVisible = true;
				reject_btn.IsVisible = true;
				add_order.IsVisible = false;
			}
			else
			{
				IsDeleting = false;

				edit_btn.IsVisible = true;
				delete_btn.IsVisible = true;
				apply_btn.IsVisible = false;
				reject_btn.IsVisible = false;
				add_order.IsVisible = true;
			}
		});
	}

	void add_order_Click(object? sender, RoutedEventArgs e)
	{
		Dispatcher.UIThread.Invoke(() =>
		{
			ViewModel.Analysis = new Analysis { Id = 0 };
		});
	}

	void add_category_Click(object? sender, RoutedEventArgs e)
	{
		initData = new();
		ViewModel.Analysis = new();
	}
}