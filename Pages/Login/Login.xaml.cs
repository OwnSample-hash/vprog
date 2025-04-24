using System.Windows;
using System.Windows.Controls;
using car.Pages.Session;

namespace car.Pages.Login {
  /// <summary>
  /// Interaction logic for Login.xaml
  /// </summary>
  public sealed partial class Login : Page {

    public string Username { get => tbNev.Text; }

    public string Password { get => pbJelszo.Password; }

    public bool DialogResult { get; private set; } = false;

    public Login(ESessionAuthError eSessionAuthError = ESessionAuthError.OK) {
      InitializeComponent();
      switch (eSessionAuthError) {
        case ESessionAuthError.InvalidCredentials:
          MessageBox.Show("Hibás felhasználónév vagy jelszó!");
          break;
        case ESessionAuthError.GeneralError:
          MessageBox.Show("Hiba történt!");
          break;
      }
      tbNev.Focus();
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e) {
      if (tbNev.Text != "" && pbJelszo.Password != "") {
        DialogResult = true;
      } else {
        MessageBox.Show("A mezőket ki kell tölteni!");
      }
    }

    private void pbJelszo_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
      if (e.Key == System.Windows.Input.Key.Enter) {
        btnLogin_Click(sender, e);
      }
    }

    public bool ShowDialog() {
      throw new NotImplementedException("Todo fhinish it");
      //var result = Show();
      //if (result == true) {
      //  return true;
      //} else {
      //  return false;
      //}
    }
  }
}
