using System.Collections.ObjectModel;

using Avalonia.SimpleRouter;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.API.SignalR;
using MedicInPoint.Extensions;
using MedicInPoint.Models;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class PatientsAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;

	[ObservableProperty]
	private ObservableCollection<Patient> _allPatients = [];

	[ObservableProperty]
	private ObservableCollection<Patient> _patientsList = [];

	public PatientsAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router, MedicSignalRConnections connections) : this()
	{
		_router = router;
		Title = "Список пациентов";

		FillPatients();

		connections.AnalysisCategoryConnection.On<Patient>("PatientAdded", patient => {
			AllPatients.Add(patient);
		});
		connections.AnalysisCategoryConnection.On<Patient>("PatientDeleted", category =>
		{
			var c = AllPatients.First(c => c.Id == category.Id);
			AllPatients.Remove(c);
		});
		AllPatients.CollectionChanged += (_, e) => OnSearchTextChanged(SearchText);
	}

	async void FillPatients()
	{
		var response = await APIService.For<IPatient>().GetPatients();
		if (!response.IsSuccessStatusCode)
			return;

		AllPatients = [.. response.Content!];
		PatientsList = SearchPatient();
	}

	ObservableCollection<Patient> SearchPatient() => [.. AllPatients.Where(a =>
		new[] {
			a.FullName, $"{a.Email}", a.Passport, a.Phone, a.Sex
		}.Any(s => s?.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase) ?? false) )
	];

	[RelayCommand]
	private void Back() => _router.Back();

	[ObservableProperty]
	private string _searchText = string.Empty;

	partial void OnSearchTextChanged(string value)
	{
		value = value.Trim();

		PatientsList = value.IsNullOrWhiteSpace() ? [..AllPatients] : SearchPatient();
	}
}