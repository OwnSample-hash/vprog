using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Imaging;

namespace car.Models {
  public class Car : INotifyPropertyChanged {
    [Key]
    int _Id { get; set; } = 0;
    public int Id {
      get => _Id;
      set {
        _Id = value;
        OnPropertyChanged(nameof(Id));
      }
    }
    string _Name { get; set; } = "";
    public string Name {
      get => _Name;
      set {
        _Name = value;
        OnPropertyChanged(nameof(Name));
      }
    }
    int _SellerId { get; set; } = 0;
    public int SellerId {
      get => _SellerId;
      set {
        _SellerId = value;
        OnPropertyChanged(nameof(SellerId));
      }
    }
    decimal _Price { get; set; } = 0;
    public decimal Price {
      get => _Price;
      set {
        _Price = value;
        OnPropertyChanged(nameof(Price));
      }
    }
    string _Description { get; set; } = "";
    public string Description {
      get => _Description;
      set {
        _Description = value;
        OnPropertyChanged(nameof(Description));
      }
    }
    public List<BitmapImage> _Pics { get; set; } = [];
    public List<BitmapImage> Pics {
      get => _Pics;
      set {
        _Pics = value;
        OnPropertyChanged(nameof(Pics));
      }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string propertyName) {
      MainWindow.Logger.SysLog($"Property changed: {propertyName}", Logging.ELogLvl.TRACE);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
