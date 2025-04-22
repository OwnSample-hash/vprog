using System.Collections.ObjectModel;
using System.Windows.Input;
using car.Pages.Session;
using Microsoft.Xaml.Behaviors.Core;

namespace car.AdminTool {
  public class MainViewModel {
    public LogViewModel LogViewModel { get; set; }

    public ObservableCollection<UserViewModel> Users { get; set; } = [];

    public static Array Permissions { get => Enum.GetValues(typeof(ESessionType)); }

    public static Array LogLevels { get => Enum.GetValues(typeof(Logging.ELogLvl)); }

    public static Logging.ELogLvl ELogLvl { get => MainWindow.Logger.LogLevel; set => MainWindow.Logger.LogLevel = value; }

    public bool ShouldRefresh { get => Users.Any((e) => e.Modified); }

    public MainViewModel() {
      LogViewModel = new();
    }

    public void AddUser(User user) {
      Users.Add(new(user, user.Id == 0));
    }

    private ActionCommand quitCommand = new ActionCommand(Quit);
    public ICommand QuitCommand => quitCommand;

    private static void Quit() {
      App.Current.Shutdown();
    }
  }
}
