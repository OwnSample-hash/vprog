using System.IO;
using System.Text;
using System.Windows;

namespace car;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
  public MainWindow() {
    InitializeComponent();
    var args = Environment.GetCommandLineArgs();
    if (args.Length > 1 && args.Any((e) => e == "--migrate")) {
      var conString = File.ReadAllText("connectionString.txt", Encoding.UTF8);
      var migration = new DB.MigrationManager(conString, args.Any((e) => e == "--verbose"));
      if (migration.Migrate(args.Any((e) => e == "--down"))) {
        MessageBox.Show("Migráció sikeres");
      }
      else {
        MessageBox.Show("Migráció sikertelen");
      }
    }
    if (args.Length > 1 && args.Any((e) => e == "--seed")) {
      var conString = File.ReadAllText("connectionString.txt", Encoding.UTF8);
      var migration = new DB.MigrationManager(conString, args.Any((e) => e == "--verbose"));
      if (migration.Seed()) {
        MessageBox.Show("Adatok betöltése sikeres");
      }
      else {
        MessageBox.Show("Adatok betöltése sikertelen");
      }
    }
    //new Login().Show();
  }
}
