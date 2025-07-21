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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZooMania.View
{
    /// <summary>
    /// Logika interakcji dla klasy HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        //  ścieżkę dostępu do bazy danych Sqlite
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        public HomeView()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ilosc klientow
            CustomersCount();
            // ilosc zamowien
            OrdersCount();
            // liczba produktow
            ProductsCount();
            // liczba kategorii
            CategoryCount();
            // liczba towaru na magazynie
            StorageCount();
            // liczba nowych zamowien
            CountNewOrders();
            // zysk całkowity
            CountTotal_Profit();
            // liczba zlozonych dzis zamowien
            CountOrdersToday();
            // liczba uzytkownikow
            UsersCount();
        }
        // Metoda, która zlicza liczbę klientów w bazie danych i wyświetla wynik w TextBoxie.
        private void CustomersCount()
        {
            // Tworzenie połączenia z bazą danych przy użyciu podanej lokalizacji bazy.
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                // Otwarcie połączenia.
                connection.Open();
                // Tworzenie komendy SQL, która zlicza liczbę wierszy w tabeli Customers.
                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM Customers", connection);
                // Wykonanie komendy i przypisanie liczby klientów do zmiennej numberOfCustomers.
                int numberOfCustomers = Convert.ToInt32(command.ExecuteScalar());
                // Ustawienie tekstu w TextBoxie tbNumberOfCustomers na wyświetlenie liczby klientów.
                tbNumberOfCustomers.Text = "Liczba klientów w bazie: \r" + numberOfCustomers.ToString();
            }
        }
        private void OrdersCount()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM Orders", connection);
                int numberOfOrders = Convert.ToInt32(command.ExecuteScalar());
                tbNumberOfOrders.Text = "Liczba zamówień w bazie: \r" + numberOfOrders.ToString();
            }
        }
        private void ProductsCount()
        {
            int numberOfProducts = 0;
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM Products", connection);
                numberOfProducts = Convert.ToInt32(command.ExecuteScalar());

                connection.Close();
            }
            tbNumberOfProducts.Text = "Liczba produktów w bazie: \r" + numberOfProducts.ToString();
        }
        private void CategoryCount()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM Categories", connection);
                int numberOfCategories = Convert.ToInt32(command.ExecuteScalar());
                tbNumberOfCategories.Text = "Liczba kategorii w bazie: \r" + numberOfCategories.ToString();
            }
        }
        private void StorageCount()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT SUM(Quantity) FROM Storage", connection);
                int numberofproductsinstock = Convert.ToInt32(command.ExecuteScalar());
                tbNumberOfStorage.Text = "Liczba towaru na magazynie: \r" + numberofproductsinstock.ToString();
            }
        }
        private void CountNewOrders()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM Orders WHERE Status = 'Nowe'", connection);
                int numberOfNewOrders = Convert.ToInt32(command.ExecuteScalar());
                tbNumberOfNewOrders.Text = "Liczba nowych zamówień: \r" + numberOfNewOrders.ToString();
            }
        }
        private void CountTotal_Profit()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT SUM(Profit) FROM Finances", connection);
                object result = command.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    decimal totalprofit = Convert.ToDecimal(result);
                    tbTotalProfit.Text = "Zysk całkowity: \r" + totalprofit.ToString();
                }
                else
                {
                    // Obsłuż sytuację, gdy wynik zapytania jest DBNull
                    tbTotalProfit.Text = "Zysk całkowity: \rBrak danych";
                }
            }
        }
        private void CountOrdersToday()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                string today = DateTime.Now.Date.ToString("yyyy-MM-dd");
                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM Orders WHERE DATE(Order_date) = @Today", connection);
                command.Parameters.AddWithValue("@Today", today);
                int numberOfOrdersToday = Convert.ToInt32(command.ExecuteScalar());
                tbNumberOfOrdersTooday.Text = "Liczba zamówień z dzisiaj: \r" + numberOfOrdersToday.ToString();
            }
        }
        private void UsersCount()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM User", connection);
                int numberOfUsers = Convert.ToInt32(command.ExecuteScalar());
                tbNumberOfUsers.Text = "Liczba użytkowników w bazie: \r" + numberOfUsers.ToString();
            }
        }
        
    }
}
