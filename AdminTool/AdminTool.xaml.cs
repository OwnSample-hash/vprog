using System.Windows;
using Dapper;
using Microsoft.Data.SqlClient;

namespace car.AdminTool {
  /// <summary>
  /// Interaction logic for AdminTool.xaml
  /// </summary>
  public partial class AdminTool : Window {

    readonly Task LogPoller;

    private int LastID = -1;

    private SqlConnection SqlConnection = new(MainWindow.conString);

    public AdminTool() {
      InitializeComponent();
      SqlConnection.Open();
      LogViewModel logViewModel = new();
      DataContext = logViewModel;

      if (MainWindow.Verbose) {
        Source.Visibility = Visibility.Visible;
      }

      LogPoller = Task.Run(async () => {
        while (true) {
          var logs = SqlConnection.Query<LogsView>("SELECT * FROM LogsView WHERE Id > @LastID", new { LastID });
          foreach (var log in logs) {
            logViewModel.AddLog(log);
            LastID = log.Id;
          }
          await Task.Delay(1000);
        }
      });
    }

    ~AdminTool() {
      LogPoller.Dispose();
      SqlConnection.Close();
    }
  }
}
