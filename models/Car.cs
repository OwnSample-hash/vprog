using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Imaging;

namespace car.models {
  public class Car {
    [Key]
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "";
    public int SellerId { get; set; } = 0;
    public float Price { get; set; } = 0;
    public List<BitmapImage> Pics => MainWindow.CM.GetPicsByCarId(this.Id);
  }
}
