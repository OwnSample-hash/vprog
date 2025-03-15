using System.IO;
using Dapper;
using Microsoft.Data.SqlClient;
using Z.Dapper.Plus;
using BC = BCrypt.Net.BCrypt;
namespace car.DB {
  class MigrationManager {
    private readonly string _connectionString;

    private readonly SqlConnection _connection;

    private readonly bool _verbose;
    public MigrationManager(string connectionString, bool verbose = false) {
      _connectionString = connectionString;
      _verbose = verbose;
      _connection = new SqlConnection(_connectionString);
      _connection.Open();
    }

    public bool Migrate(bool downFirst) {
      if (!Directory.Exists("Migrations")) {
        return false;
      }
      var files = Directory.GetFiles("Migrations");
      if (downFirst) {
        foreach (string file in files.Reverse().Where((e) => e.EndsWith(".down.sql"))) {
          Console.WriteLine($"Downing: {file}");
          string sql = File.ReadAllText(file);
          _connection.Execute(sql);
          if (_verbose) {
            Console.WriteLine("--");
            Console.WriteLine($"Ran sql:\n{sql}");
            Console.WriteLine("--");
          }
        }
      }
      foreach (string file in files.Where((e) => e.EndsWith(".up.sql"))) {
        Console.WriteLine($"Upping: {file}");
        string sql = File.ReadAllText(file);
        _connection.Execute(sql);
        if (_verbose) {
          Console.WriteLine("--");
          Console.WriteLine($"Ran sql:\n{sql}");
          Console.WriteLine("--");
        }
      }
      return true;
    }

    public bool Seed() {
      if (!Directory.Exists("Seeds"))
        return false;
      foreach (string file in Directory.GetFiles("Seeds")) {
        string sql = File.ReadAllText(file);
        Console.WriteLine($"{file} affected {_connection.Execute(sql)} rows");
        if (_verbose) {
          Console.WriteLine("--");
          Console.WriteLine($"Ran sql:\n{sql}");
          Console.WriteLine("--");
        }
      }
      List<User> users = _connection.Query<User>("SELECT * FROM Users").ToList();
      if (users.Count == 0) {
        return false;
      }
      foreach (var user in users) {
        user.Password = BC.HashPassword(user.Password, 12);
        Console.WriteLine($"User perm: {user.PermissionId}");
      }
      _connection.BulkDelete(users);
      Console.WriteLine(users.Count);
      _connection.BulkInsert(users);
      return true;
    }
  }
}
