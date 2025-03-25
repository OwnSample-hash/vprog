using System.Windows;
using System.Windows.Controls;
using Dapper;
using Microsoft.Data.SqlClient;
using BC = BCrypt.Net.BCrypt;

namespace car.Session {
  public partial class Session : Control {

    private readonly SqlConnection _connection = new(MainWindow.conString);

    public static User User { get; private set; } = User.getEmpty();

    private ESessionAuthError _eSessionAuthError = ESessionAuthError.OK;
    public void Login(object sender, RoutedEventArgs e) {
      var login = new Login(_eSessionAuthError);
      if (login.ShowDialog() == true) {
        var user = _connection.Query<User>
          ("SELECT * FROM Users WHERE Username = @Username", new { login.Username }).FirstOrDefault();
        if (user == null) {
          _eSessionAuthError = ESessionAuthError.GeneralError;
          ((Button)sender).RaiseEvent(new(Button.ClickEvent));
          _eSessionAuthError = ESessionAuthError.OK;
          return;
        } else
          User = user;
        if (BC.Verify(login.Password, User.Password)) {
          MainWindow.Logger.Log($"User {User.Username} logged in");
          this.Type = User.Permission;
        } else {
          _eSessionAuthError = ESessionAuthError.InvalidCredentials;
          User = User.getEmpty();
          ((Button)sender).RaiseEvent(new(Button.ClickEvent));
          _eSessionAuthError = ESessionAuthError.OK;
          return;
        }
      }
    }

    public void Logout(object sender, RoutedEventArgs e) {
      User = User.getEmpty();
      this.Type = ESessionType.None;
    }
  }
}
