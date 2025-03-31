using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS665_PizzaRestaurantApp.Models
{
    class OrderModel
    {
        [Key]
        public int OrderID { get; set; }  // Primary Key

        [Required]
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }  // Foreign Key linking to Customer

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; } // Auto-calculated from OrderDetails

        public virtual CustomerModel Customer { get; set; } // Navigation Property
    }
}
