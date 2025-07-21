using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ZooMania.Model;
using ZooMania.Models;
using ZooMania.ViewModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ZooMania.View
{
    /// <summary>
    /// Logika interakcji dla klasy SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        private List<UserModel> allUsers = new List<UserModel>();
        public SettingsView()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WyswietlUzytkownikow();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            // Sprawdzenie czy wszystkie pola są uzupełnione
            if (string.IsNullOrEmpty(LoginTextBox.Text) || string.IsNullOrEmpty(PasswordTextBox.Text) || string.IsNullOrEmpty(ImieTextBox.Text) || string.IsNullOrEmpty(NazwiskoTextBox.Text) || string.IsNullOrEmpty(EmailTextBox.Text))
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Przygotowanie danych do dodania do bazy danych
            string login = LoginTextBox.Text;
            string haslo = PasswordTextBox.Text;
            string imie = ImieTextBox.Text;
            string nazwisko = NazwiskoTextBox.Text;
            string email = EmailTextBox.Text;

            // Sprawdzenie, czy w polach Imie i Nazwisko nie ma liczb
            if (imie.Any(char.IsDigit) || nazwisko.Any(char.IsDigit))
            {
                MessageBox.Show("Pola Imię i Nazwisko nie mogą zawierać liczb.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Sprawdzenie, czy login i hasło mają minimum 3 znaki
            if (login.Length < 3 || haslo.Length < 3)
            {
                MessageBox.Show("Login i hasło muszą mieć minimum 3 znaki.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Dodanie użytkownika do bazy danych
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                // Sprawdzenie, czy email już istnieje w bazie danych
                var emailExistsCommand = new SqliteCommand(LokalizacjaBazy, connection);
                emailExistsCommand.CommandText = "SELECT COUNT(*) FROM User WHERE Email = @Email";
                emailExistsCommand.Parameters.AddWithValue("@Email", email);
                int count = Convert.ToInt32(emailExistsCommand.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Podany adres email już istnieje w bazie danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var command = new SqliteCommand(LokalizacjaBazy, connection);
                command.CommandText = "INSERT INTO User (Username, Password, Name, LastName, Email) VALUES (@Login, @Haslo, @Imie, @Nazwisko, @Email)";
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Haslo", haslo);
                command.Parameters.AddWithValue("@Imie", imie);
                command.Parameters.AddWithValue("@Nazwisko", nazwisko);
                command.Parameters.AddWithValue("@Email", email);
                command.ExecuteNonQuery();

                connection.Close();
            }
            MessageBox.Show("Użytkownik został dodany");
            LoginTextBox.Text = "";
            PasswordTextBox.Text = "";
            ImieTextBox.Text = "";
            NazwiskoTextBox.Text = "";
            EmailTextBox.Text = "";
        }


        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            // Pobranie ID użytkownika z pola tekstowego
            int userId;
            if (!int.TryParse(UserIdTextBox.Text, out userId))
            {
                MessageBox.Show("Nieprawidłowy format ID użytkownika!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Sprawdzenie czy użytkownik o ID równym 1 nie jest usuwany
            if (userId == 1)
            {
                MessageBox.Show("Nie możesz usunąć użytkownika o ID równym 1!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Usunięcie użytkownika z bazy danych
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                var command = new SqliteCommand(LokalizacjaBazy, connection);
                command.CommandText = "DELETE FROM User WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", userId);
                int rowsAffected = command.ExecuteNonQuery();

                connection.Close();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Użytkownik został usunięty z bazy danych.");
                }
                else
                {
                    MessageBox.Show("Nie znaleziono użytkownika o podanym ID!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void WyswietlUzytkownikow()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                string sql = "SELECT Id, Username, Password, Name, LastName, Email, ProfilePicture FROM User";
                SqliteCommand command = new SqliteCommand(sql, connection);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string id = reader.GetString(0);
                    string username = reader.GetString(1);
                    //string password = reader.GetString(2);
                    string name = reader.GetString(3);
                    string lastname = reader.GetString(4);
                    string email = reader.GetString(5);
                    byte[] imageData = reader.IsDBNull(6) ? null : (byte[])reader[6];

                    UserModel user = new UserModel();
                    user.User(id, username, name, lastname, email, imageData);

                    allUsers.Add(user);
                }
                connection.Close();
            }
            dgUsers.ItemsSource = allUsers;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Ponowne pobranie danych z bazy danych
            allUsers.Clear();
            WyswietlUzytkownikow();

            // Odświeżenie danych dla DataGrid
            dgUsers.ItemsSource = null;
            dgUsers.ItemsSource = allUsers;
            CollectionViewSource.GetDefaultView(dgUsers.ItemsSource).Refresh();

            MessageBox.Show("Dane zostały odświeżone.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnDodajZdjecie_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki graficzne (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                byte[] imageData = OdczytajObrazek(filePath); // Wywołanie nowo utworzonej metody

                if (!int.TryParse(useridfoto.Text, out int id)) // Sprawdzenie czy wpisane ID jest liczbą całkowitą
                {
                    MessageBox.Show("Wprowadź poprawne ID użytkownika.");
                    return; // Przerwanie dalszego wykonywania kodu w przypadku błędnego ID
                }

                using (var connection = new SqliteConnection(LokalizacjaBazy))
                {
                    connection.Open();

                    using (var cmd = new SqliteCommand("UPDATE [User] SET ProfilePicture=@imageData WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@imageData", imageData);
                        cmd.Parameters.AddWithValue("@id", id);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Zdjęcie zostało dodane do rekordu o ID " + id);
                        }
                        else
                        {
                            MessageBox.Show("Nie udało się dodać zdjęcia do rekordu o ID " + id);
                        }
                    }
                }

                LoadProfilePicture(id); // Wywołanie metody LoadProfilePicture do załadowania zaktualizowanego zdjęcia
            }
        }


        private byte[] OdczytajObrazek(string filePath)
        {
            byte[] imageData = null;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                imageData = new byte[stream.Length];
                stream.Read(imageData, 0, (int)stream.Length);
            }
            return imageData;
        }

        private void LoadProfilePicture(int id)
        {
            using (var connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                using (var cmd = new SqliteCommand("SELECT ProfilePicture FROM [User] WHERE Id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                byte[] imageData = (byte[])reader["ProfilePicture"];
                                using (var stream = new MemoryStream(imageData))
                                {
                                    BitmapImage image = new BitmapImage();
                                    image.BeginInit();
                                    image.CacheOption = BitmapCacheOption.OnLoad;
                                    image.StreamSource = stream;
                                    image.EndInit();

                                    // Ustawienie obrazka jako źródło Image
                                    Dispatcher.Invoke(() =>
                                    {
                                        imgProfil.Source = image;
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnUsunZdjecie_Click(object sender, EventArgs e)
        {
            if (int.TryParse(useridfoto.Text, out int userId)) // Sprawdzenie czy wpisane ID jest liczbą całkowitą
            {
                using (var connection = new SqliteConnection(LokalizacjaBazy))
                {
                    connection.Open();
                    using (SqliteCommand command = new SqliteCommand(LokalizacjaBazy, connection))
                    {
                        command.CommandText = "UPDATE [User] SET ProfilePicture = NULL WHERE Id = @UserId";
                        command.CommandType = CommandType.Text;

                        command.Parameters.AddWithValue("@UserId", userId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Zdjęcie zostało usunięte z bazy danych.");
                        }
                        else
                        {
                            MessageBox.Show("Nie udało się usunąć zdjęcia z bazy danych.");
                        }
                    }
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Wprowadź prawidłowe ID użytkownika.");
            }
        }

    }
}
