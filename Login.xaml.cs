using System.Windows;
using car.Session;

namespace car {
  /// <summary>
  /// Interaction logic for Login.xaml
  /// </summary>
  public sealed partial class Login : Window {

    public string Username { get => tbNev.Text; }
    public string Password { get => pbJelszo.Password; }

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
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e) {
      if (tbNev.Text != "" && pbJelszo.Password != "") {
        DialogResult = true;
      } else {
        MessageBox.Show("A mezőket ki kell tölteni!");
      }
    }
  }
}
