using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS665_PizzaRestaurantApp.Models
{
    class MenuItemModel
    {
        [Key]
        public int ItemID { get; set; }  // Primary Key

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string Description { get; set; }
    }
}
