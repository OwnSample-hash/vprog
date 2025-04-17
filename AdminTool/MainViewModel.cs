using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;

namespace car.AdminTool {
  public class MainViewModel {
    public LogViewModel LogViewModel { get; set; }

    public ObservableCollection<UserViewModel> Users { get; set; } = [];

    public static Array Permissions { get => Enum.GetValues(typeof(Session.ESessionType)); }

    public static Array LogLevels { get => Enum.GetValues(typeof(Logging.ELogLvl)); }

    public static Logging.ELogLvl ELogLvl { get => MainWindow.Logger.LogLevel; set => MainWindow.Logger.LogLevel = value; }

    public bool ShouldRefresh { get => Users.Any((e) => e.Modified); }

    public MainViewModel() {
      LogViewModel = new();
    }

    public void AddUser(User user) {
      Users.Add(new(user, user.Id == 0));
    }

    private ActionCommand quitCommand;
    public ICommand QuitCommand => quitCommand ??= new ActionCommand(Quit);

    private void Quit() {
      App.Current.Shutdown();
    }
  }
}
