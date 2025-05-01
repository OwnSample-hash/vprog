using System.Windows;
using System.Windows.Controls;
using car.Models;
using Microsoft.Data.SqlClient;
using Z.Dapper.Plus;

namespace car.Pages.Main {
  /// <summary>
  /// Interaction logic for CarDetails.xaml
  /// </summary>
  public partial class CarDetails : Page {
    Car car;
    public CarDetails(Car Car, bool ShowBuyBtn = true) {
      InitializeComponent();
      car = Car;
      DataContext = new CarDetailsDC(Car);
      if (ShowBuyBtn)
        btnBuy.Visibility = Visibility.Visible;
      else
        btnBuy.Visibility = Visibility.Collapsed;
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      MainWindow.MainPage.NavigationService?.GoBack();
      MainWindow.MainPage.NavigationService?.RemoveBackEntry();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e) {
      User user = Session.Session.User;
      if (!User.IsUserValid(user)) {
        MainWindow.Logger.SysLog("User not logged in", Logging.ELogLvl.ERROR);
        MessageBox.Show("Be kell jelentkezned a folytatáshoz!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      if (user.Balance < car.Price) {
        MainWindow.Logger.Log("User has not enough money", Logging.ELogLvl.ERROR);
        MessageBox.Show("Nincs elég pénzed a vásárláshoz!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      var transaction = new Transaction {
        CarId = car.Id,
        AccountId = user.Id,
        SellerId = car.SellerId,
        Amount = car.Price
      };
      MainWindow.Logger.SysLog($"Transaction: {transaction}", Logging.ELogLvl.TRACE);
      SqlConnection connection = new(MainWindow.conString);
      connection.Open();
      connection.BulkInsert(transaction);
      connection.Close();
      user.Balance -= car.Price;
      MainWindow.Logger.SysLog($"Transaction: {transaction} inserted", Logging.ELogLvl.TRACE);
      MainWindow.Logger.SysLog($"Car: {car.Name} has been removed");
      MainWindow.Logger.Log($"Bought {car.Name} for {car.Price}");
      MessageBox.Show($"Vásárlás sikeres!\nAutó: {car.Name}\nÁr: {car.Price}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
      MainWindow.MainPage.NavigationService?.GoBack();
      MainWindow.MainPage.NavigationService?.RemoveBackEntry();
    }
  }
}
