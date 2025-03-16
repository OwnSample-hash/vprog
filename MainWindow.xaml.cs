using System.IO;
using System.Text;
using System.Windows;

namespace car;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {

  public static string conString = File.ReadAllText("connectionString.txt", Encoding.UTF8);

  public static bool verbose = Environment.GetCommandLineArgs().Any((e) => e == "--verbose");
  public MainWindow() {
    InitializeComponent();
    var args = Environment.GetCommandLineArgs();
    if (args.Length > 1 && args.Any((e) => e == "--migrate")) {
      var migration = new DB.MigrationManager(conString, verbose);
      if (migration.Migrate(args.Any((e) => e == "--down"))) {
        //MessageBox.Show("Migráció sikeres");
      } else {
        MessageBox.Show("Migráció sikertelen");
      }
    }
    if (args.Length > 1 && args.Any((e) => e == "--seed")) {
      var conString = File.ReadAllText("connectionString.txt", Encoding.UTF8);
      var migration = new DB.MigrationManager(conString, verbose);
      if (migration.Seed()) {
      } else {
        MessageBox.Show("Adatok betöltése sikertelen");
      }
    }
  }
}
