using System.Collections.ObjectModel;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.SimpleRouter;

using CommunityToolkit.Mvvm.Input;

using MedicInPoint.ViewModels.Pages.Admin;
using MedicInPoint.ViewModels.Pages.Doctor;

using MenuItem = Medic.Theme.Controls.Custom.MenuItem;

namespace MedicInPoint.ViewModels.Pages;

public partial class MenuViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router;
	
	public ObservableCollection<object> AdminMenu => [
		new MenuItem { Text = "Список анализов", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/analyses_btn.png"))), Command = new RelayCommand(AdminAnalyses) },
		new MenuItem { Text = "Список пациентов", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/patients_btn.png"))) },
		new MenuItem { Text = "Список сотрудников", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/users_btn.png"))) },
		new MenuItem { Text = "Список категорий", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/categories_btn.png"))), Command = new RelayCommand(AdminAnalysisCategories) },

		new TextBlock { Text = "Документы", FontSize = 40, HorizontalAlignment = HorizontalAlignment.Center },
	];

	public ObservableCollection<object> DoctorMenu => [
		new MenuItem { Text = "Список анализов", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/analyses_btn.png"))), Command = new RelayCommand(DoctorPatients) },
		new MenuItem { Text = "Список категорий", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/categories_btn.png"))), Command = new RelayCommand(AdminAnalysisCategories) },

		new TextBlock { Text = "Документы", FontSize = 40, HorizontalAlignment = HorizontalAlignment.Center },
	];
	
	public MenuViewModel(NestedHistoryRouter<ViewModelBase, MainViewModel> router) : this()
	{
		_router = router;
		Title = "Меню";
	}

	[RelayCommand]
	public void AdminAnalyses() => _router.GoTo<AnalysisAdminViewModel>();

	[RelayCommand]
	public void DoctorPatients() => _router.GoTo<PatientDoctorViewModel>();

	[RelayCommand]
	private void AdminAnalysisCategories() => _router.GoTo<AnalysisCategoriesAdminViewModel>();
}