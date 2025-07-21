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
using ZooMania.Model;
using ZooMania.Models;

namespace ZooMania.View
{
    /// <summary>
    /// Logika interakcji dla klasy CategoryView.xaml
    /// </summary>
    public partial class CategoryView : UserControl
    {
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        private List<CategoryModel> allCategories = new List<CategoryModel>();
        public CategoryView()
        {
            InitializeComponent();
        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            // Sprawdzenie czy pole jest uzupełnione
            if (string.IsNullOrEmpty(CategoryTextBox.Text))
            {
                MessageBox.Show("Pole musi być wypełnione!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Sprawdzenie czy nazwa kategorii zawiera liczby
            if (CategoryTextBox.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Nazwa kategorii nie może zawierać liczb!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string kategoria = CategoryTextBox.Text;

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                // Sprawdzenie, czy kategoria już istnieje w bazie danych
                var categoryExistsCommand = new SqliteCommand(LokalizacjaBazy, connection);
                categoryExistsCommand.CommandText = "SELECT COUNT(*) FROM Categories WHERE Name = @Nazwa";
                categoryExistsCommand.Parameters.AddWithValue("@Nazwa", kategoria);
                int count = Convert.ToInt32(categoryExistsCommand.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Podana nazwa już istnieje", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var command = new SqliteCommand(LokalizacjaBazy, connection);
                command.CommandText = "INSERT INTO Categories (Name) VALUES (@Nazwa)";
                command.Parameters.AddWithValue("@Nazwa", kategoria);

                command.ExecuteNonQuery();

                connection.Close();
            }
            MessageBox.Show("Kategoria została dodana");
            CategoryTextBox.Text = "";
        }


        private void WyswietlKategorie()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                string sql = "SELECT c.Id, c.Name, COUNT(p.Id) AS ProductCount FROM Categories c LEFT JOIN Products p ON c.Id = p.Category_id GROUP BY c.Id, c.Name";
                SqliteCommand command = new SqliteCommand(sql, connection);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int productCount = reader.GetInt32(2);

                    CategoryModel category = new CategoryModel(id, name);
                    category.ProductCount = productCount;

                    allCategories.Add(category);
                }
                connection.Close();
            }
            dgCategory.ItemsSource = allCategories;
        }

        private void dgCategory_Loaded(object sender, RoutedEventArgs e)
        {
            WyswietlKategorie();
        }

        private void btnWczytaj_Click(object sender, RoutedEventArgs e)
        {
            int categoryId;
            if (int.TryParse(txtCategoryId.Text, out categoryId))
            {
                // Wyszukanie kategorii o podanym identyfikatorze
                CategoryModel category = allCategories.FirstOrDefault(c => c.Id == categoryId);
                if (category != null)
                {
                    // Przypisanie nazwy kategorii do TextBoxa
                    txtCategoryName.Text = category.Name;
                }
                else
                {
                    MessageBox.Show("Kategoria o podanym ID nie została znaleziona.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Podano nieprawidłowy format ID kategorii.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnZapisz_Click(object sender, RoutedEventArgs e)
        {
            int categoryId;
            if (int.TryParse(txtCategoryId.Text, out categoryId))
            {
                // Wyszukanie kategorii o podanym identyfikatorze
                CategoryModel category = allCategories.FirstOrDefault(c => c.Id == categoryId);
                if (category != null)
                {
                    // Sprawdzenie, czy podana nazwa kategorii już istnieje w bazie danych
                    using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
                    {
                        connection.Open();

                        var categoryExistsCommand = new SqliteCommand(LokalizacjaBazy, connection);
                        categoryExistsCommand.CommandText = "SELECT COUNT(*) FROM Categories WHERE Name = @Nazwa AND Id != @Id";
                        categoryExistsCommand.Parameters.AddWithValue("@Nazwa", txtCategoryName.Text);
                        categoryExistsCommand.Parameters.AddWithValue("@Id", categoryId);
                        int count = Convert.ToInt32(categoryExistsCommand.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("Podana nazwa już istnieje w bazie danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        connection.Close();
                    }

                    // Aktualizacja nazwy kategorii na podstawie wpisanej nowej nazwy
                    category.Name = txtCategoryName.Text;

                    // Zapisanie zmian do bazy danych
                    using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
                    {
                        connection.Open();

                        string sql = "UPDATE Categories SET Name=@name WHERE Id=@id";
                        SqliteCommand command = new SqliteCommand(sql, connection);
                        command.Parameters.AddWithValue("@name", category.Name);
                        command.Parameters.AddWithValue("@id", category.Id);
                        command.ExecuteNonQuery();

                        connection.Close();
                    }

                    MessageBox.Show("Nazwa kategorii została zaktualizowana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Kategoria o podanym ID nie została znaleziona.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Podano nieprawidłowy format ID kategorii.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Ponowne pobranie danych z bazy danych
            allCategories.Clear();
            WyswietlKategorie();

            // Odświeżenie danych dla DataGrid
            dgCategory.ItemsSource = null;
            dgCategory.ItemsSource = allCategories;
            CollectionViewSource.GetDefaultView(dgCategory.ItemsSource).Refresh();

            MessageBox.Show("Dane zostały odświeżone.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        //
    }
}
