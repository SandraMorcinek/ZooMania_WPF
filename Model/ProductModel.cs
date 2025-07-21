using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ZooMania.Model
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        //
        public int Quantity { get; set; }
        public byte[] ImageData { get; set; }

        public ProductModel(int id, string name, decimal price, string description, byte[] imageData, string categoryName)
        {
            Id = id;
            Name = name;
            Price = price;
            Description = description;
            ImageData = imageData;
            //CategoryId = categoryId;
            CategoryName = categoryName;
            
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
