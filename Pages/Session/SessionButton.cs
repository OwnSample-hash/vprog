using System.Windows;
using System.Windows.Controls;
using Dapper;
using Microsoft.Data.SqlClient;
using BC = BCrypt.Net.BCrypt;

namespace car.Pages.Session {
  public partial class Session : Page {

    private readonly SqlConnection _connection = new(MainWindow.conString);

    public static User User { get; private set; } = User.getEmpty();

    private ESessionAuthError _eSessionAuthError = ESessionAuthError.OK;

    public delegate void LoginEventHandler();

    public static event LoginEventHandler? LoginEvent;

    public static event LoginEventHandler? LogoutEvent;

    public void Login(object sender, RoutedEventArgs e) {
      var login = new Login.Login(_eSessionAuthError);
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
          MainWindow.Logger.SysLog("Calling events", Logging.ELogLvl.TRACE);
          LoginEvent?.Invoke();
          btLogin.Visibility = Visibility.Collapsed;
          btLogout.Visibility = Visibility.Visible;
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
      MainWindow.Logger.Log("User logged out");
      MainWindow.Logger.SysLog("Calling events", Logging.ELogLvl.TRACE);
      LogoutEvent?.Invoke();
      btLogin.Visibility = Visibility.Visible;
      btLogout.Visibility = Visibility.Collapsed;
    }
  }
}
