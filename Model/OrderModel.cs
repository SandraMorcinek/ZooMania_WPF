using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooMania.Model
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShoppingDate { get; set; }
        public string Status { get; set; }
        public int TransportCost { get; set; }
        public decimal ToPay { get; set; }

        public OrderModel(int orderId, string firstName, string lastName, string email, string productName, decimal productPrice, DateTime orderDate, DateTime shoppingDate, string status, int transportCost, decimal toPay)
        {
            Id = orderId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            ProductName = productName;
            ProductPrice = productPrice;
            OrderDate = orderDate;
            ShoppingDate = shoppingDate;
            Status = status;
            TransportCost = transportCost;
            ToPay = toPay;
        }
    }
}
