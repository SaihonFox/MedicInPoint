using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

using Medic.API.Refit.Placeholders;

using MedicInPoint.API.Refit;
using MedicInPoint.Extensions;
using MedicInPoint.Models;
using MedicInPoint.Services;
using MedicInPoint.ViewModels.Pages.Doctor;
using MedicInPoint.Views.UserControls.Items;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MIP.LocalDB;

namespace MedicInPoint.Views.Pages.Doctor;

public partial class PatientDoctorView : UserControl
{
	public PatientDoctorViewModel ViewModel => (DataContext as PatientDoctorViewModel)!;

	public PatientDoctorView()
	{
		InitializeComponent();

		var collection = new BehaviorCollection
		{
			new TestAutoCompleteBehavior.Behaviors.AutoCompleteZeroMinimumPrefixLengthDropdownBehavior()
		};
		Interaction.SetBehaviors(acb, collection);

		using var context = new LocalDbContext();
		var names = context.PatientDoctorSearches.ToList().Select(aas => aas.name);
		acb.ItemsSource = names.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();

		acb.KeyDown += acb_KeyDown;

		if (!Design.IsDesignMode)
		{
			patients.SelectionChanged += (_, _) => FillOrdersInProcesses();
			proccesses_rb.IsCheckedChanged += (_, _) => FillOrdersInProcesses();
		}
	}

	async void acb_KeyDown(object? source, KeyEventArgs e)
	{
		using var context = new LocalDbContext();
		if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(acb.SearchText))
		{
			var list = await context.PatientDoctorSearches.ToListAsync();
			if (list.FirstOrDefault(a => a.name == acb.SearchText) != null || acb.SearchText.IsNullOrWhiteSpace())
				return;

			context.PatientDoctorSearches.Add(new() { name = acb.SearchText });
			await context.SaveChangesAsync();

			var enumerable = acb.ItemsSource!.Cast<string>().ToList();
			enumerable.Add(acb.SearchText);
			enumerable = [.. enumerable.Distinct()];
			acb.ItemsSource = enumerable;
		}
		await context.DisposeAsync();
	}

	async void FillOrdersInProcesses()
	{
		var ViewModel = (DataContext as PatientDoctorViewModel)!;

		if (!proccesses_rb.IsChecked!.Value || ViewModel?.SelectedPatient == null)
			return;

		processes_ic.Items.Clear();
		using var response = await APIService.For<IAnalysisOrder>().GetAnalysisOrders4User(ViewModel.CurrentUser!.Id);
		var list = response.Content!.Where(x => x.AnalysisOrderStateId == 1 && x.PatientId == ViewModel.SelectedPatient!.Id).ToList();
		
		foreach (var item in list)
			processes_ic.Items.Add(new ProcessingAnalysesUserControl { CurrentOrder = item, OnApply = OnApply });
	}

	void OnApply(ProcessingAnalysesUserControl uc)
	{
		processes_ic.Items.Remove(uc);

		var order = ViewModel.UserOrders.First(x => x.Id == uc.CurrentOrder.Id);
		var orderIndex = ViewModel.UserOrders.IndexOf(order);
		ViewModel.UserOrders[orderIndex].AnalysisOrderStateId = 2;
		var item = requests_ic.Items[orderIndex] as AnalysisOrder;
		item.AnalysisOrderStateId = 2;
		requests_ic.UpdateLayout();
	}
}