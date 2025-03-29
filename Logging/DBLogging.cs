using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;

namespace car.Logging {
  class DBLogging : ILogger {

    public ELogLvl LogLevel { get; set; } = ELogLvl.INFO;

    public bool Ready { get; private set; }

    private readonly TimeSpan delay = TimeSpan.FromSeconds(5);

    private readonly SqlConnection _connection = new(MainWindow.conString);

    private readonly Queue<Message> _messagesBackLog = [];

    private readonly Thread senderThread;

    private bool _disposed = false;
    public DBLogging() {
      try {
        _connection.Open();
        Ready = true;
      } catch (Exception e) {
        Ready = false;
        Console.WriteLine(e.Message);
        Log(new Message(e.Message, ELogLvl.ERROR));
      }
      senderThread = new(() => {
        while (!_disposed) {
          if (!Ready) {
            try {
              _connection.Open();
              Ready = true;
            } catch (Exception e) {
              Ready = false;
              Console.WriteLine(e.Message);
              Log(new Message(e.Message, ELogLvl.ERROR));
              continue;
            }
          }
          var start = DateTime.Now.Nanosecond;
          lock (_messagesBackLog)
            while (_messagesBackLog.Count > 0)
              if (_messagesBackLog.TryDequeue(out var message)) {
                var cmd = new SqlCommand("INSERT INTO Logs (Description, UserId, LevelId, Source, Line, Date) VALUES (@Description, @UserId, @LevelId, @Source, @Line, @Date)", _connection);
                cmd.Parameters.AddWithValue("@Description", message.Description);
                cmd.Parameters.AddWithValue("@UserId", message.UserId);
                cmd.Parameters.AddWithValue("@LevelId", message.LevelId);
                cmd.Parameters.AddWithValue("@Source", message.Source);
                cmd.Parameters.AddWithValue("@Line", message.Line);
                cmd.Parameters.AddWithValue("@Date", message.TimeStamp);
                cmd.ExecuteNonQuery();
              }
          var elapsed = DateTime.Now.Nanosecond - start;
          double remainingMilliseconds = delay.TotalMilliseconds - (elapsed / 1_000_000.0);
          Thread.Sleep(remainingMilliseconds > 0 ? (int)remainingMilliseconds : 0);
        }
      });
      senderThread.Name = "DBLogging";
      senderThread.Start();
    }

    public void Dispose() {
      if (_disposed)
        return;
      _disposed = true;
      senderThread.Join();
      _connection.Close();
    }

    public void Log(string message, ELogLvl eLogLvl = ELogLvl.INFO,
      [CallerFilePath] string filePath = "",
      [CallerLineNumber] int lineNumber = 0) {
      if (eLogLvl < LogLevel)
        return;
      var m = new Message(message, eLogLvl, Session.Session.User.Id, filePath, lineNumber);
      _messagesBackLog.Enqueue(m);
      if (eLogLvl == ELogLvl.TRACE && App.Conf.Verbose)
        Console.WriteLine(m);
    }

    public void Log(Message message) {
      if (message.Level < LogLevel)
        return;
      message.UserId = Session.Session.User.Id;
      _messagesBackLog.Enqueue(message);
      if (message.Level == ELogLvl.TRACE && App.Conf.Verbose)
        Console.WriteLine(message);
    }

    public void SysLog(string message, ELogLvl level = ELogLvl.INFO, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
      if (level < LogLevel)
        return;
      var m = new Message(message, level, User.getSystem().Id, filePath, lineNumber);
      _messagesBackLog.Enqueue(m);
      if (level == ELogLvl.TRACE && App.Conf.Verbose)
        Console.WriteLine(m);
    }

    public void SysLog(Message message) {
      if (message.Level < LogLevel)
        return;
      message.UserId = User.getSystem().Id;
      _messagesBackLog.Enqueue(message);
      if (message.Level == ELogLvl.TRACE && App.Conf.Verbose)
        Console.WriteLine(message);
    }
  }
}
