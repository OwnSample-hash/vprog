using System.Windows;
using car.DebugUtily;
using car.Logging;
using car.Picture;

namespace car;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {

  public static string conString = App.Conf.ConnectionString;

  public static ILogger Logger = new DBLogging();

  public static bool Verbose = Environment.GetCommandLineArgs().Any((e) => e == "--verbose") || App.Conf.Verbose;

  public static CacheManager CM = new("Picture");

  public MainWindow() {
    bool SkipDown = false;
    if (Verbose) {
      Logger.LogLevel = ELogLvl.TRACE;
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
    MainWindowDataContext mainWindowDataContext = new();
    DataContext = mainWindowDataContext;

    Pages.Session.Session.LoginEvent += () => {
      Logger.SysLog("Login event triggered", ELogLvl.TRACE);
      mainWindowDataContext.AdminVisibility.OnPropertyChanged("AdminVisibility");
      mainWindowDataContext.SellerVisiblity.OnPropertyChanged("SellerVisibility");
    };

    Pages.Session.Session.LogoutEvent += () => {
      Logger.SysLog("Logout event triggered", ELogLvl.TRACE);
      mainWindowDataContext.AdminVisibility.OnPropertyChanged("AdminVisibility");
      mainWindowDataContext.SellerVisiblity.OnPropertyChanged("SellerVisibility");
    };

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
      if (migration.Seed()) {
        Logger.SysLog("Seed completed!");
      } else {
        Logger.SysLog("Seed failed!", ELogLvl.ERROR);
      }
    }
    this.Closed += (_, _) => {
      Logger.SysLog("Closing MainWindow", ELogLvl.DEBUG);
      if (Verbose) {
        AllocConsoleOnVerbose.DoFreeConsole();
      }
      Logger.Dispose();
      CM.Dispose();
      Environment.Exit(0);
    };
  }
}
