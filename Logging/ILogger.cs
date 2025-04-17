using System.Runtime.CompilerServices;

namespace car.Logging {
  public interface ILogger : IDisposable {

    ELogLvl LogLevel { get; set; }

    void Log(string message, ELogLvl level = ELogLvl.INFO,
      [CallerFilePath] string filePath = "",
      [CallerLineNumber] int lineNumber = 0);

    void Log(Message message);

    void SysLog(string message, ELogLvl level = ELogLvl.INFO,
      [CallerFilePath] string filePath = "",
      [CallerLineNumber] int lineNumber = 0);

    void SysLog(Message message);
  }
}
