﻿using car.Pages.Session;

namespace car {

  public class User {

    public int Id { get; set; } = 0;

    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public ESessionType Permission { get; set; } = ESessionType.User;

    public int PermissionId {
      get => (int)this.Permission;
      set => this.Permission = (ESessionType)value;
    }

    decimal _Balance { get; set; } = 0;

    public decimal Balance {
      get => _Balance;
      set {
        _Balance = value;
        UserBalChangeEvent?.Invoke(this);
      }
    }

    public override string ToString() {
      return $"{Id} {Username} {Permission} {PermissionId} {Balance}";
    }

    public static User getEmpty() {
      return new User {
        Id = -1,
        Username = "",
        Password = "",
        Permission = ESessionType.None
      };
    }

    public static User getSystem() {
      return new User {
        Id = 0,
        Username = "System",
        Password = "",
        Permission = ESessionType.System
      };
    }

    public static bool IsUserValid(User? user) =>
      user != null &&
      user.Id > 0 &&
      user.Username != "" &&
      user.Password != "" &&
      user.Permission != ESessionType.None;

    public delegate void UserBalChangeEventHandler(User user);

    public event UserBalChangeEventHandler? UserBalChangeEvent;
  }
}
