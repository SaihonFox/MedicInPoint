using System.Collections.ObjectModel;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.Input;

using MedicInPoint.Services;
using MedicInPoint.ViewModels.Pages.Admin;
using MedicInPoint.ViewModels.Pages.Doctor;

using MenuItem = Medic.Theme.Controls.Custom.MenuItem;

namespace MedicInPoint.ViewModels.Pages;

public partial class MenuViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	private readonly IAppStateService _appService = null!;

	public ObservableCollection<object> AdminMenu => [
		new MenuItem { Text = "Список анализов", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/analyses_btn.png"))), HotKey = new KeyGesture(Key.D1, KeyModifiers.Control), Command = new RelayCommand(AdminAnalyses) },
		new MenuItem { Text = "Список сотрудников", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/users_btn.png"))), HotKey = new KeyGesture(Key.D3, KeyModifiers.Control), Command = new RelayCommand(AdminUsers) },
		new MenuItem { Text = "Список пациентов", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/patients_btn.png"))), HotKey = new KeyGesture(Key.D2, KeyModifiers.Control), Command = new RelayCommand(AdminPatients) },
		new MenuItem { Text = "Список категорий", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/categories_btn.png"))), HotKey = new KeyGesture(Key.D4, KeyModifiers.Control), Command = new RelayCommand(AdminAnalysisCategories) },

		new TextBlock { Text = "Документы", FontSize = 40, LineHeight = 40, HorizontalAlignment = HorizontalAlignment.Center },
	];

	public ObservableCollection<object> DoctorMenu => [
		new MenuItem { Text = "Медицинские карты пациентов", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/patients_btn.png"))), HotKey = new KeyGesture(Key.D1, KeyModifiers.Control), Command = new RelayCommand(DoctorPatients) },
		new MenuItem { Text = "Запросы и запись", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/requests_and_records_btn.png"))), HotKey = new KeyGesture(Key.D2, KeyModifiers.Control), Command = new RelayCommand(DoctorRnR) },

		new TextBlock { Text = "Документы", FontSize = 40, LineHeight = 40, HorizontalAlignment = HorizontalAlignment.Center },
	];

	public ObservableCollection<object> Menu => _appService.CurrentUser!.Post == 1 ? AdminMenu : DoctorMenu;
	
	public MenuViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router, IAppStateService appService) : this()
	{
		_router = router;
		_appService = appService;
		
		Title = "Меню " + (_appService.CurrentUser!.Post == 1 ? "Администратора" : "Доктора");
	}

	[RelayCommand]
	private void Back()
	{
		_appService.CurrentUser = null;
		_router.Back();
	}

	#region Admin Commands
	[RelayCommand]
	private void AdminAnalyses() => _router.GoTo<AnalysesAdminViewModel>();

	[RelayCommand]
	private void AdminUsers() => _router.GoTo<UsersAdminViewModel>();

	[RelayCommand]
	private void AdminPatients() => _router.GoTo<PatientsAdminViewModel>();

	[RelayCommand]
	private void AdminAnalysisCategories() => _router.GoTo<AnalysisCategoriesAdminViewModel>();
	#endregion Admin Commands

	#region Doctor Commands
	[RelayCommand]
	private void DoctorPatients() => _router.GoTo<PatientDoctorViewModel>();

	[RelayCommand]
	private void DoctorRnR() => _router.GoTo<RnRDoctorViewModel>();
	#endregion Doctor Commands
}