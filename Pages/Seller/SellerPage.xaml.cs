using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;

namespace car.Pages.Seller {
  /// <summary>
  /// Interaction logic for SellerPage.xaml
  /// </summary>
  public partial class SellerPage : Page {
    public SellerPage() {
      InitializeComponent();
    }

    private void miBack_Click(object sender, RoutedEventArgs e) {
      MainWindow.MainPage.NavigationService?.GoBack();
      MainWindow.MainPage.NavigationService?.RemoveBackEntry();
    }

    private async void btnConf_Click(object sender, RoutedEventArgs e) {
      if (ImagePath.Text == string.Empty || CarId.Value < 0) {
        MessageBox.Show("A kép url és/vagy a kocsi azonosítója üres/érvénteleny!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      if (MainWindow.CM.GetPicsByCarId(CarId.Value).Count >= 2) {
        MessageBox.Show("A kiválasztott kocsi már tartalmaz 2 képet!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      SqlConnection SqlConnection = new(MainWindow.conString);
      SqlConnection.Open();
      SqlCommand cmd = new("INSERT INTO Pictures (CarId, Url) VALUES (@CarId, @Url)", SqlConnection);
      cmd.Parameters.AddWithValue("@CarId", CarId.Value);
      cmd.Parameters.AddWithValue("@Url", ImagePath.Text);
      try {
        await cmd.ExecuteNonQueryAsync();
        MessageBox.Show("A kép sikeresen hozzáadva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
        MainWindow.Logger.SysLog($"Kép hozzáadva a kocsihoz: {CarId.Value}", Logging.ELogLvl.TRACE);
      } catch (SqlException ex) {
        MessageBox.Show("A kép hozzáadása sikertelen!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        MainWindow.Logger.SysLog($"Kép hozzáadása a kocsihoz: {CarId.Value} - Hiba: {ex.Message}", Logging.ELogLvl.ERROR);
      } finally {
        SqlConnection.Close();
      }
      MainWindow.CM.ShouldCache = true;
    }

    private async void btnImage_Click(object sender, RoutedEventArgs e) {
      if (ImagePath.Text == string.Empty) {
        MessageBox.Show("A kép url üres!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      MainWindow.Logger.SysLog($"Downloading picture: {ImagePath.Text}", Logging.ELogLvl.TRACE);
      MemoryStream ms = new();
      BitmapImage image = new();
      using var client = new HttpClient();
      client.Timeout = TimeSpan.FromSeconds(5);
      client.DefaultRequestHeaders.Add("User-Agent", "CarManager/0.0 (ownsample@tutanota.com)");
      var response = await client.GetAsync(ImagePath.Text);
      if (response.IsSuccessStatusCode) {
        using var stream = await response.Content.ReadAsStreamAsync();
        stream.CopyTo(ms);
        ms.Position = 0;
        image.BeginInit();
        image.StreamSource = ms;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        image.Freeze();
        Preview.Source = image;
        ms.Dispose();
        MainWindow.Logger.SysLog($"Kép letöltése sikeres: {ImagePath.Text}", Logging.ELogLvl.TRACE);
      } else {
        MessageBox.Show("A kép letöltése sikertelen!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        MainWindow.Logger.SysLog($"The picture is downloaded: {ImagePath.Text}", Logging.ELogLvl.ERROR);
        MainWindow.Logger.SysLog($"Status Code: {response.StatusCode}", Logging.ELogLvl.ERROR);
        return;
      }
    }

    private void addCar_Click(object sender, RoutedEventArgs e) {
      if (NewCarName.Text == string.Empty || NewCarPrice.Value < 0 || NewCarDesc.Text == string.Empty) {
        MessageBox.Show("A kocsi neve, ára és/vagy leírása üres/érvéntelen!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      SqlConnection SqlConnection = new(MainWindow.conString);
      SqlConnection.Open();
      SqlCommand cmd = new("INSERT INTO Cars (Name, SellerId, Price, Description) VALUES (@Name, @SellerID, @Price, @Description)", SqlConnection);
      cmd.Parameters.AddWithValue("@Name", NewCarName.Text);
      cmd.Parameters.AddWithValue("@SellerID", Session.Session.User.Id);
      cmd.Parameters.AddWithValue("@Price", NewCarPrice.Value);
      cmd.Parameters.AddWithValue("@Description", NewCarDesc.Text);
      try {
        cmd.ExecuteNonQuery();
        MessageBox.Show("A kocsi sikeresen hozzáadva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
        MainWindow.Logger.SysLog($"Kocsi hozzáadva: {NewCarName.Text}", Logging.ELogLvl.TRACE);
      } catch (SqlException ex) {
        MessageBox.Show("A kocsi hozzáadása sikertelen!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
        MainWindow.Logger.SysLog($"Kocsi hozzáadása: {NewCarName.Text} - Hiba: {ex.Message}", Logging.ELogLvl.ERROR);
      } finally {
        SqlConnection.Close();
      }
    }
  }
}
