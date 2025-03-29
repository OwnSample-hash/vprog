using System.Collections.ObjectModel;

namespace car.AdminTool {
  public class MainViewModel {
    public LogViewModel LogViewModel { get; set; }

    public ObservableCollection<UserViewModel> Users { get; set; } = [];

    public static Array Permissions { get => Enum.GetValues(typeof(Session.ESessionType)); }

    public bool ShouldRefresh { get => Users.Any((e) => e.Modified); }

    public MainViewModel() {
      LogViewModel = new();
    }

    public void AddUser(User user) {
      Users.Add(new(user, user.Id == 0));
    }
  }
}
