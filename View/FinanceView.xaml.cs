using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
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
using ZooMania.Model;

namespace ZooMania.View
{
    /// <summary>
    /// Logika interakcji dla klasy FinanceView.xaml
    /// </summary>
    public partial class FinanceView : UserControl
    {
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        private List<FinanceModel> finanse = new List<FinanceModel>();
        public FinanceView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            WypelnijDataGrid();
            UzupelnijFinances();
        }

        private void WypelnijDataGrid()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                string sql = "SELECT Id, Revenue, Expenses, Profit FROM Finances";
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    using (SqliteDataReader rdr = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(rdr);
                        dgFinances.ItemsSource = dt.DefaultView;
                    }
                }
                connection.Close();
            }
        }

        private void UzupelnijFinances()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                // Wstaw lub zaktualizuj rekordy w tabeli Finances na podstawie zamówień (ID)
                string query = @"INSERT OR IGNORE INTO Finances (Id, Revenue, Expenses, Profit) 
                        SELECT O.Id, SUM(P.Price + O.Transport) AS Revenue, (SUM(P.Price) * 0.8) AS Expenses, (SUM(P.Price + O.Transport) - (P.Price * 0.8)) AS Profit 
                        FROM Orders O INNER JOIN Products P ON O.Product_id = P.Id GROUP BY O.Id ORDER BY O.Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        private void ZaktualizujWydatki(object sender, RoutedEventArgs e)
        {
            // Pobierz ID i kwotę wydatków z pól tekstowych
            int orderId = 0;
            if (int.TryParse(IdZamowieniaTextbox.Text, out orderId))
            {
                decimal expenses = 0;
                if (Decimal.TryParse(WydatkiTextbox.Text, out expenses))
                {
                    using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
                    {
                        connection.Open();

                        // Sprawdź czy istnieje zamówienie o podanym ID
                        string checkQuery = "SELECT COUNT(*) FROM Finances WHERE Id = @Id";
                        using (SqliteCommand checkCommand = new SqliteCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@Id", orderId);
                            int orderCount = Convert.ToInt32(checkCommand.ExecuteScalar());
                            if (orderCount > 0)
                            {
                                // Zaktualizuj rekord w tabeli Finances na podstawie zamówienia (ID)
                                string updateQuery = "UPDATE Finances SET Expenses = @Expenses WHERE Id = @Id";
                                using (SqliteCommand updateCommand = new SqliteCommand(updateQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@Expenses", expenses);
                                    updateCommand.Parameters.AddWithValue("@Id", orderId);
                                    updateCommand.ExecuteNonQuery();
                                }

                                MessageBox.Show("Wydatki zostały zaktualizowane.");
                            }
                            else
                            {
                                MessageBox.Show("Zamówienie o podanym ID nie istnieje.");
                            }
                        }

                        connection.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Nieprawidłowa kwota wydatków.");
                }
            }
            else
            {
                MessageBox.Show("Nieprawidłowy identyfikator zamówienia.");
            }
        }

        private void DodajDoWydatków(object sender, RoutedEventArgs e)
        {
            // Pobierz ID i kwotę wydatków z pól tekstowych
            int orderId = 0;
            if (int.TryParse(IDwydatkidodanieTB.Text, out orderId))
            {
                decimal expenses = 0;
                if (Decimal.TryParse(wydatkidodanieTB.Text, out expenses))
                {
                    using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
                    {
                        connection.Open();

                        // Zaktualizuj rekord w tabeli Finances na podstawie zamówienia (ID)
                        string updateQuery = "UPDATE Finances SET Expenses = Expenses + @Expenses WHERE Id = @Id";
                        using (SqliteCommand command = new SqliteCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Expenses", expenses);
                            command.Parameters.AddWithValue("@Id", orderId);
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }

                    MessageBox.Show("Wydatki zostały dodane do tabeli Finances.");
                }
                else
                {
                    MessageBox.Show("Nieprawidłowa kwota wydatków.");
                }
            }
            else
            {
                MessageBox.Show("Nieprawidłowy identyfikator zamówienia.");
            }
        }

        private void OdejmnijOdWydatków(object sender, RoutedEventArgs e)
        {
            // Pobierz ID i kwotę wydatków z pól tekstowych
            int orderId = 0;
            if (int.TryParse(IDwydatkiminusTB.Text, out orderId))
            {
                decimal expenses = 0;
                if (Decimal.TryParse(wydatkiminusTB.Text, out expenses))
                {
                    using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
                    {
                        connection.Open();

                        // Zaktualizuj rekord w tabeli Finances na podstawie zamówienia (ID)
                        string updateQuery = "UPDATE Finances SET Expenses = Expenses - @Expenses WHERE Id = @Id";
                        using (SqliteCommand command = new SqliteCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Expenses", expenses);
                            command.Parameters.AddWithValue("@Id", orderId);
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }

                    MessageBox.Show("Wydatki zostały odjęte w tabeli Finances.");
                }
                else
                {
                    MessageBox.Show("Nieprawidłowa kwota wydatków.");
                }
            }
            else
            {
                MessageBox.Show("Nieprawidłowy identyfikator zamówienia.");
            }
        }
       
        private void UsunFinanse_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IDusunTB.Text))
            {
                MessageBox.Show("Podaj ID do usunięcia.");
                return;
            }

            int idFinanse;
            if (!int.TryParse(IDusunTB.Text, out idFinanse))
            {
                MessageBox.Show("ID musi być liczbą całkowitą.");
                return;
            }

            string query = "SELECT COUNT(*) FROM Finances WHERE Id = @ID";

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", idFinanse);

                    int result = Convert.ToInt32(command.ExecuteScalar());

                    if (result == 0)
                    {
                        MessageBox.Show("Finanse o podanym ID nie istnieje w bazie danych.");
                        return;
                    }
                }

                query = "DELETE FROM Finances WHERE Id = @ID";

                using (SqliteCommand deleteCommand = new SqliteCommand(query, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@ID", idFinanse);

                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Finanse zostały usunięte z bazy danych.");
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się usunąć finansów z bazy danych.");
                    }
                }

                connection.Close();
            }
        }


        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Ponowne pobranie danych z bazy danych
            finanse.Clear(); // Wyczyść listę przed odświeżeniem

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                string sql = "SELECT Id, Revenue, Expenses, Profit FROM Finances";
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    using (SqliteDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            int Id = rdr.GetInt32(0);
                            decimal Revenue = rdr.GetDecimal(1);
                            decimal Expenses = rdr.GetDecimal(2);
                            decimal Profit = rdr.GetDecimal(3);

                            FinanceModel finance = new FinanceModel(Id, Revenue, Expenses, Profit);

                            finanse.Add(finance);
                        }
                    }
                }
                connection.Close();
            }
            // Odświeżenie danych w DataGrid
            dgFinances.ItemsSource = null;
            dgFinances.ItemsSource = finanse;

            MessageBox.Show("Dane zostały odświeżone.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }


    }
}
