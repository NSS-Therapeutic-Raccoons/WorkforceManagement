﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<PaymentType> Payments { get; set; } = new List<PaymentType>();

        public List<Product> Products { get; set; } = new List<Product>();

    }
}
