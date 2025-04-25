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
      // TODO: Add error handling/logging
      if (tbNev.Text != "" && pbJelszo.Password != "") {
        var user = _connection.Query<User>
          ("SELECT * FROM Users WHERE Username = @Username", new { Username = tbNev.Text }).FirstOrDefault();
        if (user == null) {
          return;
        }
        if (BC.Verify(pbJelszo.Password, user.Password)) {
          _storeUser(user, true);
          return;
        } else {
          _storeUser(User.getEmpty(), false);
          return;
        }
      } else {
        MessageBox.Show("A mezőket ki kell tölteni!");
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
