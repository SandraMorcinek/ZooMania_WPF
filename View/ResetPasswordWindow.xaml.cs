using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
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

namespace ZooMania.View
{
    /// <summary>
    /// Logika interakcji dla klasy ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        //  ścieżkę dostępu do bazy danych Sqlite
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        public ResetPasswordWindow()
        {
            InitializeComponent();
        }
        // przesuwanie okna za pomocą myszki
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        // Minimalizacja okna
        private void btnMinimalizuj_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        // Zamknięcie okna
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnResetPassword(object sender, RoutedEventArgs e)
        {
            // Sprawdzenie czy wszystkie pola są uzupełnione
            if (string.IsNullOrEmpty(UsernameTextBox.Text) || string.IsNullOrEmpty(EmailTextBox.Text) || string.IsNullOrEmpty(PasswordTB.Text))
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Sprawdzenie czy hasło ma co najmniej 3 znaki
            if (PasswordTB.Text.Length < 3)
            {
                MessageBox.Show("Hasło musi mieć co najmniej 3 znaki!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Przygotowanie danych do dodania do bazy danych
            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string newpassword = PasswordTB.Text;
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                // Sprawdzenie, czy email już istnieje w bazie danych
                var emailExistsCommand = new SqliteCommand(LokalizacjaBazy, connection);
                emailExistsCommand.CommandText = "SELECT COUNT(*) FROM User WHERE Email = @Email";
                emailExistsCommand.Parameters.AddWithValue("@Email", email);
                int count = Convert.ToInt32(emailExistsCommand.ExecuteScalar());
                if (count == 0)
                {
                    MessageBox.Show("Podany adres email nie istnieje w bazie danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Sprawdzenie, czy podany użytkownik posiada podany adres e-mail
                var userExistsCommand = new SqliteCommand(LokalizacjaBazy, connection);
                userExistsCommand.CommandText = "SELECT COUNT(*) FROM User WHERE Username = @Username AND Email = @Email";
                userExistsCommand.Parameters.AddWithValue("@Username", username);
                userExistsCommand.Parameters.AddWithValue("@Email", email);
                int userCount = Convert.ToInt32(userExistsCommand.ExecuteScalar());
                if (userCount == 0)
                {
                    MessageBox.Show("Podany użytkownik nie posiada podanego adresu e-mail w bazie danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Aktualizacja hasła
                string sql = "UPDATE User SET Password=@newPassword WHERE Username = @userUsername and Email = @userEmail";
                SqliteCommand command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@newPassword", newpassword);
                command.Parameters.AddWithValue("@userUsername", username);
                command.Parameters.AddWithValue("@userEmail", email);

                command.ExecuteNonQuery();

                connection.Close();
            }
            // Zamknięcie okna resetu hasła
            MessageBox.Show("Hasło zostało zmienione");
            Close();
        }

    }
}
