using System.Collections.ObjectModel;

using Avalonia.Controls;
using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MedicInPoint.API.Refit;
using MedicInPoint.API.Refit.Placeholders;
using MedicInPoint.Models;

namespace MedicInPoint.ViewModels.Pages.Doctor;

public partial class PatientDoctorViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router;

	public PatientDoctorViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		Title = "Пациенты";
		_router = router;

		FillPatients();
	}

	async void FillPatients()
	{
		try
		{
			Patients = [..await APIService.For<IPatient>().GetPatients()];
			SelectedPatientIndex = 0;
		}
		catch (Exception ex)
		{
			File.WriteAllText("c:/users/ilnar/desktop/error.txt", ex.Message);
		}
	}

	[RelayCommand]
	public void Back() => _router.Back();

	[ObservableProperty]
	private ObservableCollection<Patient> _patients = !Design.IsDesignMode ? [] : [
			new Patient { Surname = "Тулькубаев", Name = "Ильнар", Sex = "Мужской", Phone = "89123456789", Birthday = new(2005, 04, 22), Passport = "8019123456" },
			new Patient { Surname = "Набиева", Name = "Лиана", Patronym = "Рамилевна", Sex = "Женский", Phone = "89098765432", Birthday = new(1999, 05, 25), Passport = "8020123456" },
		];

	[ObservableProperty]
	private Patient? _selectedPatient = null;

	[ObservableProperty]
	private int? _selectedPatientIndex = Design.IsDesignMode ? 0 : null;
}