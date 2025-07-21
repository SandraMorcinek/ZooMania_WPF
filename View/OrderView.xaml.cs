using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Logika interakcji dla klasy OrderView.xaml
    /// </summary>
    public partial class OrderView : UserControl
    {
        private string LokalizacjaBazy = "Data Source=ZooManiaDB.sqlite";
        List<OrderModel> allOrders = new List<OrderModel>(); // Lista do przechowywania danych zamówień
        public OrderView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WyswietlZamowienia();
        }

        private void WyswietlZamowienia()
        {
            using (SqliteConnection connection = new SqliteConnection(LokalizacjaBazy))
            {
                connection.Open();

                string sql = "SELECT Orders.Id, Customers.First_name, Customers.Last_name, Customers.Email, Products.Name as Product_name, Products.Price as Product_Price, strftime('%d.%m.%Y %H:%M:%S', Orders.Order_date) as Order_date, Orders.Shipping_date as Shipping_date, Orders.Status, Orders.Transport  " +
                             "FROM Orders INNER JOIN Customers ON Orders.Customer_id = Customers.Id INNER JOIN Products ON Orders.Product_id = Products.Id";
                SqliteCommand command = new SqliteCommand(sql, connection);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int orderId = reader.GetInt32(0);
                    string firstName = reader.GetString(1);
                    string lastName = reader.GetString(2);
                    string email = reader.GetString(3);
                    string productName = reader.GetString(4);
                    decimal productPrice = reader.GetDecimal(5);
                    DateTime orderDate = DateTime.ParseExact(reader.GetString(6), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);// wyswietlenie z godz
                    DateTime shoppingDate = reader.GetDateTime(7);
                    string status = reader.GetString(8);
                    int transportCost = reader.GetInt32(9);
                    decimal toPay = productPrice + transportCost;

                    OrderModel order = new OrderModel(orderId, firstName, lastName, email, productName, productPrice, orderDate, shoppingDate, status, transportCost, toPay);
                    allOrders.Add(order);
                }

                connection.Close(); 
            }

            dgZamowienia.ItemsSource = allOrders; // Przypisanie danych zamówień do DataGrid
        }


        // filtrowanie zamowien
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<OrderModel> filteredList = new List<OrderModel>();
            string searchValue = tbSearch.Text.ToLower();

            foreach (OrderModel order in allOrders)
            {
                if (order.Id.ToString().ToLower().Contains(searchValue) ||
                    order.FirstName.ToLower().Contains(searchValue) ||
                    order.LastName.ToLower().Contains(searchValue) ||
                    order.Email.ToLower().Contains(searchValue) ||
                    order.ProductName.ToLower().Contains(searchValue) ||
                    order.ProductPrice.ToString().ToLower().Contains(searchValue) ||
                    order.OrderDate.ToString("dd-MM-yyyy HH:mm:ss").ToLower().Contains(searchValue) ||
                    order.ShoppingDate.ToString("dd-MM-yyyy").ToLower().Contains(searchValue) ||
                    order.Status.ToLower().Contains(searchValue) ||
                    order.TransportCost.ToString().ToLower().Contains(searchValue) ||
                    order.ToPay.ToString().ToLower().Contains(searchValue))
                {
                    filteredList.Add(order);
                }
            }
            dgZamowienia.ItemsSource = filteredList;
        }


    }
}
