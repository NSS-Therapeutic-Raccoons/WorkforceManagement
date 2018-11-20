//view for computer including datetime to correspond with database type

using System;


namespace BangazonWorkforce.Models
{
    public class Computer
    {

        public int Id { get; set; }

        public DateTime PurchaseDate { get; set; }

        public DateTime? DecomissionDate { get; set; }

        public string Manufacturer { get; set; }

        public string Make { get; set; }



    }
}
