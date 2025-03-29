using System.IO;
using System.Text;
using System.Windows;
using car.DebugUtily;
using car.Logging;

namespace car;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {

  public static string conString = App.Conf.ConnectionString;

  public static ILogger Logger = new DBLogging();

  public static bool Verbose = Environment.GetCommandLineArgs().Any((e) => e == "--verbose") || App.Conf.Verbose;

  public MainWindow() {
    bool SkipDown = false;
    if (Verbose) {
      AllocConsoleOnVerbose.DoAllocConsoleOnVerbose();
      int tries = 0;
      while (!ShiftHoldBypassDown.IsShiftDown() && tries != 10) {
        tries++;
        Thread.Sleep(50);
      }
      if (tries < 10) {
        SkipDown = true;
        Console.WriteLine("Force skipping down");
        Logger.SysLog("Force skipping down", ELogLvl.DEBUG);
      }
    }
    InitializeComponent();
    var args = Environment.GetCommandLineArgs();
    var migration = new DB.MigrationManager(conString, Verbose);
    if (args.Length > 1 && args.Any((e) => e == "--migrate")) {
      if (migration.Migrate(!SkipDown && args.Any((e) => e == "--down"))) {
        Logger.SysLog("Migration completed!");
      } else {
        Logger.SysLog("Migration failed!", ELogLvl.ERROR);
      }
    }
    if (args.Length > 1 && args.Any((e) => e == "--seed")) {
      var conString = File.ReadAllText("connectionString.txt", Encoding.UTF8);
      if (migration.Seed()) {
        Logger.SysLog("Seed completed!");
      } else {
        Logger.SysLog("Seed failed!", ELogLvl.ERROR);
      }
    }
    this.Closed += (_, _) => {
      Logger.SysLog("Closing MainWindow", ELogLvl.DEBUG);
      if (Verbose) {
        DebugUtily.AllocConsoleOnVerbose.DoFreeConsole();
      }
      Logger.Dispose();
    };
  }
}
