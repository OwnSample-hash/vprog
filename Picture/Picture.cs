using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace car.Picture {
  public class Picture {

    [Key]
    public int Id { get; set; } = 0;

    public int CarId { get; set; } = 0;

    public string Url { get; set; } = "";

    [JsonIgnore]
    [NotMapped]
    public BitmapImage Image { get; set; } = null!;
  }
}
