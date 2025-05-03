using System.Windows;
using System.Windows.Controls;

namespace car.Pages.Session {
  public partial class Session : Page {

    public static User User { get; private set; } = User.getEmpty();

    public delegate void LoginEventHandler();

    public static event LoginEventHandler? LoginEvent;

    public static event LoginEventHandler? LogoutEvent;

    public void Login(object sender, RoutedEventArgs e) {
      MainWindow.MainPage.NavigationService?.Navigate(new Login.Login((user, success) => {
        if (success) {
          User = user;
          user.UserBalChangeEvent += UserBalChangeEventHandler;
          MainWindow.Logger.Log($"User {user.Username} logged in");
          btLogin.Visibility = Visibility.Collapsed;
          btLogout.Visibility = Visibility.Visible;
          MWDC.Status.Status = $"Bejelentkezve {user.Username}\nEgyenleg: {user.Balance}";
          LoginEvent?.Invoke();
          MainWindow.MainPage.NavigationService?.GoBack();
          MainWindow.MainPage.NavigationService?.RemoveBackEntry();
        } else {
          MainWindow.Logger.Log("Login failed");
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
      MWDC.Status.Status = "Kérlek jelentkezz be!";
    }
  }
}
