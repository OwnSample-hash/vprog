using System.Collections.ObjectModel;
using System.Windows;

namespace car.AdminTool {
  public class LogViewModel {

    public ObservableCollection<LogsView> Logs { get; set; } = [];

    public void AddLog(LogsView log) {
      Application.Current.Dispatcher.Invoke(() => {
        Logs.Add(log);
      });
    }
  }
}
