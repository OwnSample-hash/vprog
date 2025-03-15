using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace car {
  enum EPermission {
    User = 0,
    Seller = 1,
    Admin = 2
  }

  [Table("Users")]
  class User {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = 0;
    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    private EPermission _Permission { get; set; } = EPermission.User;
    public int PermissionId {
      get => (int)this._Permission;
      set => this._Permission = (EPermission)value;
    }
  }
}
