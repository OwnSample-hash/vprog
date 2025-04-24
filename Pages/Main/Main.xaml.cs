using System.Windows;
using System.Windows.Controls;

namespace car.Pages.Main {
  /// <summary>
  /// Interaction logic for Main.xaml
  /// </summary>
  public partial class Main : Page {
    public Main() {
      InitializeComponent();
    }
    private void miAdmin_Click(object sender, RoutedEventArgs e) {
      new AdminTool.AdminTool().Show();
    }

    private void miSeller_Click(object sender, RoutedEventArgs e) {

    }
  }
}
