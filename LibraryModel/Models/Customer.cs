using LibraryModel.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ionescu_Serban_Andrei_Lab2.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public int? CityID { get; set; }
        public City? City { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public DateTime BirthDate { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
