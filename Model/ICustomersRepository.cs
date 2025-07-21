using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooMania.Model
{
    public interface ICustomersRepository
    {
        void Add(CustomerModel customerModel);
        void Update(CustomerModel customerModel);
        void Delete(int id);
        IEnumerable<CustomerModel> GetAll();
        CustomerModel GetById(int id);
        IEnumerable<CustomerModel> GetCustomersByLastName(string lastName);
    }
}
