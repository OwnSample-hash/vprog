using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using car.Session;

namespace car {

  [Table("Users")]
  class User {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = 0;
    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public ESessionType Permission { get; private set; } = ESessionType.User;
    public int PermissionId {
      get => (int)this.Permission;
      set => this.Permission = (ESessionType)value;
    }

    public static User getEmpty() {
      return new User {
        Id = -1,
        Username = "",
        Password = "",
        Permission = ESessionType.None
      };
    }
  }
}
