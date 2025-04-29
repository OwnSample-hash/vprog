using System.Windows;
using System.Windows.Controls;
using car.Logging;
using car.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace car.Pages.Main {
  /// <summary>
  /// Interaction logic for Main.xaml
  /// </summary>
  public partial class Main : Page {

    public MainWindowDataContext mainWindowDataContext { get; private set; } = null!;

    private SqlConnection SqlConnection = new(MainWindow.conString);

    AdminTool.AdminTool? adminTool = null;

    public Main() {
      InitializeComponent();
      KeepAlive = true;
      MainWindowDataContext mainWindowDataContext = new();
      DataContext = mainWindowDataContext;

      var cars = SqlConnection.Query<Car>("SELECT * FROM Cars").ToList();
      cars.ForEach((c) => MainWindow.Logger.SysLog($"Got {c.Name} for {c.Price}", ELogLvl.TRACE));
      mainWindowDataContext.cars.cars = [.. cars];

      Session.Session.LoginEvent += () => {
        MainWindow.Logger.SysLog("Login event triggered", ELogLvl.TRACE);
        mainWindowDataContext.AdminVisibility.OnPropertyChanged("AdminVisibility");
        mainWindowDataContext.SellerVisiblity.OnPropertyChanged("SellerVisibility");
      };

      Session.Session.LogoutEvent += () => {
        MainWindow.Logger.SysLog("Logout event triggered", ELogLvl.TRACE);
        if (adminTool != null) {
          MainWindow.Logger.SysLog("Closing admin tool", ELogLvl.TRACE);
          adminTool.Close();
          adminTool.Dispose();
          adminTool = null;
        }
        mainWindowDataContext.AdminVisibility.OnPropertyChanged("AdminVisibility");
        mainWindowDataContext.SellerVisiblity.OnPropertyChanged("SellerVisibility");
      };
    }

    private void miAdmin_Click(object sender, RoutedEventArgs e) {
      if (adminTool != null) {
        MainWindow.Logger.SysLog("Admin tool already open", ELogLvl.TRACE);
        adminTool.Focus();
        return;
      }
      adminTool = new AdminTool.AdminTool();
      adminTool.Show();
    }

    private void miSeller_Click(object sender, RoutedEventArgs e) {
      MainWindow.MainPage.NavigationService?.Navigate(new Seller.SellerPage());
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      var button = sender as Button;
      if (button?.DataContext is Car item) {
        MainWindow.MainPage.NavigationService?.Navigate(new CarDetails(item));
      } else {
        MainWindow.Logger.SysLog("Item is null", ELogLvl.ERROR);
      }
    }
  }
}
