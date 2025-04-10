using CS665_PizzaRestaurantApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CS665_PizzaRestaurantApp.Views
{
    /// <summary>
    /// Interaction logic for ViewOrders.xaml
    /// </summary>
    public partial class ViewOrders : Window
    {
        public ViewOrders()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            using var context = new ApplicationDbContext();
            var orders = context.OrderModels
                .Include(o => o.Customer)
                .Select(o => new
                {
                    o.OrderID,
                    CustomerName = o.Customer.Name,
                    o.OrderDate,
                    o.TotalAmount
                })
                .ToList();

            OrdersDataGrid.ItemsSource = orders;
        }

        private void OrdersDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem == null) return;

            dynamic selectedOrder = OrdersDataGrid.SelectedItem;
            int orderId = selectedOrder.OrderID;

            using var context = new ApplicationDbContext();
            var details = context.OrderDetailModels
                .Include(od => od.MenuItem)
                .Where(od => od.OrderID == orderId)
                .Select(od => new
                {
                    ItemName = od.MenuItem.Name,
                    od.Quantity,
                    od.UnitPrice,
                    Total = od.Quantity * od.UnitPrice
                })
                .ToList();

            OrderDetailsDataGrid.ItemsSource = details;
        }
    }
}
