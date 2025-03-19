using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;
using Z.Dapper.Plus;

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
        if (!Ready) {
          try {
            _connection.Open();
            Ready = true;
          } catch (Exception e) {
            Ready = false;
            Console.WriteLine(e.Message);
            Log(new Message(e.Message, ELogLvl.ERROR));
          }
          while (true) {
            var start = DateTime.Now.Nanosecond;
            OnTick();
            var elapsed = DateTime.Now.Nanosecond - start;
            Thread.Sleep((int)(elapsed > delay.TotalMilliseconds ? 0 : delay.TotalMilliseconds - elapsed));
          }
        }
      });
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
      lock (_messagesBackLog) {
        List<Message> messages = [];
        while (_messagesBackLog.Count > 0)
          if (_messagesBackLog.TryDequeue(out var message))
            messages.Add(message);
        _connection.BulkInsert(messages);
      }
    }
  }
}
