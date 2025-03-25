using System.ComponentModel.DataAnnotations;

namespace car.AdminTool {
  class LogsView {

    [Key]
    public int Id { get; set; } = 0;

    public string User { get; set; } = "";

    public string Description { get; set; } = "";

    public string Level { get; set; } = "";

    public string Source { get; set; } = "";

    public int Line { get; set; } = 0;

    public string SL { get => $"{Source}:{Line}"; }

    public DateTime Date { get; set; } = DateTime.Now;
  }
}
