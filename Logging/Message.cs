using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace car.Logging {

  [Table("Logs")]
  public class Message(string description, ELogLvl level,
    [CallerFilePath] string source = "",
    [CallerLineNumber] int line = 0) {

    [Key]
    public int Id { get; set; } = 0;

    public string Description { get; set; } = description;

    public ELogLvl Level { get; set; } = level;

    public int LevelId { get => (int)Level; set => Level = (ELogLvl)value; }

    public string Source { get; set; } = source;

    public int Line { get; set; } = line;

    public DateTime TimeStamp { get; set; } = DateTime.Now;
  }
}
