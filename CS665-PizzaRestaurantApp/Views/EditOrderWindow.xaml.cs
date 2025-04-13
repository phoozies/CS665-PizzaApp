using CS665_PizzaRestaurantApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CS665_PizzaRestaurantApp.Views
{
    public partial class EditOrderWindow : Window
    {
        private int _orderId;
        private List<OrderItemDisplay> _currentOrderItems = new List<OrderItemDisplay>();
        private List<MenuItemModel> _menuItems = new List<MenuItemModel>();
        private OrderModel _currentOrder;

        public EditOrderWindow(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            LoadOrderData();
            LoadMenuItems();
        }

        private void LoadOrderData()
        {
            try
            {
                using var context = new ApplicationDbContext();
                _currentOrder = context.OrderModels
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.MenuItem)
                    .FirstOrDefault(o => o.OrderID == _orderId);

                if (_currentOrder == null)
                {
                    MessageBox.Show("Order not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                // Display order info
                CustomerNameText.Text = _currentOrder.Customer?.Name ?? "Unknown";
                OrderDateText.Text = _currentOrder.OrderDate.ToString("MM/dd/yyyy hh:mm tt");
                OrderIdText.Text = _currentOrder.OrderID.ToString();

                // Load existing order items using OrderID
                _currentOrderItems = _currentOrder.OrderDetails?
                    .Where(od => od.OrderID == _currentOrder.OrderID) // Filter by correct OrderID
                    .Select(od => new OrderItemDisplay
                    {
                        OrderDetailID = od.OrderDetailID,
                        ItemID = od.ItemID,
                        Name = od.MenuItem?.Name ?? "Unknown",
                        Price = od.UnitPrice,
                        Quantity = od.Quantity,
                        ImagePath = od.MenuItem?.ImagePath,
                    })
                    .ToList() ?? new List<OrderItemDisplay>();

                OrderItemsDataGrid.ItemsSource = _currentOrderItems;
                UpdateOrderTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void LoadMenuItems()
        {
            try
            {
                using var context = new ApplicationDbContext();
                _menuItems = context.MenuItemModels.ToList();
                MenuItemsControl.ItemsSource = _menuItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading menu items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddItemToOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int itemId)
            {
                var menuItem = _menuItems.FirstOrDefault(m => m.ItemID == itemId);
                if (menuItem == null) return;

                // Check if item already exists in order
                var existingItem = _currentOrderItems.FirstOrDefault(i => i.ItemID == itemId);

                if (existingItem != null)
                {
                    // Increase quantity of existing item
                    existingItem.Quantity++;
                }
                else
                {
                    // Add new item to order
                    _currentOrderItems.Add(new OrderItemDisplay
                    {
                        OrderDetailID = 0, // 0 indicates new item (not saved yet)
                        ItemID = menuItem.ItemID,
                        Name = menuItem.Name,
                        Price = menuItem.Price,
                        Quantity = 1,
                        ImagePath = menuItem.ImagePath,
                    });
                }

                RefreshOrderItems();
                UpdateOrderTotal();
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderDetailId)
            {
                var item = _currentOrderItems.FirstOrDefault(i => i.OrderDetailID == orderDetailId);
                if (item != null)
                {
                    item.Quantity++;
                    RefreshOrderItems();
                    UpdateOrderTotal();
                }
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderDetailId)
            {
                var item = _currentOrderItems.FirstOrDefault(i => i.OrderDetailID == orderDetailId);
                if (item != null)
                {
                    if (item.Quantity > 1)
                    {
                        item.Quantity--;
                    }
                    else
                    {
                        _currentOrderItems.Remove(item);
                    }
                    RefreshOrderItems();
                    UpdateOrderTotal();
                }
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderDetailId)
            {
                var item = _currentOrderItems.FirstOrDefault(i => i.OrderDetailID == orderDetailId);
                if (item != null)
                {
                    _currentOrderItems.Remove(item);
                    RefreshOrderItems();
                    UpdateOrderTotal();
                }
            }
        }

        private void RefreshOrderItems()
        {
            OrderItemsDataGrid.ItemsSource = null;
            OrderItemsDataGrid.ItemsSource = _currentOrderItems;
        }

        private void UpdateOrderTotal()
        {
            decimal total = _currentOrderItems.Sum(i => i.LineTotal);
            OrderTotalText.Text = total.ToString("C");
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var context = new ApplicationDbContext();
                using var transaction = context.Database.BeginTransaction();

                var order = context.OrderModels
                    .Include(o => o.OrderDetails)
                    .FirstOrDefault(o => o.OrderID == _orderId);

                if (order == null)
                {
                    MessageBox.Show("Order not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Process removed items
                var existingDetails = order.OrderDetails.ToList();
                foreach (var existingDetail in existingDetails)
                {
                    if (!_currentOrderItems.Any(i => i.OrderDetailID == existingDetail.OrderDetailID))
                    {
                        context.OrderDetailModels.Remove(existingDetail);
                    }
                }

                // Process updated or new items
                foreach (var item in _currentOrderItems)
                {
                    if (item.OrderDetailID > 0) // Existing item
                    {
                        var existingDetail = order.OrderDetails
                            .FirstOrDefault(od => od.OrderDetailID == item.OrderDetailID && od.OrderID == _orderId);

                        if (existingDetail != null)
                        {
                            existingDetail.Quantity = item.Quantity;
                            existingDetail.UnitPrice = item.Price;
                        }
                    }
                    else // New item
                    {
                        order.OrderDetails.Add(new OrderDetailModel
                        {
                            ItemID = item.ItemID,
                            Quantity = item.Quantity,
                            UnitPrice = item.Price,
                            OrderID = _orderId // Use the correct OrderID here
                        });
                    }
                }

                context.SaveChanges();
                transaction.Commit();

                MessageBox.Show("Order updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class OrderItemDisplay
    {
        public int OrderDetailID { get; set; } // 0 for new items, >0 for existing
        public int ItemID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }
        public decimal LineTotal => Quantity * Price;
    }
}