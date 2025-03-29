namespace car {
  public class SettingsBind {
    public string ConnectionString { get; set; } = "";

    public bool Verbose { get; set; } = false;

    public bool Migrate { get; set; } = false;

    public bool Seed { get; set; } = false;
  }
}
