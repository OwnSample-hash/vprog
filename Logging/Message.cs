using System.Runtime.CompilerServices;

namespace car.Logging {
  public class Message(string description, ELogLvl level,
    [CallerFilePath] string source = "",
    [CallerLineNumber] int line = 0) {

    public int Id { get; set; } = 0;

    public string Description { get; set; } = description;

    public ELogLvl Level { get; set; } = level;

    public int LevelId { get => (int)Level; set => Level = (ELogLvl)value; }

    public string Source { get; set; } = source;

    public int Line { get; set; } = line;

    public DateTime TimeStamp { get; set; } = DateTime.Now;
  }
}
