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
using System.Xml.Linq;
using ZooMania.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using ZooMania.Models;
using System.Data;

namespace ZooMania.View
{
    /// <summary>
    /// Logika interakcji dla klasy StorageView.xaml
    /// </summary>
    public partial class StorageView : UserControl
    {
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        private List<ProductModel> storage = new List<ProductModel>();
        public StorageView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            WypelnijComboBox();
            WypelnijDataGrid();
        }

        private const int MinValue = -99;
        private const int MaxValue = 999;

        private void IncreaseButton_Click(object sender, RoutedEventArgs e)
        {
            int currentValue = int.Parse(txtQuantity.Text);
            if (currentValue < MaxValue)
            {
                txtQuantity.Text = (currentValue + 1).ToString();
            }
        }

        private void DecreaseButton_Click(object sender, RoutedEventArgs e)
        {
            int currentValue = int.Parse(txtQuantity.Text);
            if (currentValue > MinValue)
            {
                txtQuantity.Text = (currentValue - 1).ToString();
            }
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

        private void WypelnijDataGrid()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                string sql = "SELECT p.Id, p.Name, p.Price, c.Name as CategoryName, s.Quantity FROM Products p, Categories c ON p.Category_id = c.Id LEFT JOIN Storage s ON p.Id = s.Product_id";
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    using (SqliteDataReader rdr = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(rdr);
                        dgMagazyn.ItemsSource = dt.DefaultView;
                    }
                }
                connection.Close();
            }
        }

        private void DodajDoStorage(object sender, RoutedEventArgs e)
        {
            if (cbProdukty.SelectedItem == null || string.IsNullOrEmpty(txtQuantity.Text))
            {
                MessageBox.Show("Proszę wybrać produkt i wprowadzić ilość.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ProductModel selectedProduct = (ProductModel)cbProdukty.SelectedItem;
            int ilosc;
            if (int.TryParse(txtQuantity.Text, out ilosc))
            {
                using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
                {
                    connection.Open();
                    // Sprawdzenie, czy produkt o podanym ID już istnieje w magazynie
                    string checkSql = "SELECT COUNT(*) FROM Storage WHERE Product_id = @ProductId";
                    using (SqliteCommand checkCommand = new SqliteCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@ProductId", selectedProduct.Id);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                        if (count > 0)
                        {
                            // Jeśli produkt już istnieje, aktualizuj ilość w istniejącym rekordzie
                            string updateSql = "UPDATE Storage SET Quantity = Quantity + @Quantity WHERE Product_id = @ProductId";
                            using (SqliteCommand updateCommand = new SqliteCommand(updateSql, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@Quantity", ilosc);
                                updateCommand.Parameters.AddWithValue("@ProductId", selectedProduct.Id);
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Jeśli produkt nie istnieje, dodaj nowy rekord
                            string insertSql = "INSERT INTO Storage (Product_id, Quantity) VALUES (@ProductId, @Quantity)";
                            using (SqliteCommand insertCommand = new SqliteCommand(insertSql, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@ProductId", selectedProduct.Id);
                                insertCommand.Parameters.AddWithValue("@Quantity", ilosc);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    MessageBox.Show("Stan magazynu został zmieniony.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Wprowadź prawidłową ilość.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Ponowne pobranie danych z bazy danych
            storage.Clear(); // Wyczyść listę przed odświeżeniem

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                string sql = "SELECT P.Id, P.Name, P.Price, C.Name AS CategoryName, S.Quantity FROM Products P JOIN Categories C ON P.Category_id = C.Id LEFT JOIN Storage S ON P.Id = S.Product_id";
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    using (SqliteDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            int Id = rdr.GetInt32(0);
                            string Name = rdr.GetString(1);
                            decimal Price = rdr.GetDecimal(2);
                            string CategoryName = rdr.GetString(3);
                            int Quantity = rdr.IsDBNull(4) ? 0 : rdr.GetInt32(4);

                            ProductModel produkt = new ProductModel(Id, Name, Price, "", null, CategoryName);
                            produkt.Quantity = Quantity;
                            storage.Add(produkt);
                        }
                    }
                }
                connection.Close();
            }

            // Odświeżenie danych w DataGrid
            dgMagazyn.ItemsSource = null;
            dgMagazyn.ItemsSource = storage;

            MessageBox.Show("Dane zostały odświeżone.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }




        //
    }
}
