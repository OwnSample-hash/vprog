using System.Runtime.InteropServices;

namespace car.DebugUtily {
  /// <summary>
  /// Allocate Console If Verbose argument is passed
  /// </summary>
  public static class AllocConsoleOnVerbose {

    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();

    /// <summary>
    /// Allocates The Console and redirects the output to it
    /// </summary>
    public static void DoAllocConsoleOnVerbose() {
      AllocConsole();
      Console.SetOut(new System.IO.StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
    }

    /// <summary>
    /// Frees the console
    /// </summary>
    public static void DoFreeConsole() {
      FreeConsole();
    }
  }
}
