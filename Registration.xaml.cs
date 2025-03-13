using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace car
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void btnRegisztral_Click(object sender, RoutedEventArgs e)
        {
            if (tbNev.Text == "")
            {
                tbNev.Focus();
            }
            else
            {
                if (pbJelszo.Password == "")
                {
                    pbJelszo.Focus();
                }
                else
                {
                    StreamWriter sw = new StreamWriter("users.txt", true);
                    sw.WriteLine(String.Format("{0};{1}",
                    tbNev.Text, pbJelszo.Password));
                    sw.Close();
                    MessageBox.Show("Sikeres regisztráció", "Regisztráció", MessageBoxButton.OK);
                    tbNev.Text = "";
                    pbJelszo.Password = "";
                }
            }
        }
    }
}
