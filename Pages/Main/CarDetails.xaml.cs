using System.Windows.Controls;
using car.Models;

namespace car.Pages.Main {
  /// <summary>
  /// Interaction logic for CarDetails.xaml
  /// </summary>
  public partial class CarDetails : Page {
    public CarDetails(Car Car) {
      InitializeComponent();
      DataContext = new CarDetailsDC(Car);
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e) {
      MainWindow.MainPage.NavigationService?.GoBack();
      MainWindow.MainPage.NavigationService?.RemoveBackEntry();
    }
  }
}
