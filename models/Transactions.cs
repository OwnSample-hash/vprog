namespace car.Models {
  internal class Transaction {
    public int Id { get; set; } = 0;
    public decimal Amount { get; set; } = 0;
    public DateTime Date { get; set; } = DateTime.Now;
    public int AccountId { get; set; } = 0;
    public int CarId { get; set; } = 0;
    public int SellerId { get; set; } = 0;
    public override string ToString() {
      return $"{Id} {Amount} {Date} {AccountId} {CarId} {SellerId}";
    }
  }
}
