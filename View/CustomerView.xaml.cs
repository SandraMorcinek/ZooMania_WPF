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

namespace ZooMania.View
{
    /// <summary>
    /// Logika interakcji dla klasy CustomerView.xaml
    /// </summary>
    public partial class CustomerView : UserControl
    {
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        private List<CustomerModel> allCustomers = new List<CustomerModel>();
        public CustomerView()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WyswietlKlientow();
        }

        private void WyswietlKlientow()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                string sql = "SELECT Id, First_name, Last_name, Email, Address, Postal_code, City, Country FROM Customers";
                SqliteCommand command = new SqliteCommand(sql, connection);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string firstName = reader.GetString(1);
                    string lastName = reader.GetString(2);
                    string email = reader.GetString(3);
                    string address = reader.GetString(4);
                    string postalCode = reader.GetString(5);
                    string city = reader.GetString(6);
                    string country = reader.GetString(7);

                    CustomerModel klient = new CustomerModel(id, firstName, lastName, email, address, postalCode, city, country);
                    allCustomers.Add(klient);
                }

                connection.Close();
            }

            dgKlienci.ItemsSource = allCustomers;
        }

        // filtrowanie klientow
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<CustomerModel> filteredList = new List<CustomerModel>();
            string searchValue = tbSearch.Text.ToLower();

            foreach (CustomerModel customer in allCustomers)
            {
                if (customer.Id.ToString().ToLower().Contains(searchValue) ||
                    customer.First_name.ToLower().Contains(searchValue) ||
                    customer.Last_name.ToLower().Contains(searchValue) ||
                    customer.Email.ToLower().Contains(searchValue) ||
                    customer.Address.ToLower().Contains(searchValue) ||
                    customer.Postal_code.ToLower().Contains(searchValue) ||
                    customer.City.ToLower().Contains(searchValue) ||
                    customer.Country.ToLower().Contains(searchValue))
                {
                    filteredList.Add(customer);
                }
            }
            dgKlienci.ItemsSource = filteredList;
        }



        //
    }
}
