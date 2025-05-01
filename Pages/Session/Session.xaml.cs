using System.Windows.Controls;
using static car.User;

namespace car.Pages.Session {
  /// <summary>
  /// Interaction logic for Session.xaml
  /// </summary>
  public partial class Session : Page {

    Main.MainWindowDataContext MWDC { get; set; }

    UserBalChangeEventHandler UserBalChangeEventHandler { get; set; }

    public Session(Main.MainWindowDataContext MWDC, UserBalChangeEventHandler handler) {
      InitializeComponent();
      this.MWDC = MWDC;
      UserBalChangeEventHandler = handler;
      DataContext = MWDC;
    }
  }
}
