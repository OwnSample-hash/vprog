using System.ComponentModel;
using System.Windows.Media.Imaging;
using car.Models;

namespace car.Pages.Main {
  public class CarDetailsDC : INotifyPropertyChanged {
    public CarDetailsDC() {
      _Id = 0;
      _Image1 = new BitmapImage();
      _Image2 = new BitmapImage();
      _Price = 0;
      _Name = "";
      _Description = "";
    }

    public CarDetailsDC(Car car) {
      _Id = car.Id;
      _Image1 = car.Pics[0];
      _Image2 = car.Pics[1];
      _Price = car.Price;
      _Name = car.Name;
      _Description = car.Description;

    }
    int _Id { get; set; }
    public int Id {
      get => _Id;
      set {
        _Id = value;
        OnPropertyChanged(nameof(Id));
      }
    }

    BitmapImage _Image1 { get; set; }

    public BitmapImage Image1 {
      get => _Image1;
      set {
        _Image1 = value;
        OnPropertyChanged(nameof(Image1));
      }
    }

    BitmapImage _Image2 { get; set; }
    public BitmapImage Image2 {
      get => _Image2;
      set {
        _Image2 = value;
        OnPropertyChanged(nameof(Image2));
      }
    }

    float _Price { get; set; }
    public float Price {
      get => _Price;
      set {
        _Price = value;
        OnPropertyChanged(nameof(Price));
      }
    }

    string _Name { get; set; }
    public string Name {
      get => _Name;
      set {
        _Name = value;
        OnPropertyChanged(nameof(Name));
      }
    }

    string _Description { get; set; }
    public string Description {
      get => _Description;
      set {
        _Description = value;
        OnPropertyChanged(nameof(Description));
      }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string propertyName) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
