using System.ComponentModel;
using car.Session;

namespace car.AdminTool {
  public class UserViewModel : INotifyPropertyChanged {

    public bool Modified { get; set; } = false;

    public User User { get; set; } = User.getEmpty();

    public bool IsReadOnly {
      get => User.Id == 0;
      set {
        OnPropertyChanged(nameof(IsReadOnly));
      }
    }

    public bool IsEditable { get => !IsReadOnly; }

    public ESessionType SelectedPerm {
      get => User.Permission;
      set {
        if (IsReadOnly || Session.Session.User.Id == User.Id || value == ESessionType.System) {
          OnPropertyChanged(nameof(User.Permission));
          return;
        }
        MainWindow.Logger.Log($"User {Session.Session.User.Username} changed {User.Username}'s permission from {User.Permission} to {value}");
        User.Permission = value;
        OnPropertyChanged(nameof(User.Permission));
        Modified = true;
      }
    }

    public string Username {
      get => User.Username;
      set {
        if (IsReadOnly) {
          OnPropertyChanged(nameof(User.Username));
          return;
        }
        MainWindow.Logger.Log($"User {Session.Session.User.Username} changed {User.Username}'s username to {value}");
        User.Username = value;
        OnPropertyChanged(nameof(User.Username));
        Modified = true;
      }
    }

    public decimal Balance {
      get => User.Balance;
      set {
        if (IsReadOnly) {
          OnPropertyChanged(nameof(User.Balance));
          return;
        }
        MainWindow.Logger.Log($"User {Session.Session.User.Username} changed {User.Username}'s balance to {value}");
        User.Balance = value;
        OnPropertyChanged(nameof(User.Balance));
        Modified = true;
      }
    }

    public UserViewModel(User user, bool IsReadOnly = false) {
      User = user;
      this.IsReadOnly = IsReadOnly;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string name) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}
