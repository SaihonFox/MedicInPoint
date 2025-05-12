using System.Collections.ObjectModel;

using Avalonia.Controls;
using Avalonia.Input;
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

namespace MedicInPoint.Views.Pages.Admin;

public partial class AnalysesAdminView : UserControl
{
	private ObservableCollection<Analysis> AllAnalyses = [];
	private ObservableCollection<AnalysisCategory> AllCategories = [];
	private ObservableCollection<AnalysisItem_UserControl_View> AllAnalysesView = [];
	public AnalysesAdminViewModel ViewModel => (DataContext as AnalysesAdminViewModel)!;

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

		categories_list.SelectionChanged += async(_, _) => await FillWithSearch();

		if (!Design.IsDesignMode)
		{
			Loaded += async(s, e) =>
			{
				await FillCategories().ContinueWith(async t => await FillAnalyses());
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
		App.services.GetRequiredService<INotificationService>().Show("Уведомление", "Загрузка списка");
		using var response = await APIService.For<IAnalysisCategory>().GetAnalysisCategories().ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;

		AllCategories = [.. response.Content!];

		var allCategory = new AnalysisCategory { Id = 0, Name = "Все" };
		AllCategories.Insert(0, allCategory);
		foreach (var category in AllCategories)
		{
			await Dispatcher.UIThread.InvokeAsync(() =>
			{
				categories_list.Items.Add(category);
			});
		}

		categories_list.SelectedItem = allCategory;
		categories_list.UpdateLayout();

		foreach(var category in AllCategories)
			AnalysisDrawerView.AllCategories.Add(category);
	}

	async Task FillAnalyses()
	{
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
		await Dispatcher.UIThread.InvokeAsync(analyses_list.Items.Clear);
		var analyses = await Dispatcher.UIThread.InvokeAsync(() => {
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

		await Dispatcher.UIThread.InvokeAsync(() =>
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
			});
		}

		await Dispatcher.UIThread.InvokeAsync(() => {
			centerText.IsVisible = analyses.Count == 0;
			centerText.Text = "Пустой список";
		});
		await Dispatcher.UIThread.InvokeAsync(() =>
		{
			ViewModel.SelectedAnalysis = selectedAnalysis?.ViewModel?.Analysis;
			drawer.ViewModel.Analysis = ViewModel.SelectedAnalysis;
		});
	}
}