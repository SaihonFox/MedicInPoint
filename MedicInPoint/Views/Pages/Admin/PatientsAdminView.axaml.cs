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
using MedicInPoint.ViewModels.UserControls.Items;
using MedicInPoint.Views.UserControls.Items;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MIP.LocalDB;

namespace MedicInPoint.Views.Pages.Admin;

public partial class PatientsAdminView : UserControl
{
	private ObservableCollection<Patient> AllPatients = [];
	private ObservableCollection<PatientItem_UserControl_View> AllPatientsView = [];
	public PatientsAdminViewModel ViewModel => (DataContext as PatientsAdminViewModel)!;

	public PatientsAdminView()
	{
		InitializeComponent();

		var collection = new BehaviorCollection
		{
			new TestAutoCompleteBehavior.Behaviors.AutoCompleteZeroMinimumPrefixLengthDropdownBehavior()
		};
		Interaction.SetBehaviors(acb, collection);

		using var context = new LocalDbContext();
		var names = context.PatientsAdminSearches.ToList().Select(aas => aas.name);
		acb.ItemsSource = names.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();

		acb.KeyDown += acb_KeyDown;
		acb.TextChanged += acb_TextChanged;

		if (!Design.IsDesignMode)
			Loaded += (s, e) => FillPatients();

		App.services.GetRequiredService<MedicSignalRConnections>().AnalysisCategoryConnection.On<Patient>("PatientAdded", patient => {
			AllPatients.Add(patient);
			FillWithSearch();
		});
		App.services.GetRequiredService<MedicSignalRConnections>().AnalysisCategoryConnection.On<Patient>("PatientDeleted", patient =>
		{
			var c = patients_list.Items.ToList().First(c => (c as Patient)!.Id == patient.Id) as Patient;
			AllPatients.Remove(c!);
			FillWithSearch();
		});
	}

	private void acb_TextChanged(object? sender, TextChangedEventArgs e) => FillWithSearch();

	private PatientItem_UserControl_View? selectedPatient = null;

	void OnSelectPatient(PatientItem_UserControl_View view, bool isSelected)
	{
		Dispatcher.UIThread.Invoke(() =>
		{
			if (selectedPatient != null)
			{
				selectedPatient.ViewModel.IsSelected = false;
				selectedPatient = null;
			}
			selectedPatient = isSelected ? view : null;
			ViewModel.SelectedPatient = selectedPatient?.ViewModel.Patient;
		});
	}

	async void acb_KeyDown(object? source, KeyEventArgs e)
	{
		using var context = new LocalDbContext();
		if (e.Key == Key.Enter && !acb.SearchText.IsNullOrWhiteSpace())
		{
			var list = await context.PatientsAdminSearches.ToListAsync();
			if (list.FirstOrDefault(a => a.name == acb.SearchText) != null || acb.SearchText.IsNullOrWhiteSpace())
				return;

			await context.PatientsAdminSearches.AddAsync(new() { name = acb.SearchText! });
			await context.SaveChangesAsync();

			var enumerable = acb.ItemsSource!.Cast<string>().ToList();
			enumerable.Add(acb.SearchText!);
			enumerable = [.. enumerable.Distinct()];
			acb.ItemsSource = enumerable;
		}
		await context.DisposeAsync();
	}

	async void FillPatients()
	{
		using var response = await APIService.For<IPatient>().GetPatients().ConfigureAwait(false);
		if (!response.IsSuccessful)
			return;
		
		AllPatients = [..response.Content!];
		foreach (var patient in AllPatients)
		{
			await Dispatcher.UIThread.InvokeAsync(() =>
			{
				var view = new PatientItem_UserControl_View
				{
					DataContext = new PatientItem_UserControl_ViewModel
					{
						Patient = patient,
						IsSelected = patient.Id == selectedPatient?.ViewModel.Patient?.Id
					},
					OnToggle = OnSelectPatient
				};
				AllPatientsView.Add(view);
				patients_list.Items.Add(view);
			});
		}

		FillWithSearch();
	}

	async void FillWithSearch()
	{
		await Dispatcher.UIThread.InvokeAsync(patients_list.Items.Clear);
		var patients = await Dispatcher.UIThread.InvokeAsync(() =>
			AllPatientsView.Where(x =>
				x.ViewModel.Patient!.FullName.Contains(Dispatcher.UIThread.Invoke(() => acb.Text!), StringComparison.CurrentCultureIgnoreCase))
			.ToList()
		);

		await Dispatcher.UIThread.InvokeAsync(() =>
		{
			if (patients.Find(x => x.ViewModel.Patient!.Id == selectedPatient?.ViewModel.Patient?.Id) == null && selectedPatient != null)
			{
				selectedPatient.ViewModel.IsSelected = false;
				selectedPatient = null;
			}
		});

		foreach (var patient in patients)
		{
			Dispatcher.UIThread.Invoke(() => {
				patients_list.Items.Add(patient);
			});
		}

		await Dispatcher.UIThread.InvokeAsync(() => {
			centerText.IsVisible = AllPatients.Count == 0;
			centerText.Text = "Пустой список";
		});
		await Dispatcher.UIThread.InvokeAsync(() =>
		{
			ViewModel.SelectedPatient = selectedPatient?.ViewModel.Patient;
		});
	}
}