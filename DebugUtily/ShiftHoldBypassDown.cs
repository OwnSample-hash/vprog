using System.Runtime.InteropServices;

namespace car.DebugUtily {

  /// <summary>
  /// Check if the shift key is held down
  /// </summary>
  public static class ShiftHoldBypassDown {

    [DllImport("User32.dll")]
    private static extern short GetAsyncKeyState(int code);

    /// <summary>
    /// Check if the shift key is held down
    /// </summary>
    /// <returns>Returns true if either of shitfs are held </returns>
    public static bool IsShiftDown() {
      return GetAsyncKeyState(0x10) != 0;
    }
  }
}
