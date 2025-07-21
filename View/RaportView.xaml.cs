using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace ZooMania.View
{
    /// <summary>
    /// Logika interakcji dla klasy RaportView.xaml
    /// </summary>
    public partial class RaportView : UserControl
    {
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        public RaportView()
        {
            InitializeComponent(); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> tableNames = new List<string> { "Klienci", "Zamówienia", "Kategorie", "Produkty", "Magazyn", "Finanse" }; // Lista nazw tabel do wyboru
            TableListBox.ItemsSource = tableNames;
        }

        private void GenerujRaport_Click(object sender, RoutedEventArgs e)
        {
            string selectedTableName = TableListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedTableName))
            {
                MessageBox.Show("Nie wybrano tabeli.");
                return;
            }

            // Pobierz dane z wybranej tabeli
            List<string> data = PobierzDaneZTabeli(selectedTableName);

            // Generuj raport w formacie tekstowym
            StringBuilder raport = new StringBuilder();
            foreach (string row in data)
            {
                raport.AppendLine(row);
            }

            // Zapisz raport do pliku tekstowego
            string sciezkaPliku = $"Raporty/raport_{selectedTableName}.txt";
            File.WriteAllText(sciezkaPliku, raport.ToString());

            MessageBox.Show($"Raport został zapisany do pliku: {sciezkaPliku}");
        }

        private List<string> PobierzDaneZTabeli(string tableName)
        {
            List<string> data = new List<string>();

            string query = string.Empty;
            string headers = string.Empty; // Nagłówki kolumn

            switch (tableName)
            {
                case "Klienci":
                    query = "SELECT * FROM Customers";
                    headers = "ID, Imię, Nazwisko, e-mail, Adres, Kod_pocztowy, Miasto, Kraj";
                    break;
                case "Zamówienia":
                    query = "SELECT * FROM Orders";
                    headers = "ID, ID_Klienta, ID_Produktu, Data_Zamowienia, Data_Dostawy, Status, Koszt_dostawy";
                    break;
                case "Kategorie":
                    query = "SELECT * FROM Categories";
                    headers = "ID, Nazwa";
                    break;
                case "Produkty":
                    query = "SELECT Id, Name, Price, Description, Category_id FROM Products";
                    headers = "ID, Nazwa, Cena, Opis, Kategoria_ID";
                    break;
                case "Magazyn":
                    query = "SELECT * FROM Storage";
                    headers = "ID_Produktu, Ilosc_na_magazynie";
                    break;
                case "Finanse":
                    query = "SELECT * FROM Finances";
                    headers = "ID, Przychody, Wydatki, Zysk";
                    break;
                default:
                    throw new ArgumentException("Nieprawidłowa nazwa tabeli.", nameof(tableName));
            }

            data.Add(headers); // Dodaj nagłówki do listy

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(query, connection);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string row = string.Empty;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row += reader[i].ToString() + ", ";
                    }
                    data.Add(row.TrimEnd(',', ' '));
                }
            }

            return data;
        }

        private void GenerujRaport2_Click(object sender, RoutedEventArgs e)
        {
            // Pobierz datę z DatePicker
            DateTime selectedDate = DatePicker.SelectedDate ?? DateTime.Now;

            // Usuń godzinę, minuty i sekundy, pozostawiając tylko datę
            selectedDate = selectedDate.Date;

            // Pobierz liczbę klientów z zamówieniami na podaną datę
            int numberOfCustomersWithOrders = GetNumberOfCustomersWithOrders(selectedDate);

            // Tworzenie tekstu raportu
            StringBuilder reportText = new StringBuilder();
            reportText.AppendLine("Liczba klientów z zamówieniem w określonym dniu: " + numberOfCustomersWithOrders.ToString());

            // Zapisz raport do pliku
            string selectedTableName = $"liczba_klientow_{selectedDate.ToString("yyyy-MM-dd")}";
            string raportFileName = $"raport_{selectedTableName}.txt"; // Nazwa pliku raportu
            string raportFilePath = Path.Combine("Raporty", raportFileName); // Ścieżka pliku raportu
            Directory.CreateDirectory("Raporty"); // Utworzenie folderu "Raporty", jeśli nie istnieje
            File.WriteAllText(raportFilePath, reportText.ToString()); // Zapisanie raportu do pliku

            MessageBox.Show($"Raport został zapisany do pliku: {raportFilePath}", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private int GetNumberOfCustomersWithOrders(DateTime date)
        {
            int numberOfCustomers = 0;

            try
            {
                using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
                {
                    connection.Open();

                    // Zapytanie SQL do pobrania liczby klientów z zamówieniami na podaną datę (ignorując godziny)
                    string sql = "SELECT COUNT(DISTINCT Customer_id) FROM Orders WHERE DATE(Order_date) = DATE(@date);";
                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        // Parametr z datą (tylko data, bez godziny)
                        command.Parameters.AddWithValue("@date", date.Date);

                        // Wykonanie zapytania i odczytanie wyniku
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            numberOfCustomers = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas pobierania danych: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return numberOfCustomers;
        }





    }
}
