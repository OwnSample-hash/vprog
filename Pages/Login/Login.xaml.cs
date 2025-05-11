using System.Windows;
using System.Windows.Controls;
using Dapper;
using Microsoft.Data.SqlClient;
using BC = BCrypt.Net.BCrypt;

namespace car.Pages.Login {
  /// <summary>
  /// Interaction logic for Login.xaml
  /// </summary>
  public sealed partial class Login : Page {

    private readonly SqlConnection _connection = new(MainWindow.conString);

    private Action<User, bool> _storeUser;

    public Login(Action<User, bool> storeUser) {
      InitializeComponent();
      this.KeepAlive = false;
      _storeUser = storeUser;
      tbNev.Focus();
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e) {
      if (tbNev.Text != "" && pbJelszo.Password != "") {
        var user = _connection.Query<User>
          ("SELECT * FROM Users WHERE Username = @Username", new { Username = tbNev.Text }).FirstOrDefault();
        if (user == null) {
          tbNev.BorderBrush = System.Windows.Media.Brushes.Red;
          tbNev.BorderThickness = new Thickness(2);
          tbNev.Text = "";
          tbNev.GotFocus += RemoveBorder;
          pbJelszo.BorderBrush = System.Windows.Media.Brushes.Red;
          pbJelszo.BorderThickness = new Thickness(2);
          pbJelszo.Password = "";
          pbJelszo.GotFocus += RemoveBorder;
          MainWindow.Logger.SysLog("Login failed");
          MessageBox.Show("Sikertelen bejelentkezés!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }
        if (BC.Verify(pbJelszo.Password, user.Password)) {
          _storeUser(user, true);
          return;
        } else {
          _storeUser(User.getEmpty(), false);
          tbNev.BorderBrush = System.Windows.Media.Brushes.Red;
          tbNev.BorderThickness = new Thickness(2);
          tbNev.Text = "";
          tbNev.GotFocus += RemoveBorder;
          pbJelszo.BorderBrush = System.Windows.Media.Brushes.Red;
          pbJelszo.BorderThickness = new Thickness(2);
          pbJelszo.Password = "";
          pbJelszo.GotFocus += RemoveBorder;
          MainWindow.Logger.SysLog("Login failed");
          MessageBox.Show("Sikertelen bejelentkezés!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }
      } else {
        MessageBox.Show("A mezőket ki kell tölteni!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
    private void RemoveBorder(object sender, object _) {
      if (sender is TextBox tb) {
        tb.BorderThickness = new Thickness(0);
        tb.BorderBrush = System.Windows.Media.Brushes.Transparent;
        tb.GotFocus -= RemoveBorder;
      } else if (sender is PasswordBox pb) {
        pb.BorderThickness = new Thickness(0);
        pb.BorderBrush = System.Windows.Media.Brushes.Transparent;
        pb.GotFocus -= RemoveBorder;
      }
    }

    private void pbJelszo_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
      if (e.Key == System.Windows.Input.Key.Enter) {
        btnLogin_Click(sender, e);
      }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) {
      MainWindow.MainPage.NavigationService?.GoBack();
      MainWindow.MainPage.NavigationService?.RemoveBackEntry();
    }
  }
}
