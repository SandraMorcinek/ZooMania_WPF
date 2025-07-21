using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using ZooMania.Model;

namespace ZooMania.View
{
    /// <summary>
    /// Logika interakcji dla klasy ExecutionOfTheOrderView.xaml
    /// </summary>
    public partial class ExecutionOfTheOrderView : UserControl
    {
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        private List<ProductModel> storage = new List<ProductModel>();
        public ExecutionOfTheOrderView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WypelnijComboBox();
        }

        private void WypelnijComboBox()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                string sql = "SELECT Id, Name FROM Products";
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    using (SqliteDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            int Id = rdr.GetInt32(0);
                            string Name = rdr.GetString(1);

                            ProductModel produkt = new ProductModel(Id, Name, 0, "", null, "");
                            cbProdukty.Items.Add(produkt);
                        }
                    }
                }
                connection.Close();
            }
        }

        private void DodajKlienta_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ImieTextBox.Text) || string.IsNullOrWhiteSpace(NazwiskoTextBox.Text)
                || string.IsNullOrWhiteSpace(EmailTextBox.Text) || string.IsNullOrWhiteSpace(AdresTextBox.Text)
                || string.IsNullOrWhiteSpace(KodTextBox.Text) || string.IsNullOrWhiteSpace(MiastoTextBox.Text)
                || string.IsNullOrWhiteSpace(KrajTextBox.Text))
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione.");
                return;
            }

            if (!Regex.IsMatch(ImieTextBox.Text, @"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
            {
                MessageBox.Show("Imię nie może zawierać cyfr ani znaków specjalnych.");
                return;
            }

            if (!Regex.IsMatch(NazwiskoTextBox.Text, @"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
            {
                MessageBox.Show("Nazwisko nie może zawierać cyfr ani znaków specjalnych.");
                return;
            }

            if (KrajTextBox.Text == "Poland")
            {
                if (!Regex.IsMatch(KodTextBox.Text, @"^\d{2}-\d{3}$"))
                {
                    MessageBox.Show("Kod pocztowy może zawierać tylko cyfry i musi być w formacie xx-xxx.");
                    return;
                }
            }
            else if (!Regex.IsMatch(KodTextBox.Text, @"^[0-9]+$"))
            {
                MessageBox.Show("Kod pocztowy może zawierać tylko cyfry.");
                return;
            }

            if (!Regex.IsMatch(MiastoTextBox.Text, @"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
            {
                MessageBox.Show("Miasto nie może zawierać cyfr ani znaków specjalnych.");
                return;
            }

            if (!Regex.IsMatch(KrajTextBox.Text, @"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
            {
                MessageBox.Show("Kraj nie może zawierać cyfr ani znaków specjalnych.");
                return;
            }

            string queryCheck = "SELECT COUNT(*) FROM Customers WHERE Email = @Email";
            string queryInsert = "INSERT INTO Customers (First_name, Last_name, Email, Address, Postal_code, City, Country) VALUES (@FirstName, @LastName, @Email, @Address, @PostalCode, @City, @Country)";

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                // Sprawdzenie czy klient o podanym adresie e-mail już istnieje
                using (SqliteCommand commandCheck = new SqliteCommand(queryCheck, connection))
                {
                    commandCheck.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                    int count = Convert.ToInt32(commandCheck.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Klient o podanym adresie e-mail już istnieje.");
                        return;
                    }
                }

                // Dodanie klienta do bazy danych
                using (SqliteCommand commandInsert = new SqliteCommand(queryInsert, connection))
                {
                    commandInsert.Parameters.AddWithValue("@FirstName", ImieTextBox.Text);
                    commandInsert.Parameters.AddWithValue("@LastName", NazwiskoTextBox.Text);
                    commandInsert.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                    commandInsert.Parameters.AddWithValue("@Address", AdresTextBox.Text);
                    commandInsert.Parameters.AddWithValue("@PostalCode", KodTextBox.Text);
                    commandInsert.Parameters.AddWithValue("@City", MiastoTextBox.Text);
                    commandInsert.Parameters.AddWithValue("@Country", KrajTextBox.Text);

                    int rowsAffected = commandInsert.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Klient został dodany do bazy danych.");
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się dodać klienta do bazy danych.");
                    }
                }
                connection.Close();
            }
        }

        private void WczytajKlienta_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IdKlientaTextBox.Text))
            {
                MessageBox.Show("Podaj ID klienta.");
                return;
            }

            int idKlienta;
            if (!int.TryParse(IdKlientaTextBox.Text, out idKlienta))
            {
                MessageBox.Show("Nieprawidłowe ID klienta.");
                return;
            }

            string query = "SELECT First_name, Last_name, Address, Postal_code, City, Country, Email FROM Customers WHERE Id = @CustomerID";

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", idKlienta);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ImieNazwiskoTB.Text = reader["First_name"].ToString() + " " + reader["Last_name"].ToString();
                            AdresTB.Text = reader["Address"].ToString() + ", " + reader["Postal_code"].ToString() + " " + reader["City"].ToString() + ", " + reader["Country"].ToString();
                            EmailTB.Text = reader["Email"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Klient o podanym ID nie istnieje.");
                        }
                    }
                }

                connection.Close();
            }
        }

        private void DodajZamowienie_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IdKlientaTextBox.Text) || string.IsNullOrWhiteSpace(ImieNazwiskoTB.Text) || string.IsNullOrWhiteSpace(AdresTB.Text) || string.IsNullOrWhiteSpace(EmailTB.Text) || cbProdukty.SelectedItem == null || cbKosztDostawy.SelectedItem == null)
            {
                MessageBox.Show("Wypełnij wszystkie pola przed dodaniem zamówienia.");
                return;
            }

            int idKlienta = Convert.ToInt32(IdKlientaTextBox.Text);
            string imieNazwisko = ImieNazwiskoTB.Text;
            string adres = AdresTB.Text;
            string email = EmailTB.Text;
            int produkt = ((ProductModel)cbProdukty.SelectedItem).Id;
            DateTime dataZamowienia = DateTime.Now; // Aktualna data i czas systemowy
            DateTime dataDostawy = DateTime.Now.AddDays(3).Date.Add(new TimeSpan(12, 0, 0)); // Data dostawy 3 dni po aktualnej dacie o godzinie 12:00 po południu
            decimal kosztDostawy = Convert.ToDecimal(((ComboBoxItem)cbKosztDostawy.SelectedItem).Content);

            // Sprawdzenie czy email nalezy do podanego id klienta
            string emailQuery = "SELECT Email FROM Customers WHERE Id = @CustomerID";
            string customerEmail;

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(emailQuery, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", idKlienta);

                    customerEmail = (string)command.ExecuteScalar();
                }

                connection.Close();
            }

            if (customerEmail == null)
            {
                MessageBox.Show("Nie znaleziono klienta o podanym ID.");
                return;
            }

            if (customerEmail != email)
            {
                MessageBox.Show("Podany email nie należy do klienta o podanym ID.");
                return;
            }


            string query = "INSERT INTO Orders (Customer_id, Product_id, Order_date, Shipping_date, Status, Transport) VALUES (@CustomerID, @ProductID, @OrderDate, @ShippingDate, @Status, @Transport)";

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", idKlienta);
                    command.Parameters.AddWithValue("@ProductID", produkt);
                    command.Parameters.AddWithValue("@OrderDate", dataZamowienia.ToString("yyyy-MM-dd HH:mm:ss")); // Godzina, minuta, sekunda
                    command.Parameters.AddWithValue("@ShippingDate", dataDostawy.ToString("yyyy-MM-dd HH:mm:ss")); // Data dostawy z ustawioną godziną
                    command.Parameters.AddWithValue("@Status", "Nowe");
                    command.Parameters.AddWithValue("@Transport", kosztDostawy);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Zamówienie zostało dodane do bazy danych.");
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się dodać zamówienia do bazy danych.");
                    }
                }

                connection.Close();
            }
        }

        private void UsunZamowienie_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IdOrderTB.Text))
            {
                MessageBox.Show("Podaj ID zamówienia do usunięcia.");
                return;
            }

            int idZamowienia;
            if (!int.TryParse(IdOrderTB.Text, out idZamowienia))
            {
                MessageBox.Show("ID zamówienia musi być liczbą całkowitą.");
                return;
            }

            string query = "SELECT COUNT(*) FROM Orders WHERE Id = @OrderID";

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderID", idZamowienia);

                    int result = Convert.ToInt32(command.ExecuteScalar());

                    if (result == 0)
                    {
                        MessageBox.Show("Zamówienie o podanym ID nie istnieje w bazie danych.");
                        return;
                    }
                }

                query = "DELETE FROM Orders WHERE Id = @OrderID";

                using (SqliteCommand deleteCommand = new SqliteCommand(query, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@OrderID", idZamowienia);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Zamówienie zostało usunięte z bazy danych.");
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się usunąć zamówienia z bazy danych.");
                    }
                }

                connection.Close();
            }
        }

        private void ZmienStatus_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IdZamowieniaStatusTB.Text))
            {
                MessageBox.Show("Podaj ID zamówienia.");
                return;
            }

            int idZamowienia;
            if (!int.TryParse(IdZamowieniaStatusTB.Text, out idZamowienia))
            {
                MessageBox.Show("ID zamówienia musi być liczbą całkowitą.");
                return;
            }

            string selectedStatus = (cbStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(selectedStatus))
            {
                MessageBox.Show("Wybierz status zamówienia.");
                return;
            }

            string queryCheckExistence = "SELECT COUNT(*) FROM Orders WHERE Id = @OrderID";

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                using (SqliteCommand commandCheckExistence = new SqliteCommand(queryCheckExistence, connection))
                {
                    commandCheckExistence.Parameters.AddWithValue("@OrderID", idZamowienia);

                    int orderExistence = Convert.ToInt32(commandCheckExistence.ExecuteScalar());

                    if (orderExistence == 0)
                    {
                        MessageBox.Show("Podane ID zamówienia nie istnieje w bazie.");
                        return;
                    }
                }

                string queryUpdateStatus = "UPDATE Orders SET Status = @Status WHERE Id = @OrderID";

                using (SqliteCommand commandUpdateStatus = new SqliteCommand(queryUpdateStatus, connection))
                {
                    commandUpdateStatus.Parameters.AddWithValue("@Status", selectedStatus);
                    commandUpdateStatus.Parameters.AddWithValue("@OrderID", idZamowienia);

                    int rowsAffected = commandUpdateStatus.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Status zamówienia został zmieniony.");
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się zmienić statusu zamówienia.");
                    }
                }

                connection.Close();
            }
        }

        private void ZaktualizujDate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IdZamowieniaDataTB.Text))
            {
                MessageBox.Show("Podaj ID zamówienia.");
                return;
            }

            int idZamowienia;
            if (!int.TryParse(IdZamowieniaDataTB.Text, out idZamowienia))
            {
                MessageBox.Show("ID zamówienia musi być liczbą całkowitą.");
                return;
            }

            DateTime selectedDate = dpSData.SelectedDate ?? DateTime.MinValue;

            if (selectedDate == DateTime.MinValue)
            {
                MessageBox.Show("Wybierz datę dostawy.");
                return;
            }

            // Uzyskanie daty bez godziny
            DateTime selectedDateWithoutTime = selectedDate.Date;

            string queryCheckId = "SELECT COUNT(*) FROM Orders WHERE Id = @OrderID";
            string queryUpdateDate = "UPDATE Orders SET Shipping_date = @DataDostawy WHERE Id = @OrderID";

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                // Sprawdzenie, czy podane ID zamówienia istnieje w bazie
                using (SqliteCommand commandCheckId = new SqliteCommand(queryCheckId, connection))
                {
                    commandCheckId.Parameters.AddWithValue("@OrderID", idZamowienia);

                    int orderCount = Convert.ToInt32(commandCheckId.ExecuteScalar());

                    if (orderCount == 0)
                    {
                        MessageBox.Show("Zamówienie o podanym ID nie istnieje w bazie.");
                        return;
                    }
                }

                using (SqliteCommand commandUpdateDate = new SqliteCommand(queryUpdateDate, connection))
                {
                    commandUpdateDate.Parameters.AddWithValue("@DataDostawy", selectedDateWithoutTime); // Używanie daty bez godziny
                    commandUpdateDate.Parameters.AddWithValue("@OrderID", idZamowienia);

                    int rowsAffected = commandUpdateDate.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data dostawy została zaktualizowana.");
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się zaktualizować daty dostawy.");
                    }
                }

                connection.Close();
            }
        }





    }
}
