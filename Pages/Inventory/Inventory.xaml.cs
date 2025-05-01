using System.Windows;
using System.Windows.Controls;
using car.Logging;
using car.Models;
using car.Pages.Main;

namespace car.Pages.Inventory {
  /// <summary>
  /// Interaction logic for Inventory.xaml
  /// </summary>
  public partial class Inventory : Page {
    public Inventory(Main.MainWindowDataContext MWDC) {
      InitializeComponent();
      DataContext = MWDC;
    }
    private void miBack_Click(object sender, RoutedEventArgs e) {
      MainWindow.MainPage.NavigationService?.GoBack();
      MainWindow.MainPage.NavigationService?.RemoveBackEntry();
    }
    private void Button_Click(object sender, RoutedEventArgs e) {
      var button = sender as Button;
      if (button?.DataContext is Car item) {
        MainWindow.MainPage.NavigationService?.Navigate(new CarDetails(item, false));
      } else {
        MainWindow.Logger.SysLog("Item is null", ELogLvl.ERROR);
      }
    }
  }
}
