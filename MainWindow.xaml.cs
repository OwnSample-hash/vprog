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

  public static string conString = File.ReadAllText("connectionString.txt", Encoding.UTF8);

  public static ILogger Logger = new DBLogging();

  public static bool Verbose = Environment.GetCommandLineArgs().Any((e) => e == "--verbose");
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
        Logger.Log("Force skipping down", ELogLvl.DEBUG);
      }
    }
    InitializeComponent();
    var args = Environment.GetCommandLineArgs();
    var migration = new DB.MigrationManager(conString, Verbose);
    if (args.Length > 1 && args.Any((e) => e == "--migrate")) {
      if (migration.Migrate(!SkipDown && args.Any((e) => e == "--down"))) {
        Logger.Log("Migration completed!");
      } else {
        Logger.Log("Migration failed!", ELogLvl.ERROR);
      }
    }
    if (args.Length > 1 && args.Any((e) => e == "--seed")) {
      var conString = File.ReadAllText("connectionString.txt", Encoding.UTF8);
      if (migration.Seed()) {
        Logger.Log("Seed completed!");
      } else {
        Logger.Log("Seed failed!", ELogLvl.ERROR);
      }
    }
  }
  ~MainWindow() {
    if (Verbose) {
      DebugUtily.AllocConsoleOnVerbose.DoFreeConsole();
    }
  }
}
