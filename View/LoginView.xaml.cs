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
    /// Logika interakcji dla klasy LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";

        public LoginView()
        {
            InitializeComponent();

            // utworzenie polaczenia 
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                string createTableQuery = "CREATE TABLE IF NOT EXISTS [User] (Id INTEGER PRIMARY KEY, Username NVARCHAR(50) UNIQUE NOT NULL, [Password] NVARCHAR(100) NOT NULL, [Name] NVARCHAR(50) NOT NULL, LastName NVARCHAR(50) NOT NULL,Email NVARCHAR(100) UNIQUE NOT NULL, ProfilePicture BLOB);";

                SqliteCommand commandTocreateTable = new SqliteCommand(createTableQuery, connection);
                commandTocreateTable.ExecuteNonQuery();

                // Dodawanie danych do tabeli User
                string insertQuery1 = "INSERT OR IGNORE INTO [User] (Username, [Password], [Name], LastName, Email) VALUES ('admin', 'admin', 'Sandra', 'Morcinek', 'smorcinek@gmail.com')";
                string insertQuery2 = "INSERT OR IGNORE INTO [User] (Username, [Password], [Name], LastName, Email) VALUES ('jan_kowalski', 'haslo123', 'Jan', 'Kowalski', 'jan.kowalski@gmail.com')";
                string insertQuery3 = "INSERT OR IGNORE INTO [User] (Username, [Password], [Name], LastName, Email) VALUES ('anna_nowak', 'abc123', 'Anna', 'Nowak', 'anna.nowak@gmail.com')";

                using (SqliteCommand command = new SqliteCommand(insertQuery1, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (SqliteCommand command = new SqliteCommand(insertQuery2, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (SqliteCommand command = new SqliteCommand(insertQuery3, connection))
                {
                    command.ExecuteNonQuery();
                }

                // tworzenie tabeli Klienci
                string createTableQuery2 = "CREATE TABLE IF NOT EXISTS Customers (Id INTEGER PRIMARY KEY, First_name VARCHAR(50), Last_name VARCHAR(50), Email VARCHAR(50) UNIQUE, Address VARCHAR(100), Postal_code VARCHAR(20), City VARCHAR(50), Country VARCHAR(50));";
                // tworzenie tabeli Zamowienia
                string createTableQuery3 = "CREATE TABLE IF NOT EXISTS Orders (Id INTEGER PRIMARY KEY, Customer_id INTEGER, Product_id INTEGER, Order_date DATE, Shipping_date DATE, Status VARCHAR(20), Transport INTEGER, FOREIGN KEY (Customer_Id) REFERENCES Customers(Id), FOREIGN KEY (Product_Id) REFERENCES Products(Id));";
                // tworzenie tabeli Kategorie
                string createTableQuery4 = "CREATE TABLE IF NOT EXISTS Categories (Id INTEGER PRIMARY KEY, Name VARCHAR(50));";
                // tworzenie tabeli Produkty
                string createTableQuery5 = "CREATE TABLE IF NOT EXISTS Products (Id INTEGER PRIMARY KEY, Name VARCHAR(50), Price DECIMAL(10,2), Description VARCHAR(500), ImageData BLOB, Category_id INT, FOREIGN KEY (Category_Id) REFERENCES Categories(Id));";
                // tworzenie tabeli Magazyn
                string createTableQuery6 = "CREATE TABLE IF NOT EXISTS Storage (Product_id INTEGER, Quantity INTEGER, FOREIGN KEY (Product_Id) REFERENCES Products(Id));";
                // tworzenie tabeli Finanse
                string createTableQuery7 = "CREATE TABLE IF NOT EXISTS Finances (Id INTEGER PRIMARY KEY, Revenue DECIMAL(10,2), Expenses DECIMAL(10,2), Profit DECIMAL(10,2));";
                //string createTableQuery8 = "CREATE TABLE IF NOT EXISTS ";

                using (SqliteCommand command = new SqliteCommand(createTableQuery2, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (SqliteCommand command = new SqliteCommand(createTableQuery3, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (SqliteCommand command = new SqliteCommand(createTableQuery4, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (SqliteCommand command = new SqliteCommand(createTableQuery5, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (SqliteCommand command = new SqliteCommand(createTableQuery6, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (SqliteCommand command = new SqliteCommand(createTableQuery7, connection))
                {
                    command.ExecuteNonQuery();
                }

                // dodanie klientow do tabeli Customers
                string insertQueryCustomer = "INSERT OR IGNORE INTO Customers (First_name, Last_name, Email, Address, Postal_code, City, Country) VALUES ('Anna', 'Kowalska', 'anna.kowalska@gmail.com', 'ul. Mokotowska 12', '00-001', 'Warszawa', 'Poland'), ('Jan', 'Nowak', 'jan.nowak@gmail.com', 'ul. Nowowiejska 8', '00-002', 'Warszawa', 'Poland'), ('Magdalena', 'Duda', 'magda.duda@gmail.com', 'ul. Tadeusza Rejtana 15', '30-901', 'Kraków', 'Poland'), ('Tomasz', 'Kaczmarek', 'tomasz.kaczmarek@gmail.com', 'ul. Kazimierza Wielkiego 10', '50-077', 'Wrocław', 'Poland'), ('Alicja', 'Szymańska', 'alicja.sz@gmail.com', 'ul. Podwale 7', '50-449', 'Wrocław', 'Poland'), ('Piotr', 'Wójcik', 'piotr.wojcik@gmail.com', 'ul. Sienkiewicza 3', '90-001', 'Łódź', 'Poland'),('Katarzyna', 'Adamczyk', 'kasia.adamczyk@gmail.com', 'ul. Piłsudskiego 20', '50-001', 'Wrocław', 'Poland'),('Marcin', 'Lewandowski', 'marcin.lew@gmail.com', 'ul. Piotrkowska 128', '90-001', 'Łódź', 'Poland'),('Karolina', 'Majewska', 'karolina.majewska@gmail.com', 'ul. Mickiewicza 12', '00-678', 'Warszawa', 'Poland'),('Michał', 'Nowicki', 'michal.nowicki@gmail.com', 'ul. Grunwaldzka 10', '80-001', 'Gdańsk', 'Poland'),('Ewa', 'Kaczmarczyk', 'ewa.kaczmarczyk@gmail.com', 'ul. Podgórska 4', '30-000', 'Kraków', 'Poland'),('Wojciech', 'Sobczyk', 'wojciech.sobczyk@gmail.com', 'ul. Aleje Jerozolimskie 98', '00-999', 'Warszawa', 'Poland'),('Aleksandra', 'Pawlak', 'ala.pawlak@gmail.com', 'ul. Grzybowska 43', '00-001', 'Warszawa', 'Poland'),('Tadeusz', 'Kwiatkowski', 'tadeusz.kwiatkowski@gmail.com', 'ul. Aleja Armii Krajowej 2', '80-001', 'Gdańsk', 'Poland'),('Dominika', 'Krajewska', 'dominika.krajewska@gmail.com', 'ul. Dębowa 65', '60-001', 'Poznań','Poland'),('Thomas', 'Schmidt', 'thomas.schmidt@gmail.com', 'Burgstraße 5', '10115', 'Berlin', 'Germany'),('Katrin', 'Müller', 'katrin.mueller@gmail.com', 'Prenzlauer Allee 55', '10405', 'Berlin', 'Germany'),('Jan', 'Novák', 'jan.novak@gmail.com', 'Václavské náměstí 4', '110 00', 'Prague', 'Czech Republic'),('Petra', 'Svobodová', 'petra.svobodova@gmail.com', 'Karla Engliše 15', '150 00', 'Prague', 'Czech Republic');";

                using (SqliteCommand command = new SqliteCommand(insertQueryCustomer, connection))
                {
                    command.ExecuteNonQuery();
                }
                
                // dodanie kategorii do tabeli Categories
                string InsertQueryCategory = @" INSERT INTO Categories (Name) SELECT @Name WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = @Name);";

                using (SqliteCommand command = new SqliteCommand(InsertQueryCategory, connection))
                {
                    command.Parameters.AddWithValue("@Name", "Karma dla psów");
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Name", "Karma dla kotów");
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }


        }

        // przesuwanie okna myszka
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimalizuj_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ResetPasswordButton_Click()
        {
            // Otwórz nowe okno resetowania hasła
            ResetPasswordWindow resetPasswordWindow = new ResetPasswordWindow();
            resetPasswordWindow.Show();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResetPasswordButton_Click();
        }
    }
}
