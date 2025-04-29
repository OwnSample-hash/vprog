using System.ComponentModel;
using System.Windows;
using car.Models;
using car.Pages.Session;

namespace car.Pages.Main {
  public class MainWindowDataContext {

    public IsAdminVis AdminVisibility { get; } = new();

    public IsSellerVis SellerVisiblity { get; } = new();

    public CarNPC cars { get; set; } = new();
  }

  public class CarNPC : INotifyPropertyChanged {

    List<Car> _cars = new();

    public List<Car> cars {
      get => _cars; set {
        _cars = value;
        OnPropertyChanged(nameof(cars));
      }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string propertyName) {
      MainWindow.Logger.SysLog($"Property changed: {propertyName}", Logging.ELogLvl.TRACE);
      MainWindow.Logger.SysLog($"Callbacks: {PropertyChanged?.GetInvocationList().Length}", Logging.ELogLvl.TRACE);
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
}
