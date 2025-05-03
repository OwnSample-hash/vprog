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

    MainWindowDataContext MainWindowDataContext { get; set; }

    static MainWindowDataContext MainWindowDataContextStatic { get; set; } = new();

    static SqlConnection SqlConnection = new(MainWindow.conString);

    AdminTool.AdminTool? adminTool = null;

    public Main() {
      InitializeComponent();
      KeepAlive = true;
      MainWindowDataContext = new MainWindowDataContext();
      MainWindowDataContextStatic = MainWindowDataContext;
      DataContext = MainWindowDataContext;

      frStatus.Navigate(new Session.Session(MainWindowDataContext,
        (User user) => {
          MainWindowDataContext.Status.Status = $"Bejelentkezve {user.Username}\nEgyenleg: {user.Balance}";
        }
        ));

      var cars = SqlConnection.Query<Car>("SELECT * FROM Cars").ToList();
      cars.ForEach((c) => MainWindow.Logger.SysLog($"Got {c.Name} for {c.Price}", ELogLvl.TRACE));
      Application.Current.Dispatcher.Invoke(() => {
        MainWindowDataContext.cars.Clear();
        cars.ForEach((c) => MainWindowDataContext.cars.Add(c));
      });
      cars.ForEach((c) => c.Pics = MainWindow.CM.GetPicsByCarId(c.Id));

      Session.Session.LoginEvent += () => {
        MainWindow.Logger.SysLog("Login event triggered", ELogLvl.TRACE);
        MainWindowDataContext.AdminVisibility.OnPropertyChanged("AdminVisibility");
        MainWindowDataContext.SellerVisiblity.OnPropertyChanged("SellerVisibility");
        MainWindowDataContext.UserVisibility.OnPropertyChanged("UserVisibility");
      };

      Session.Session.LogoutEvent += () => {
        MainWindow.Logger.SysLog("Logout event triggered", ELogLvl.TRACE);
        if (adminTool != null) {
          MainWindow.Logger.SysLog("Closing admin tool", ELogLvl.TRACE);
          adminTool.Close();
          adminTool.Dispose();
          adminTool = null;
        }
        MainWindowDataContext.AdminVisibility.OnPropertyChanged("AdminVisibility");
        MainWindowDataContext.SellerVisiblity.OnPropertyChanged("SellerVisibility");
        MainWindowDataContext.UserVisibility.OnPropertyChanged("UserVisibility");
      };
    }

    public static void FetchCars() {
      var cars = SqlConnection.Query<Car>("SELECT * FROM Cars").ToList();
      cars.ForEach((c) => MainWindow.Logger.SysLog($"Got {c.Name} for {c.Price}", ELogLvl.TRACE));
      Application.Current.Dispatcher.Invoke(() => {
        MainWindowDataContextStatic.cars.Clear();
        cars.ForEach((c) => MainWindowDataContextStatic.cars.Add(c));
      });
      cars.ForEach((c) => c.Pics = MainWindow.CM.GetPicsByCarId(c.Id));
    }

    private void miAdmin_Click(object sender, RoutedEventArgs e) {
      if (adminTool != null && adminTool.IsOpen) {
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

    private void miUpdate_Click(object sender, RoutedEventArgs e) {
      FetchCars();
    }

    private void miInv_click(object sender, RoutedEventArgs e) {
      if (!User.IsUserValid(Session.Session.User)) {
        MainWindow.Logger.SysLog("User not logged in", ELogLvl.ERROR);
        MessageBox.Show("Be kell jelentkezned a folytatáshoz!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      MainWindowDataContext newDC = new();
      newDC.Status = MainWindowDataContext.Status;
      newDC.AdminVisibility = MainWindowDataContext.AdminVisibility;
      newDC.SellerVisiblity = MainWindowDataContext.SellerVisiblity;
      var cars = SqlConnection.Query<Car>("SELECT * FROM Cars WHERE Id IN (SELECT CarId FROM Transactions WHERE AccountId = @Id)", new { Id = Session.Session.User.Id }).ToList();
      newDC.cars.Clear();
      cars.ForEach((c) => MainWindow.Logger.SysLog($"Got {c.Name} for {c.Price}", ELogLvl.TRACE));
      cars.ForEach((c) => c.Pics = MainWindow.CM.GetPicsByCarId(c.Id));
      cars.ForEach(newDC.cars.Add);
      MainWindow.MainPage.NavigationService?.Navigate(new Inventory.Inventory(newDC));
    }
  }
}
