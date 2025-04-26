using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace car.Picture {
  public class CacheManager : IDisposable {

    List<Cache> Pics { get; set; } = [];

    bool _disposed { get; set; } = false;

    string _path { get; set; }

    string _cache { get => Path.Combine(_path, "cache.json"); }

    private Thread _thread { get; set; }

    private readonly SqlConnection _connection = new(MainWindow.conString);

    private readonly TimeSpan delay = TimeSpan.FromSeconds(15);

    public bool ShouldCache { get; set; } = true;

    public CacheManager(string path) {
      _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
      MainWindow.Logger.SysLog($"Current Path is {_path}", Logging.ELogLvl.TRACE);
      if (!Directory.Exists(_path)) {
        Directory.CreateDirectory(_path);
      }
      if (!File.Exists(_cache)) {
        File.WriteAllText(_cache, JsonConvert.SerializeObject(Pics));
      }
      var pics = JsonConvert.DeserializeObject<List<Cache>>(File.ReadAllText(_cache));
      if (pics != null) {
        Pics = pics;
      }
      _thread = new(Thread) {
        Name = "Picture Caching"
      };
      MainWindow.Logger.SysLog("Starting Picture Caching");
      _thread.Start();
    }

    void Thread() {
      List<Task<Cache>> tasks = [];
      while (!_disposed) {
        var start = DateTime.Now.Nanosecond;
        var PictureData = _connection.Query<Picture>("SELECT * FROM Pictures");
        foreach (var p in PictureData) {
          var CacheEntry = Pics.FirstOrDefault((e) => e.Pic.Id == p.Id);
          if (CacheEntry == null) {
            CacheEntry = new Cache() {
              Path = Path.Combine(_path, Uri.EscapeDataString(p.Url.Split("/").Last())),
              IsCached = false,
              LastUpdated = DateTime.Now,
              Pic = p
            };
            Pics.Add(CacheEntry);
          }
        }
        if (!ShouldCache) {
          double remainingMilliseconds2 = delay.TotalMilliseconds - ((DateTime.Now.Nanosecond - start) / 1_000_000.0);
          System.Threading.Thread.Sleep(remainingMilliseconds2 > 0 ? (int)remainingMilliseconds2 : 0);
          continue;
        }
        foreach (var CacheEntry in Pics) {
          if (CacheEntry.IsCached) {
            continue;
          }
          tasks.Add(DownloadImage(CacheEntry));
        }
        while (tasks.Count > 0) {
          var task = Task.WhenAny(tasks);
          tasks.Remove(task.Result);
          if (task.Result.Result.IsDownloaded) {
            var CacheEntry = Pics.FirstOrDefault((e) => e.Path == task.Result.Result.Path);
            if (CacheEntry != null) {
              CacheEntry.IsCached = true;
              CacheEntry.LastUpdated = DateTime.Now;
            }
          }
        }
        File.WriteAllText(_cache, JsonConvert.SerializeObject(Pics));
        MainWindow.Logger.SysLog($"Cached {Pics.Count((e) => e.IsCached)} pictures out of {Pics.Count}", Logging.ELogLvl.TRACE);
        MainWindow.Logger.SysLog($"Sleeping for {delay.TotalMilliseconds} milliseconds", Logging.ELogLvl.TRACE);
        ShouldCache = !Pics.All((e) => e.IsCached);
        var elapsed = DateTime.Now.Nanosecond - start;
        double remainingMilliseconds = delay.TotalMilliseconds - (elapsed / 1_000_000.0);
        System.Threading.Thread.Sleep(remainingMilliseconds > 0 ? (int)remainingMilliseconds : 0);
      }
    }

    private async Task<Cache> DownloadImage(Cache cache) {
      MainWindow.Logger.SysLog($"Downloading {cache.Pic.Url} to {cache.Path}", Logging.ELogLvl.DEBUG);
      using var client = new HttpClient();
      client.Timeout = TimeSpan.FromSeconds(5);
      client.DefaultRequestHeaders.Add("User-Agent", "CarManager/0.0 (ownsample@tutanota.com)");
      var response = await client.GetAsync(cache.Pic.Url);
      if (response.IsSuccessStatusCode) {
        using var stream = await response.Content.ReadAsStreamAsync();
        using var fileStream = new FileStream(cache.Path, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fileStream);
        cache.IsDownloaded = true;
        return cache;
      } else {
        MainWindow.Logger.SysLog($"Failed to download {cache.Pic.Url}", Logging.ELogLvl.ERROR);
        MainWindow.Logger.SysLog($"Status Code: {response.StatusCode}", Logging.ELogLvl.ERROR);
        MainWindow.Logger.SysLog($"Reason: {response.ReasonPhrase}", Logging.ELogLvl.ERROR);
        cache.IsDownloaded = false;
        cache.IsCached = false;
        cache.LastUpdated = DateTime.Now;
        return cache;
      }
    }

    public List<BitmapImage> GetPicsByCarId(int CarId) {
      var pics = Pics.Where((e) => e.Pic.CarId == CarId && e.IsCached).Select((e) => {
        var image = new BitmapImage();
        image.BeginInit();
        image.UriSource = new Uri(e.Path);
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        image.Freeze();
        return image;
      }).ToList();
      return pics;
    }

    public void Dispose() {
      if (_disposed) {
        return;
      }
      _disposed = true;
      _thread.Join();
      File.WriteAllText(_cache, JsonConvert.SerializeObject(Pics));
    }
  }
}
