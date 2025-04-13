using CS665_PizzaRestaurantApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

// TODO: FIX DISPLAYING ITEMS

namespace CS665_PizzaRestaurantApp.Views
{
    public partial class ViewOrders : Window
    {
        private int _selectedCustomerId = 0;
        private int _selectedOrderId = 0;

        public ViewOrders()
        {
            InitializeComponent();
            LoadCustomersWithOrders();
        }

        private void LoadCustomersWithOrders()
        {
            using var context = new ApplicationDbContext();

            var customers = context.CustomerModels
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderDetails)
                .Where(c => c.Orders.Any())
                .AsEnumerable()
                .Select(c => new CustomerOrderSummary
                {
                    CustomerID = c.CustomerID,
                    Name = c.Name,
                    Phone = c.Phone,
                    Email = c.Email,
                    OrderCount = c.Orders.Count,
                })
                .ToList();

            CustomersDataGrid.ItemsSource = customers;
        }

        private void CustomersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is CustomerOrderSummary selectedCustomer)
            {
                _selectedCustomerId = selectedCustomer.CustomerID;
                System.Diagnostics.Debug.WriteLine(_selectedCustomerId);
                LoadCustomerOrders(selectedCustomer.CustomerID);
            }
        }

        private void LoadCustomerOrders(int customerId)
        {
            using var context = new ApplicationDbContext();

            var orders = context.OrderModels
                .Where(o => o.CustomerID == customerId)
                .Include(o => o.OrderDetails)
                .Select(o => new
                {
                    o.OrderID,
                    o.OrderDate,
                    TotalAmount = context.OrderDetailModels
                    .Where(od => od.OrderID == o.OrderID)
                    .Sum(o => o.Quantity * o.UnitPrice),
                })
                .ToList();

            OrdersDataGrid.ItemsSource = orders;
            OrderItemsDataGrid.ItemsSource = null;
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem == null) return;

            dynamic selectedOrder = OrdersDataGrid.SelectedItem;
            _selectedOrderId = selectedOrder.OrderID;
            LoadOrderItems(_selectedOrderId);
        }

        private void LoadOrderItems(int orderId)
        {
            using var context = new ApplicationDbContext();
            var items = context.OrderDetailModels
                .Include(od => od.MenuItem)
                .Where(od => od.OrderID == orderId)
                .Select(od => new OrderItemDisplay
                {
                    OrderDetailID = od.OrderDetailID,
                    Name = od.MenuItem.Name,
                    Price = od.UnitPrice,
                    Quantity = od.Quantity,
                    ImagePath = od.MenuItem.ImagePath,
                })
                .ToList();

            OrderItemsDataGrid.ItemsSource = items;
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                var editWindow = new EditOrderWindow(orderId);
                editWindow.ShowDialog();
                RefreshData();
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                var result = MessageBox.Show("Are you sure you want to delete this order?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using var context = new ApplicationDbContext();
                    using var transaction = context.Database.BeginTransaction();

                    try
                    {
                        var order = context.OrderModels
                            .Include(o => o.OrderDetails)
                            .FirstOrDefault(o => o.OrderID == orderId);

                        if (order != null)
                        {
                            context.OrderDetailModels.RemoveRange(order.OrderDetails);
                            context.OrderModels.Remove(order);
                            context.SaveChanges();
                            transaction.Commit();
                            RefreshData();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error deleting order: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void RemoveOrderItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderDetailId)
            {
                var result = MessageBox.Show("Remove this item from the order?",
                    "Confirm Remove", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using var context = new ApplicationDbContext();
                    using var transaction = context.Database.BeginTransaction();

                    try
                    {
                        var detail = context.OrderDetailModels
                            .Include(od => od.Order)
                            .FirstOrDefault(od => od.OrderDetailID == orderDetailId);

                        if (detail != null)
                        {
                            context.OrderDetailModels.Remove(detail);
                            context.SaveChanges();
                            transaction.Commit();
                            LoadOrderItems(_selectedOrderId);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error removing item: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void RefreshData()
        {
            LoadCustomersWithOrders();
            if (_selectedCustomerId > 0)
            {
                LoadCustomerOrders(_selectedCustomerId);
            }
            if (_selectedOrderId > 0)
            {
                LoadOrderItems(_selectedOrderId);
            }
        }
    }

    // Helper classes for data binding
    public class CustomerOrderSummary
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}