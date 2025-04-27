using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car.models {
   public class Car {
    [Key]
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "";
    public int SellerId { get; set; } = 0;
    public float Price { get; set; } = 0;
  }
}
