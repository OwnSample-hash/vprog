using System.Windows;
using System.Windows.Controls;
using car.Logging;
using car.models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace car.Pages.Main {
  /// <summary>
  /// Interaction logic for Main.xaml
  /// </summary>
  public partial class Main : Page {

    public MainWindowDataContext mainWindowDataContext { get; private set; } = null!;

    private SqlConnection SqlConnection = new(MainWindow.conString);

    public Main() {
      InitializeComponent();
      KeepAlive = true;
      MainWindowDataContext mainWindowDataContext = new();
      DataContext = mainWindowDataContext;

      mainWindowDataContext.cars = SqlConnection.Query<List<Car>>("select * from cars").ToList();

      Pages.Session.Session.LoginEvent += () => {
        MainWindow.Logger.SysLog("Login event triggered", ELogLvl.TRACE);
        mainWindowDataContext.AdminVisibility.OnPropertyChanged("AdminVisibility");
        mainWindowDataContext.SellerVisiblity.OnPropertyChanged("SellerVisibility");
      };

      Pages.Session.Session.LogoutEvent += () => {
        MainWindow.Logger.SysLog("Logout event triggered", ELogLvl.TRACE);
        mainWindowDataContext.AdminVisibility.OnPropertyChanged("AdminVisibility");
        mainWindowDataContext.SellerVisiblity.OnPropertyChanged("SellerVisibility");
      };
    }

    private void miAdmin_Click(object sender, RoutedEventArgs e) {
      new AdminTool.AdminTool().Show();
    }

    private void miSeller_Click(object sender, RoutedEventArgs e) {
      MainWindow.MainPage.NavigationService?.Navigate(new Seller.SellerPage());
    }
  }
}
