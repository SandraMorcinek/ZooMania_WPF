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
using System.Drawing;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Reflection.Metadata;
using Image = System.Windows.Controls.Image;
using System.Globalization;
using System.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ZooMania.View
{
    /// <summary>
    /// Logika interakcji dla klasy ProductView.xaml
    /// </summary>
    public partial class ProductView : UserControl
    {
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        private ObservableCollection<ProductModel> allProducts = new ObservableCollection<ProductModel>();

        public ProductView()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WypelnijComboBox();
            WyswietlProdukty();
            WypelnijComboBoxEdit();
        }

        private void WypelnijComboBox()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                string sql = "SELECT Id, Name FROM Categories";
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    using (SqliteDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            int Id = rdr.GetInt32(0);
                            string Name = rdr.GetString(1);
                            cbKategoria.Items.Add(new CategoryModel(Id, Name));
                        }
                    }
                }
                connection.Close();
            }
        }

        private void WyswietlProdukty()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                string sql = "SELECT p.Id, p.Name, p.Price, p.Description, p.ImageData, c.Name AS CategoryName FROM Products p INNER JOIN Categories c ON p.Category_id = c.Id;";
                SqliteCommand command = new SqliteCommand(sql, connection);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    decimal price = reader.GetDecimal(2);
                    string description = reader.GetString(3);
                    byte[] imageData = reader.IsDBNull(4) ? null : (byte[])reader[4];
                    string categoryName = reader.GetString(5);

                    ProductModel produkt = new ProductModel(id, name, price, description, imageData, categoryName);
                    allProducts.Add(produkt);
                }

                connection.Close();
            }

            // Przypisanie elementów DataGrid do kolekcji allProducts
            dgProdukty.ItemsSource = allProducts;
        }


        private void DodajProdukt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNazwaProduktu.Text) ||
                string.IsNullOrWhiteSpace(txtCena.Text) ||
                string.IsNullOrWhiteSpace(txtOpis.Text) ||
                cbKategoria.SelectedItem == null)
            {
                MessageBox.Show("Proszę uzupełnić wszystkie pola przed dodaniem produktu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string nazwa = txtNazwaProduktu.Text;

            // Sprawdzenie, czy produkt o podanej nazwie już istnieje w bazie danych
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM Products WHERE Name = @Nazwa";
                SqliteCommand command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@Nazwa", nazwa);
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Produkt o podanej nazwie już istnieje w bazie danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            decimal cena;
            if (!decimal.TryParse(txtCena.Text, out cena))
            {
                MessageBox.Show("Proszę wprowadzić poprawną wartość ceny.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string opis = txtOpis.Text;
            int idKategorii = ((CategoryModel)cbKategoria.SelectedItem).Id;
            // kod do obsługi dodania zdjęcia

            byte[] imageData = null;
            if (imgProdukt.Source != null)
            {
                imageData = ImageToByte(imgProdukt.Source);
            }
            else
            {
                MessageBox.Show("Nie wybrano zdjęcia produktu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // utworzenie polaczenia 
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                string sql = "INSERT INTO Products (Name, Price, Description, ImageData, Category_id) VALUES (@Nazwa, @Cena, @Opis, @ImageData, @idKategorii)";
                SqliteCommand command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@Nazwa", nazwa);
                command.Parameters.AddWithValue("@Cena", cena);
                command.Parameters.AddWithValue("@Opis", opis);
                command.Parameters.AddWithValue("@idKategorii", idKategorii);
                command.Parameters.AddWithValue("@ImageData", imageData);
                command.ExecuteNonQuery();

                connection.Close();
            }
            MessageBox.Show("Produkt został dodany");
            // resetowanie pól formularza i wybranego zdjęcia
            txtNazwaProduktu.Text = "";
            txtCena.Text = "";
            txtOpis.Text = "";
            cbKategoria.SelectedIndex = -1;

            imgProdukt.Source = null;
            if (imageData != null)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(imageData);
                bitmap.EndInit();
                imgProdukt.Source = bitmap;
            }

        }


        private void btnWybierzZdjecie_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki obrazów (*.jpg;*.jpeg;*.gif;*.png)|*.jpg;*.jpeg;*.gif;*.png|Wszystkie pliki (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.EndInit();
                imgProdukt.Source = bitmap;
            }
        }
        private byte[] ImageToByte(ImageSource imageSource)
        {
            BitmapImage bitmapImage = (BitmapImage)imageSource;
            byte[] imageData;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                imageData = ms.ToArray();
            }
            return imageData;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Usunięcie danych z listy
            allProducts.Clear();

            // Pobranie nowych danych i dodanie ich do listy
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                string sql = "SELECT p.Id, p.Name, p.Price, p.Description, p.ImageData, c.Name AS Category FROM Products p JOIN Categories c ON p.Category_id = c.Id";
                SqliteCommand command = new SqliteCommand(sql, connection);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    decimal price = reader.GetDecimal(2);
                    string description = reader.GetString(3);
                    byte[] imageData = (byte[])reader["ImageData"];
                    string category = reader.GetString(5);

                    ProductModel product = new ProductModel(id, name, price, description, imageData, category);
                    allProducts.Add(product);
                }
                connection.Close();
            }

            // Przypisanie listy do ItemsSource DataGrid
            dgProdukty.ItemsSource = allProducts;

            MessageBox.Show("Dane zostały odświeżone.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // do edycji

        private void WczytajProdukt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdProduktu.Text))
            {
                MessageBox.Show("Proszę wprowadzić numer ID produktu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int idProduktu;
            if (!int.TryParse(txtIdProduktu.Text, out idProduktu))
            {
                MessageBox.Show("Wprowadzony numer ID produktu jest nieprawidłowy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand("SELECT * FROM Products WHERE Id=@Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", idProduktu);
                    SqliteDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        EdittxtNazwaProduktu.Text = reader["Name"].ToString();
                        EdittxtCena.Text = reader["Price"].ToString();
                        EdittxtOpis.Text = reader["Description"].ToString();

                        byte[] imageBytes = (byte[])reader["ImageData"];
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            BitmapImage image = new BitmapImage();
                            using (MemoryStream stream = new MemoryStream(imageBytes))
                            {
                                image.BeginInit();
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.StreamSource = stream;
                                image.EndInit();
                            }
                            EditimgProdukt.Source = image;
                        }
                        else
                        {
                            EditimgProdukt.Source = null;
                        }

                        // Wczytanie kategorii produktu do ComboBox
                        int categoryId = reader.GetInt32(reader.GetOrdinal("Category_id"));
                        // Ustawienie wybranej kategorii w ComboBox
                        EditcbKategoria.SelectedValue = categoryId;
                        EditcbKategoria.SelectedItem = EditcbKategoria.Items.OfType<CategoryModel>().FirstOrDefault(x => x.Id == categoryId);
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono produktu o podanym ID.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    reader.Close();
                }

                connection.Close();
            }
        }

        private void WypelnijComboBoxEdit()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();
                string sql = "SELECT Id, Name FROM Categories";
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    using (SqliteDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            int Id = rdr.GetInt32(0);
                            string Name = rdr.GetString(1);
                            EditcbKategoria.Items.Add(new CategoryModel(Id, Name));
                        }
                    }
                }
                connection.Close();
            }
        }

        private void EditbtnWybierzZdjecie_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki obrazów (*.jpg;*.jpeg;*.gif;*.png)|*.jpg;*.jpeg;*.gif;*.png|Wszystkie pliki (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.EndInit();
                EditimgProdukt.Source = bitmap;
            }
        }

        private void EdytujProdukt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EdittxtNazwaProduktu.Text) || string.IsNullOrWhiteSpace(EdittxtCena.Text) || string.IsNullOrWhiteSpace(EdittxtOpis.Text) || EditcbKategoria.SelectedItem == null)
            {
                MessageBox.Show("Wprowadź wszystkie wymagane dane.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (EditimgProdukt.Source == null)
            {
                MessageBox.Show("Wybierz obrazek produktu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtIdProduktu.Text))
            {
                MessageBox.Show("Proszę wprowadzić numer ID produktu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int idProduktu;
            if (!int.TryParse(txtIdProduktu.Text, out idProduktu))
            {
                MessageBox.Show("Wprowadzony numer ID produktu jest nieprawidłowy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            decimal cena;
            if (!decimal.TryParse(EdittxtCena.Text, out cena))
            {
                MessageBox.Show("Proszę wprowadzić poprawną wartość ceny.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] imageBytes;
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapImage image = (BitmapImage)EditimgProdukt.Source;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                imageBytes = stream.ToArray();
            }

            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand("UPDATE Products SET Name=@Name, Price=@Price, Description=@Description, ImageData=@ImageData, Category_id=@CategoryId WHERE Id=@Id", connection))
                {
                    command.Parameters.AddWithValue("@Name", EdittxtNazwaProduktu.Text);
                    command.Parameters.AddWithValue("@Price", cena);
                    command.Parameters.AddWithValue("@Description", EdittxtOpis.Text);
                    command.Parameters.AddWithValue("@ImageData", imageBytes);
                    command.Parameters.AddWithValue("@CategoryId", ((CategoryModel)EditcbKategoria.SelectedItem).Id);
                    command.Parameters.AddWithValue("@Id", idProduktu);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            MessageBox.Show("Produkt został zaktualizowany.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        //
    }
}

