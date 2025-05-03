using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using car.Models;
using car.Pages.Session;

namespace car.Pages.Main {
  public class MainWindowDataContext {

    public IsAdminVis AdminVisibility { get; set; } = new();

    public IsSellerVis SellerVisiblity { get; set; } = new();

    public IsUserVis UserVisibility { get; set; } = new();

    public StatusMsg Status { get; set; } = new();

    public ObservableCollection<Car> cars { get; set; } = [];
  }

  public class StatusMsg : INotifyPropertyChanged {
    private string _status = "Kérlek jelentkezz be!";
    public string Status {
      get => _status;
      set {
        _status = value;
        OnPropertyChanged(nameof(Status));
      }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string propertyName) {
      MainWindow.Logger.SysLog($"Property changed: {propertyName}", Logging.ELogLvl.TRACE);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  public class IsAdminVis : INotifyPropertyChanged {
    private bool IsAdmin => Session.Session.User.Permission == ESessionType.Admin;
    public Visibility AdminVisibility {
      get => IsAdmin ? Visibility.Visible : Visibility.Collapsed;
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string propertyName) {
      MainWindow.Logger.SysLog($"Property changed: {propertyName}", Logging.ELogLvl.TRACE);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  public class IsSellerVis : INotifyPropertyChanged {
    private bool IsSeller => Session.Session.User.Permission == ESessionType.Seller || Session.Session.User.Permission == ESessionType.Admin;
    public Visibility SellerVisibility {
      get => IsSeller ? Visibility.Visible : Visibility.Collapsed;
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string propertyName) {
      MainWindow.Logger.SysLog($"Property changed: {propertyName}", Logging.ELogLvl.TRACE);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  public class IsUserVis : INotifyPropertyChanged {
    private bool IsUser => User.IsUserValid(Session.Session.User);
    public Visibility UserVisibility {
      get => IsUser ? Visibility.Visible : Visibility.Collapsed;
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string propertyName) {
      MainWindow.Logger.SysLog($"Property changed: {propertyName}", Logging.ELogLvl.TRACE);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
