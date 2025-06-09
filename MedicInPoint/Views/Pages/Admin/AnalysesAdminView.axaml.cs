using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.API.SignalR;
using MedicInPoint.Extensions;
using MedicInPoint.Models;
using MedicInPoint.Services;
using MedicInPoint.ViewModels.Pages.Admin;
using MedicInPoint.ViewModels.UserControls.Drawers;
using MedicInPoint.ViewModels.UserControls.Items;
using MedicInPoint.Views.UserControls.Drawers;
using MedicInPoint.Views.UserControls.Items;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MIP.LocalDB;

using System.Collections.ObjectModel;

namespace MedicInPoint.Views.Pages.Admin;

public partial class AnalysesAdminView : UserControl
{
	private ObservableCollection<Analysis> AllAnalyses = [];
	private ObservableCollection<AnalysisCategory> AllCategories = [];
	private ObservableCollection<AnalysisItem_UserControl_View> AllAnalysesView = [];
	public AnalysesAdminViewModel ViewModel => (DataContext as AnalysesAdminViewModel)!;

	public enum EMode
	{
		None,
		Adding,
		Editing
	}
	public EMode CurrentMode = EMode.None;

	public AnalysesAdminView()
	{
		InitializeComponent();

		var collection = new BehaviorCollection
		{
			new TestAutoCompleteBehavior.Behaviors.AutoCompleteZeroMinimumPrefixLengthDropdownBehavior()
		};
		Interaction.SetBehaviors(acb, collection);

		drawer.DataContext = new AnalysisDrawerViewModel();

		using var context = new LocalDbContext();
		var names = context.AnalysesAdminSearches.ToList().Select(aas => aas.name);
		acb.ItemsSource = names.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();

		acb.KeyDown += acb_KeyDown;
		acb.TextChanged += acb_TextChanged;

		add_analysis.Click += Add_analysis_Click;
		apply_btn.Click += Apply_btn_Click;
		reject_btn.Click += Reject_btn_Click;

		add_category_to_analysis.Click += Add_category_to_analysis_Click;

		categories_list.SelectionChanged += async(_, _) => await FillWithSearch();

		if (!Design.IsDesignMode)
		{
			Loaded += async(s, e) =>
			{
				await Task.WhenAll(FillCategories(), FillAnalyses());
			};
		}

		App.services.GetRequiredService<MedicSignalRConnections>().AnalysisCategoryConnection.On<Analysis>("AnalysisAdded", async analysis => {
			AllAnalyses.Add(analysis);
			await FillWithSearch();
		});
		App.services.GetRequiredService<MedicSignalRConnections>().AnalysisCategoryConnection.On<Analysis>("AnalysisUpdated", analysis => {
			Dispatcher.UIThread.Invoke(() =>
			{
				var user1 = AllAnalyses.FirstOrDefault(x => x.Id == analysis.Id);
				if (user1 != null)
					user1 = analysis;

				var userView = AllAnalysesView.FirstOrDefault(x => x.ViewModel.Analysis?.Id == analysis.Id);
				if (userView != null)
					userView.ViewModel.Analysis = analysis;
				if (selectedAnalysis != null)
					selectedAnalysis.ViewModel.Analysis = analysis;
				if (ViewModel.SelectedAnalysis != null)
					ViewModel.SelectedAnalysis = analysis;
				if (drawer.ViewModel.Analysis != null)
					drawer.ViewModel.Analysis = analysis;
			});
		});
		App.services.GetRequiredService<MedicSignalRConnections>().AnalysisCategoryConnection.On<Analysis>("AnalysisDeleted", async analysis =>
		{
			var c = analyses_list.Items.ToList().First(c => (c as Analysis)!.Id == analysis.Id) as Analysis;
			AllAnalyses.Remove(c!);
			await FillWithSearch();
		});

		drawer.OnEditing += Drawer_OnEditing;
		drawer.OnDeleting += Drawer_OnDeleting;
	}

	private async void acb_TextChanged(object? sender, TextChangedEventArgs e) => await FillWithSearch();

	private AnalysisItem_UserControl_View? selectedAnalysis = null;

	void OnSelectAnalysis(AnalysisItem_UserControl_View view, bool isSelected)
	{
		Dispatcher.UIThread.Invoke(() =>
		{
			if (selectedAnalysis != null)
			{
				selectedAnalysis.ViewModel.IsSelected = false;
				selectedAnalysis = null;
			}
			selectedAnalysis = isSelected ? view : null;
			ViewModel.SelectedAnalysis = selectedAnalysis?.ViewModel?.Analysis;
			drawer.ViewModel.Analysis = selectedAnalysis?.ViewModel?.Analysis;
		});
	}

