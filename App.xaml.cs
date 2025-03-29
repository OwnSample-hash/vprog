using System.Windows;
using Microsoft.Extensions.Configuration;

namespace car;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
  public IConfiguration Configuration { get; private set; } = null!;

  public static SettingsBind Conf { get; private set; } = null!;

  protected override void OnStartup(StartupEventArgs e) {
    base.OnStartup(e);
    var builder = new ConfigurationBuilder()
      .SetBasePath(System.IO.Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

    Configuration = builder.Build();

    Conf = new SettingsBind();
    Configuration.GetSection("AppSettings").Bind(Conf);
  }
}

