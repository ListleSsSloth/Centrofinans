using System.Windows;

namespace OfficeCheckerWPF
{
    public partial class LoginWindow
    {
        private const string Password1 = "459632";
        private const string Password2 = "nahuiall";

        public LoginWindow()
        {
            InitializeComponent();
            PasswordBox.Focus();
        }

        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password != Password1 && PasswordBox.Password != Password2) return;
            Close();
        }

        private void ButtonBaseClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow != null) Application.Current.MainWindow.Close();
        }
    }
}