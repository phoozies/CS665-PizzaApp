﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS665_PizzaRestaurantApp.Models
{
    public class OrderItemDisplay
    {
        public int OrderDetailID { get; set; }
        public int ItemID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }
        public decimal LineTotal { get; set; }
    }
}
