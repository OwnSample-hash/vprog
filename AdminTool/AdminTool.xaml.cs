using System.Diagnostics;
using System.Windows;
using car.Logging;
using Dapper;
using Microsoft.Data.SqlClient;
using Z.Dapper.Plus;

namespace car.AdminTool {
  /// <summary>
  /// Interaction logic for AdminTool.xaml
  /// </summary>
  public partial class AdminTool : Window, IDisposable {

    private bool _disposed = false;

    readonly Task LogPoller;

    private int LastID = -1;

    private SqlConnection SqlConnection = new(MainWindow.conString);

    public bool IsOpen => !_disposed && Visibility == Visibility.Visible;

    public AdminTool() {
      InitializeComponent();
      SqlConnection.Open();
      MainViewModel MainViewModel = new();
      DataContext = MainViewModel;

      if (MainWindow.Verbose) {
        Source.Visibility = Visibility.Visible;
      }
      bool firstRun = true;
      LogPoller = Task.Run(async () => {
        while (!_disposed) {
          var logs = SqlConnection.Query<LogsView>("SELECT * FROM LogsView WHERE Id > @LastID", new { LastID });
          foreach (var log in logs) {
            MainViewModel.LogViewModel.AddLog(log);
            LastID = log.Id;
          }
          if (firstRun) {
            var users = SqlConnection.Query<User>("SELECT * FROM Users");
            try {
              Application.Current.Dispatcher.Invoke(() => users.AsEnumerable().ToList().ForEach((u) => MainViewModel.AddUser(u)));
            } catch (Exception e) {
              Console.WriteLine(e);
              Debugger.Break();
            }
            firstRun = false;
            await Task.Delay(1000);
            continue;
          }
          if (MainViewModel.ShouldRefresh) {
            Console.WriteLine("Refreshing data");
            SqlConnection.BulkUpdate(MainViewModel.Users.Select((e) => e.User));
            var users = SqlConnection.Query<User>("SELECT * FROM Users");
            Application.Current.Dispatcher.Invoke(() => MainViewModel.Users.Clear());
            try {
              Application.Current.Dispatcher.Invoke(() => users.AsEnumerable().ToList().ForEach((u) => MainViewModel.AddUser(u)));
            } catch (Exception e) {
              Console.WriteLine(e);
              Debugger.Break();
            }
            firstRun = false;
          }
          await Task.Delay(1000);
        }
      });
      this.Closed += (_, _) => Dispose();
    }

    public void Dispose() {
      if (_disposed)
        return;
      MainWindow.Logger.SysLog("Disposing AdminTool", ELogLvl.DEBUG);
      _disposed = true;
      LogPoller.Wait();
      LogPoller.Dispose();
      SqlConnection.Close();
      SqlConnection.Dispose();
      GC.SuppressFinalize(this);
    }

    ~AdminTool() {
      Dispose();
    }
  }
}