	async void acb_KeyDown(object? source, KeyEventArgs e)
	{
		using var context = new LocalDbContext();
		if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(acb.SearchText))
		{
			var list = await context.AnalysesAdminSearches.ToListAsync();
			if (list.FirstOrDefault(a => a.name == acb.SearchText) != null || acb.SearchText.IsNullOrWhiteSpace())
				return;

			context.AnalysesAdminSearches.Add(new() { name = acb.SearchText });
			await context.SaveChangesAsync();

			var enumerable = acb.ItemsSource!.Cast<string>().ToList();
			enumerable.Add(acb.SearchText);
			enumerable = [.. enumerable.Distinct()];
			acb.ItemsSource = enumerable;
		}
		await context.DisposeAsync();
	}

	async Task FillCategories()
	{
		App.services.GetRequiredService<INotificationService>().Show("Уведомление", "Загрузка списка категорий").Expiration = TimeSpan.FromSeconds(1.2);
		using var response = await APIService.For<IAnalysisCategory>().GetAnalysisCategories().ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;

		AllCategories = [.. response.Content!];
		Dispatcher.UIThread.Invoke(() => categories_list_in_analysis.ItemsSource = new List<AnalysisCategory>(AllCategories));

		var allCategory = new AnalysisCategory { Id = 0, Name = "Все" };
		AllCategories.Insert(0, allCategory);
		foreach (var category in AllCategories)
		{
			await Dispatcher.UIThread.InvokeAsync(() =>
			{
				categories_list.Items.Add(category);
			});
		}

		Dispatcher.UIThread.Invoke(() =>
		{
			categories_list.SelectedItem = allCategory;
			categories_list.UpdateLayout();
		});

		foreach(var category in AllCategories)
			AnalysisDrawerView.AllCategories.Add(category);
	}

	async Task FillAnalyses()
	{
		App.services.GetRequiredService<INotificationService>().Show("Уведомление", "Загрузка списка анализов").Expiration = TimeSpan.FromSeconds(1.2);
		using var response = await APIService.For<IAnalysis>().GetAnalyses().ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;

		AllAnalyses = [.. response.Content!];
		foreach (var analysis in AllAnalyses)
		{
			await Dispatcher.UIThread.InvokeAsync(() =>
			{
				var view = new AnalysisItem_UserControl_View
				{
					DataContext = new AnalysisItem_UserControl_ViewModel
					{
						Analysis = analysis,
						IsSelected = analysis.Id == selectedAnalysis?.ViewModel.Analysis?.Id
					},
					OnToggle = OnSelectAnalysis
				};
				AllAnalysesView.Add(view);
				analyses_list.Items.Add(view);
			});
		}

		await FillWithSearch();
	}

	async Task FillWithSearch()
	{
		Dispatcher.UIThread.Invoke(analyses_list.Items.Clear);
		var analyses = Dispatcher.UIThread.Invoke(() => {
			var list = AllAnalysesView.Where(x =>
				x.ViewModel.Analysis!.Name.Contains(Dispatcher.UIThread.Invoke(() => acb.Text!), StringComparison.CurrentCultureIgnoreCase))
			.ToList();

			if (categories_list.SelectedIndex == 0)
				return list;

			list = [.. list.Where(x =>
				x.ViewModel.Analysis!.AnalysisCategoriesLists.Any(x =>
					x.AnalysisCategory.Name.Contains((categories_list.SelectedItem as AnalysisCategory)?.Name ?? "", StringComparison.CurrentCultureIgnoreCase)
				)
			)];

			return list;
		});
		Dispatcher.UIThread.Invoke(() => {
			centerText.IsVisible = analyses.Count == 0;
			centerText.Text = "Пустой список";
		});

		Dispatcher.UIThread.Invoke(() =>
		{
			if (analyses.Find(x => x.ViewModel.Analysis!.Id == selectedAnalysis?.ViewModel.Analysis?.Id) == null && selectedAnalysis != null)
			{
				selectedAnalysis.ViewModel.IsSelected = false;
				selectedAnalysis = null;
			}
		});

		foreach (var analysis in analyses)
		{
			Dispatcher.UIThread.Invoke(() => {
				analyses_list.Items.Add(analysis);
				centerText.IsVisible = false;
			});
		}

		Dispatcher.UIThread.Invoke(() => {
			centerText.IsVisible = analyses.Count == 0;
			centerText.Text = "Пустой список";
		});
		await Dispatcher.UIThread.InvokeAsync(() =>
		{
			ViewModel.SelectedAnalysis = selectedAnalysis?.ViewModel?.Analysis;
			drawer.ViewModel.Analysis = ViewModel.SelectedAnalysis;
		});
	}

	void ClearDialog()
	{
		analysis_name.Text = string.Empty;
		analysis_description.Text = string.Empty;
		analysis_biomaterial.Text = string.Empty;
		analysis_price.Text = string.Empty;
		analysis_preparation.Text = string.Empty;
		analysis_results_after.Text = string.Empty;

		categories_in_analysis.Items.Clear();
	}

	void Reject_btn_Click(object? sender, RoutedEventArgs e)
	{
		ClearDialog();
		CurrentMode = EMode.None;
		dialog.Opacity = 0;
		(dialog.Transitions![0] as DoubleTransition)!.PropertyChanged += (_, e) =>
		{
			App.services.GetRequiredService<INotificationService>().Show("%", e.NewValue!.ToString()!);
			dialog.IsVisible = false;
		};
		dialog.IsVisible = false;
	}

	async void Apply_btn_Click(object? sender, RoutedEventArgs e)
	{
		var notification = App.services.GetRequiredService<INotificationService>();

		if (analysis_name.Text.IsNullOrWhiteSpace() || analysis_price.Text.IsNullOrWhiteSpace() || analysis_results_after.Text.IsNullOrWhiteSpace() || analysis_biomaterial.Text.IsNullOrWhiteSpace())
		{
			notification.Show("Ошибка!", "Не все поля заполнены", NotificationType.Error);
			return;
		}

		if (CurrentMode == EMode.Adding)
		{
			var new_analysis = new Analysis
			{
				Name = analysis_name.Text!.Trim(),
				Price = decimal.Parse(analysis_price.Text!.Trim()),
				Biomaterial = analysis_biomaterial.Text!.Trim(),
				Preparation = analysis_preparation.Text!.Trim(),
				Description = analysis_description.Text!.Trim(),
				ResultsAfter = analysis_results_after.Text!.Trim()
			};
			using var response = await APIService.For<IAnalysis>().AddAnalysis(new_analysis);
			if(!response.IsSuccessful)
			{
				notification.Show("Ошибка!", $"Message: {response.Error.Message}", NotificationType.Error);
				return;
			}
			using var responseCategory = await APIService.For<IAnalysisCategoriesLists>().UpdateCategories4Analysis(
				ViewModel.SelectedAnalysis!.Id,
				[.. categories_in_analysis.Items.Cast<AnalysisCategory>().Select(x => x.Id)]
			);
			if (!response.IsSuccessful)
			{
				notification.Show("Ошибка категорий!", $"Message: {response.Error.Message}", NotificationType.Error);
				return;
			}

			Dispatcher.UIThread.Invoke(() =>
			{
				AllAnalyses.Add(response.Content);
				AllAnalysesView.Add(new AnalysisItem_UserControl_View
				{
					DataContext = new AnalysisItem_UserControl_ViewModel
					{
						Analysis = response.Content,
						IsSelected = response.Content.Id == selectedAnalysis?.ViewModel.Analysis?.Id
					},
					OnToggle = OnSelectAnalysis
				});
			});
			notification.Show("Успех!", $"Успешно добавлено!", NotificationType.Success);
		}
		if(CurrentMode == EMode.Editing)
		{
			ViewModel.SelectedAnalysis!.Name = analysis_name.Text!.Trim();
			ViewModel.SelectedAnalysis.Price = decimal.Parse(analysis_price.Text!.Trim());
			ViewModel.SelectedAnalysis.Biomaterial = analysis_biomaterial.Text!.Trim();
			ViewModel.SelectedAnalysis.ResultsAfter = analysis_results_after.Text!.Trim();
			ViewModel.SelectedAnalysis.Description = analysis_description.Text.IsNullOrWhiteSpace() ? null : analysis_description.Text!.Trim();
			ViewModel.SelectedAnalysis.Preparation = analysis_preparation.Text.IsNullOrWhiteSpace() ? null : analysis_preparation.Text!.Trim();
			using var response = await APIService.For<IAnalysis>().UpdateAnalysis(ViewModel.SelectedAnalysis!);
			if (!response.IsSuccessful)
			{
				notification.Show("Ошибка!", $"Message: {response.Error.Message}", NotificationType.Error);
				return;
			}
			using var responseCategory = await APIService.For<IAnalysisCategoriesLists>().UpdateCategories4Analysis(
				ViewModel.SelectedAnalysis!.Id,
				[.. categories_in_analysis.Items.Cast<CategoryInAnalysisUserControl>().Select(x => x.Category.Id)]
			);
			if (!response.IsSuccessful)
			{
				notification.Show("Ошибка категорий!", $"Message: {response.Error.Message}", NotificationType.Error);
				return;
			}

			Dispatcher.UIThread.Invoke(() =>
			{
				var analysis = AllAnalyses.First(x => x.Id == ViewModel.SelectedAnalysis.Id);
				analysis.Name = analysis_name.Text!.Trim();
				analysis.Price = decimal.Parse(analysis_price.Text!.Trim());
				analysis.Biomaterial = analysis_biomaterial.Text!.Trim();
				analysis.ResultsAfter = analysis_results_after.Text!.Trim();
				analysis.Description = analysis_description.Text.IsNullOrWhiteSpace() ? null : analysis_description.Text!.Trim();
				analysis.Preparation = analysis_preparation.Text.IsNullOrWhiteSpace() ? null : analysis_preparation.Text!.Trim();

				var analysisView = AllAnalysesView.First(x => x.ViewModel.Analysis!.Id == ViewModel.SelectedAnalysis.Id);
				analysisView.ViewModel.Analysis!.Name = analysis_name.Text!.Trim();
				analysisView.ViewModel.Analysis.Price = decimal.Parse(analysis_price.Text!.Trim());
				analysisView.ViewModel.Analysis.Biomaterial = analysis_biomaterial.Text!.Trim();
				analysisView.ViewModel.Analysis.ResultsAfter = analysis_results_after.Text!.Trim();
				analysisView.ViewModel.Analysis.Description = analysis_description.Text.IsNullOrWhiteSpace() ? null : analysis_description.Text!.Trim();
				analysisView.ViewModel.Analysis.Preparation = analysis_preparation.Text.IsNullOrWhiteSpace() ? null : analysis_preparation.Text!.Trim();
			});
			notification.Show("Успех!", $"Успешно изменено!", NotificationType.Success);
		}

		ClearDialog();
		categories_in_analysis.Items.Clear();
		dialog.Opacity = 0;
		(dialog.Transitions![0] as DoubleTransition)!.PropertyChanged += (_, e) =>
		{
			App.services.GetRequiredService<INotificationService>().Show("%", e.NewValue!.ToString()!);
			dialog.IsVisible = false;
		};
		dialog.IsVisible = false;
		await FillWithSearch();
	}

	void Add_analysis_Click(object? sender, RoutedEventArgs e)
	{
		CurrentMode = EMode.Adding;

		dialog.IsVisible = true;
		dialog.Opacity = 1;
	}

	void Drawer_OnEditing(AnalysisDrawerView obj)
	{
		CurrentMode = EMode.Editing;

		dialog.IsVisible = true;
		dialog.Opacity = 1;

		analysis_name.Text = ViewModel.SelectedAnalysis!.Name;
		analysis_description.Text = ViewModel.SelectedAnalysis.Description;
		analysis_biomaterial.Text = ViewModel.SelectedAnalysis.Biomaterial;
		analysis_preparation.Text = ViewModel.SelectedAnalysis.Preparation;
		analysis_price.Text = ViewModel.SelectedAnalysis.Price.ToString();
		analysis_results_after.Text = ViewModel.SelectedAnalysis.ResultsAfter;

		foreach(var category in ViewModel.SelectedAnalysis.AnalysisCategoriesLists)
			categories_in_analysis.Items.Add(new CategoryInAnalysisUserControl { Category = category.AnalysisCategory, OnDelete = DeleteCategoryFromAnalysis });
	}

	void Drawer_OnDeleting(AnalysisDrawerView view)
	{
		Dispatcher.UIThread.Invoke(async() =>
		{
			var analysis = view.ViewModel.Analysis!;
			var ucView = analyses_list.Items.Cast<AnalysisItem_UserControl_View>().ToList().First(c => c.ViewModel.Analysis!.Id == analysis.Id);
			AllAnalyses.Remove(ucView.ViewModel.Analysis!);
			analyses_list.Items.Remove(ucView);

			var analysisView = AllAnalysesView.First(x => x.ViewModel.Analysis!.Id == analysis.Id);
			AllAnalysesView.Remove(analysisView);
			await FillWithSearch();
		});
	}

	void Add_category_to_analysis_Click(object? sender, RoutedEventArgs e)
	{
		var category = (categories_list_in_analysis.SelectedItem as AnalysisCategory)!;
		if (categories_in_analysis.Items.Cast<CategoryInAnalysisUserControl>().Any(x => x.Category.Id == category.Id))
		{
			App.services.GetRequiredService<INotificationService>().Show("Уведомление", "Уже имеется такая категория");
			return;
		}

		categories_in_analysis.Items.Add(new CategoryInAnalysisUserControl { Category = category, OnDelete = DeleteCategoryFromAnalysis });
	}

	void DeleteCategoryFromAnalysis(CategoryInAnalysisUserControl uc)
	{
		categories_in_analysis.Items.Remove(uc);
	}
}