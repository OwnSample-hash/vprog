using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace car.Logging {

  [Table("Logs")]
  public class Message(string description, ELogLvl level, int UserId = 0,
    [CallerFilePath] string source = "",
    [CallerLineNumber] int line = 0) {

    [Key]
    public int Id { get; set; } = 0;

    public int UserId { get; set; } = UserId;

    public string Description { get; set; } = description;

    public ELogLvl Level { get; set; } = level;

    public int LevelId { get => (int)Level; set => Level = (ELogLvl)value; }

    public string Source { get; set; } = source;

    public int Line { get; set; } = line;

    public DateTime TimeStamp { get; set; } = DateTime.Now;

    public override string ToString() {
      return $"[{TimeStamp}] {Level} {Description} by {UserId} at {Source}:{Line}";
    }
  }
}
