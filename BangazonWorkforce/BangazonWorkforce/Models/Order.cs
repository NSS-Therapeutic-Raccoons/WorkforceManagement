using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int? PaymentTypeId { get; set; }

        public Customer Customer { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
