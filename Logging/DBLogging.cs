using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;

namespace car.Logging {
  class DBLogging : ILogger {

    public bool Verbose { get; set; }

    public ELogLvl LogLevel { get; set; } = ELogLvl.INFO;

    public bool Ready { get; private set; }

    private readonly TimeSpan delay = TimeSpan.FromSeconds(5);

    private readonly SqlConnection _connection = new(MainWindow.conString);

    private readonly Queue<Message> _messagesBackLog = [];

    private readonly Thread senderThread;
    public DBLogging() {
      try {
        _connection.Open();
        Ready = true;
      } catch (Exception e) {
        Ready = false;
        Console.WriteLine(e.Message);
        Log(new Message(e.Message, ELogLvl.ERROR));
      }
      senderThread = new Thread(() => {
        while (true) {
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
          OnTick();
          var elapsed = DateTime.Now.Nanosecond - start;
          Thread.Sleep((int)(elapsed > delay.TotalMilliseconds ? 0 : delay.TotalMilliseconds - elapsed));
        }
      });
      senderThread.Name = "DBLogging";
      senderThread.Start();
    }

    ~DBLogging() {
      _connection.Close();
    }

    public void Log(string message, ELogLvl eLogLvl = ELogLvl.INFO,
      [CallerFilePath] string filePath = "",
      [CallerLineNumber] int lineNumber = 0) {
      if (eLogLvl < LogLevel)
        return;
      _messagesBackLog.Enqueue(new Message(message, eLogLvl, filePath, lineNumber));
    }

    public void Log(Message message) {
      if (message.Level < LogLevel)
        return;
      _messagesBackLog.Enqueue(message);
    }

    private void OnTick() {
      lock (_messagesBackLog)
        while (_messagesBackLog.Count > 0)
          if (_messagesBackLog.TryDequeue(out var message)) {
            var cmd = new SqlCommand("INSERT INTO Logs (Description, LevelId, Source, Line, Date) VALUES (@Description, @LevelId, @Source, @Line, @Date)", _connection);
            cmd.Parameters.AddWithValue("@Description", message.Description);
            cmd.Parameters.AddWithValue("@LevelId", message.LevelId);
            cmd.Parameters.AddWithValue("@Source", message.Source);
            cmd.Parameters.AddWithValue("@Line", message.Line);
            cmd.Parameters.AddWithValue("@Date", message.TimeStamp);
            cmd.ExecuteNonQuery();
          }
    }
  }
}
