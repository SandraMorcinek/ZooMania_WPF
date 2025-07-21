using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooMania.Model
{
    class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductCount { get; set; }

        public CategoryModel(int id, string nazwa)
        {
            Id = id;
            Name = nazwa;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
