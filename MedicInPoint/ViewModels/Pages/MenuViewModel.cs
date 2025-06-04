using System.Collections.ObjectModel;
using System.Windows.Input;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.SimpleRouter;
using Avalonia.Styling;

using CommunityToolkit.Mvvm.Input;

using MedicInPoint.Extensions;
using MedicInPoint.Services;
using MedicInPoint.ViewModels.Pages.Admin;
using MedicInPoint.ViewModels.Pages.Admin.Documents;
using MedicInPoint.ViewModels.Pages.Doctor;

using MenuItem = Medic.Theme.Controls.Custom.MenuItem;

namespace MedicInPoint.ViewModels.Pages;

#pragma warning disable CA1822
public partial class MenuViewModel() : ViewModelBase
{
	private readonly NestedHistoryRouter<ViewModelBase, MainViewModel> _router = null!;
	private readonly IAppStateService _appService = null!;

	public ObservableCollection<object> AdminMenu => [
		new MenuItem { Text = "Список анализов", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/analyses_btn.png"))), HotKey = new KeyGesture(Key.D1, KeyModifiers.Control), Command = new RelayCommand(AdminAnalyses), Margin = new Thickness(5) },
		new MenuItem { Text = "Список сотрудников", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/users_btn.png"))), HotKey = new KeyGesture(Key.D3, KeyModifiers.Control), Command = new RelayCommand(AdminUsers), Margin = new Thickness(5) },
		new MenuItem { Text = "Список пациентов", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/patients_btn.png"))), HotKey = new KeyGesture(Key.D2, KeyModifiers.Control), Command = new RelayCommand(AdminPatients), Margin = new Thickness(5) },
		new MenuItem { Text = "Список категорий", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/categories_btn.png"))), HotKey = new KeyGesture(Key.D4, KeyModifiers.Control), Command = new RelayCommand(AdminAnalysisCategories), Margin = new Thickness(5) },

		new TextBlock { Text = "Документы", FontSize = 40, LineHeight = 40, HorizontalAlignment = HorizontalAlignment.Center },

		new ScrollViewer {
			Content = new WrapPanel { ItemsAlignment = WrapPanelItemsAlignment.Center }.Also(x => {
				x.Children.AddRange([
					//BorderDocument("Отчеты\nлаборантов", AdminDocumentUsersCommand),
					//BorderDocument("Отчеты\nпациентов", AdminDocumentPatientsCommand),
					BorderDocument("Отчеты\nрезультатов\nанализов", AdminDocumentAnalysesCommand),
				]);
			}),
		},
	];

	public ObservableCollection<object> DoctorMenu => [
		new MenuItem { Text = "Медицинские карты пациентов", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/patients_btn.png"))), HotKey = new KeyGesture(Key.D1, KeyModifiers.Control), Command = new RelayCommand(DoctorPatients), Margin = new Thickness(5) },
		new MenuItem { Text = "Запросы и запись", ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://MedicInPoint/Assets/Images/buttons/menu/requests_and_records_btn.png"))), HotKey = new KeyGesture(Key.D2, KeyModifiers.Control), Command = new RelayCommand(DoctorRnR), Margin = new Thickness(5) },

		new TextBlock { Text = "Документы", FontSize = 40, LineHeight = 40, HorizontalAlignment = HorizontalAlignment.Center },

		new ScrollViewer {
			Content = new WrapPanel { ItemsAlignment = WrapPanelItemsAlignment.Center }.Also(x => {
				x.Children.AddRange([
					BorderDocument("Отчеты\nрезультатов\nанализов", DoctorDocumentAnalysesCommand),
				]);
			}),
		},
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

	#region Documents
	[RelayCommand]
	private void AdminDocumentUsers() => _router.GoTo<UsersAdminDocumentsViewModel>();

	[RelayCommand]
	private void AdminDocumentPatients() => _router.GoTo<PatientsAdminDocumentsViewModel>();

	[RelayCommand]
	private void AdminDocumentAnalyses() => _router.GoTo<AnalysesAdminDocumentsViewModel>();

	[RelayCommand]
	private void DoctorDocumentAnalyses() => _router.GoTo<AnalysesAdminDocumentsViewModel>();
	#endregion Documents

	private Button BorderDocument(string text, ICommand? command = null)
	{
		var button = new Button
		{
			ClipToBounds = false,
			Margin = new Thickness(10, 5),
			Padding = new Thickness(10),

			MinWidth = 100,
			MinHeight = 50,

			Template = new FuncControlTemplate((_, _) =>
			{
				return new Border
				{
					Child = new ContentPresenter
					{
						Content = text,
						FontSize = 24,
						TextAlignment = TextAlignment.Center,
						TextWrapping = TextWrapping.WrapWithOverflow,
					}
				}.Also(x =>
				{
					x.Classes.Add("drawerbg");
					x.Styles.Add(new Style { Selector = Selectors.Class(null, ":pointerover") }.Also(x =>
					{
						x.Setters.Add(new Setter(TemplatedControl.BackgroundProperty, SolidColorBrush.Parse("#F3F3F3")));
					}));
				});
			})
		};
		button.Command = command;

		return button;
	}
}
#pragma warning restore CA1822