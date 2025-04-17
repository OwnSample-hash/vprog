namespace car.Picture {
  public class Cache {
    public string Path { get; set; } = "";

    public bool IsCached { get; set; } = false;

    public bool IsDownloaded { get; set; } = false;

    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public Picture Pic { get; set; } = new();
  }
}
