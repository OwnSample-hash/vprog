using System.Runtime.CompilerServices;

namespace car.Logging {
  public interface ILogger {
    void Log(string message, ELogLvl level = ELogLvl.INFO,
      [CallerFilePath] string filePath = "",
      [CallerLineNumber] int lineNumber = 0);

    void Log(Message message);
  }
}
