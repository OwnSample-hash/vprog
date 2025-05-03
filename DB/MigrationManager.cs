using System.IO;
using System.Windows;
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

    ~MigrationManager() {
      _connection.Close();
    }

    public bool Migrate(bool downFirst) {
      if (!Directory.Exists("Migrations")) {
        return false;
      }
      var files = Directory.GetFiles("Migrations");
      if (downFirst) {
        foreach (string file in files.Reverse().Where((e) => e.EndsWith(".down.sql"))) {
          MainWindow.Logger.SysLog($"Downing: {file}", Logging.ELogLvl.TRACE);
          string sql = File.ReadAllText(file);
          try {
            _connection.Execute(sql);
          } catch (SqlException ex) {
            MessageBox.Show($"Error running down migration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            if (ex.Message.Contains("Cannot drop")) {
              MessageBox.Show("Cannot drop the table/trigger, hold sifth to bypass --down", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
          }
          if (_verbose) {
            Console.WriteLine("--");
            Console.WriteLine($"Ran sql:\n{sql}");
            Console.WriteLine("--");
          }
        }
      }
      foreach (string file in files.Where((e) => e.EndsWith(".up.sql"))) {
        MainWindow.Logger.SysLog($"Upping: {file}", Logging.ELogLvl.TRACE);
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
        MainWindow.Logger.SysLog($"{file} affected {_connection.Execute(sql)} rows", Logging.ELogLvl.TRACE);
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
      }
      _connection.BulkUpdate(users);
      return true;
    }
  }
}
