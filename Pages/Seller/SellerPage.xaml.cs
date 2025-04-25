using System.Windows;
using System.Windows.Controls;

namespace car.Pages.Seller {
  /// <summary>
  /// Interaction logic for SellerPage.xaml
  /// </summary>
  public partial class SellerPage : Page {
    public SellerPage() {
      InitializeComponent();
    }

    private void miBack_Click(object sender, RoutedEventArgs e) {
      MainWindow.MainPage.NavigationService?.GoBack();
      MainWindow.MainPage.NavigationService?.RemoveBackEntry();
    }
  }
}
