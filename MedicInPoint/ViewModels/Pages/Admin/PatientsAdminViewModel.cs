using System.Collections.ObjectModel;

using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.Pages.Admin;

public partial class PatientsAdminViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;

	[ObservableProperty]
	private ObservableCollection<Patient> _allPatients = [];

	[ObservableProperty]
	private ObservableCollection<Patient> _patientsList = [];

	public PatientsAdminViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		_router = router;
		Title = "Список пациентов";

		FillPatients();
	}

	async void FillPatients()
	{
		var response = await APIService.For<IPatient>().GetPatients();
		if (!response.IsSuccessStatusCode)
			return;

		AllPatients = [.. response.Content!];
		//SelectedPatientIndex = 0;
		Log("none", "PatientDoctor");
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

	async partial void OnSearchTextChanged(string value)
	{
		value = value.Trim();
		var response = await APIService.For<IPatient>().GetPatients();
		if (response.IsSuccessStatusCode)
			return;

		PatientsList = [.. response.Content!];
	}
}