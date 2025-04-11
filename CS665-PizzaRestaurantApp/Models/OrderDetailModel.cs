using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CS665_PizzaRestaurantApp.Models
{
    class OrderDetailModel
    {
        [Key]
        public int OrderDetailID { get; set; }  // Primary Key

        [Required]
        [ForeignKey("Order")]
        public int OrderID { get; set; }  // Foreign Key linking to Order

        [Required]
        [ForeignKey("MenuItem")]
        public int ItemID { get; set; }  // Foreign Key linking to MenuItem

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }  // Price at the time of order

        public virtual OrderModel Order { get; set; }
        public virtual MenuItemModel MenuItem { get; set; }

        // Calculated property
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
