using System.Windows;
using System.Windows.Controls;

namespace car.Pages.Session {
  public partial class Session : Page {


    public static User User { get; private set; } = User.getEmpty();

    private ESessionAuthError _eSessionAuthError = ESessionAuthError.OK;

    public delegate void LoginEventHandler();

    public static event LoginEventHandler? LoginEvent;

    public static event LoginEventHandler? LogoutEvent;

    public void Login(object sender, RoutedEventArgs e) {
      MainWindow.MainPage.NavigationService?.Navigate(new Login.Login((user, success) => {
        if (success) {
          User = user;
          MainWindow.Logger.Log($"User {user.Username} logged in");
          btLogin.Visibility = Visibility.Collapsed;
          btLogout.Visibility = Visibility.Visible;
          LoginEvent?.Invoke();
          MainWindow.MainPage.NavigationService?.GoBack();
          MainWindow.MainPage.NavigationService?.RemoveBackEntry();
        } else {
          MainWindow.Logger.Log("Login failed");
          _eSessionAuthError = ESessionAuthError.InvalidCredentials;
        }
      }));
    }

    public void Logout(object sender, RoutedEventArgs e) {
      MainWindow.Logger.Log("User logged out");
      User = User.getEmpty();
      MainWindow.Logger.SysLog("Calling events", Logging.ELogLvl.TRACE);
      LogoutEvent?.Invoke();
      btLogin.Visibility = Visibility.Visible;
      btLogout.Visibility = Visibility.Collapsed;
    }
  }
}
