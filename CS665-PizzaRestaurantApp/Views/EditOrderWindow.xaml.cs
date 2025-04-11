using CS665_PizzaRestaurantApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

// TODO: FIX EDIT ORDER FUNCTIONALITY

namespace CS665_PizzaRestaurantApp.Views
{
    public partial class EditOrderWindow : Window
    {
        private int _orderId;
        private List<OrderItemDisplay> _currentOrderItems = new List<OrderItemDisplay>();
        private List<MenuItemModel> _menuItems = new List<MenuItemModel>();

        public EditOrderWindow(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            LoadOrderData();
            LoadMenuItems();
        }

        private void LoadOrderData()
        {
            using var context = new ApplicationDbContext();
            var order = context.OrderModels
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.MenuItem)
                .FirstOrDefault(o => o.OrderID == _orderId);

            if (order != null)
            {
                // Display order info
                CustomerNameText.Text = order.Customer.Name;
                OrderDateText.Text = order.OrderDate.ToString("MM/dd/yyyy hh:mm tt");
                OrderIdText.Text = order.OrderID.ToString();
                OrderTotalText.Text = order.TotalAmount.ToString("C");

                // Load order items
                _currentOrderItems = order.OrderDetails.Select(od => new OrderItemDisplay
                {
                    OrderDetailID = od.OrderDetailID,
                    ItemID = od.ItemID,
                    Name = od.MenuItem.Name,
                    Price = od.UnitPrice,
                    Quantity = od.Quantity,
                    ImagePath = od.MenuItem.ImagePath,
                    LineTotal = od.Quantity * od.UnitPrice
                }).ToList();

                OrderItemsDataGrid.ItemsSource = _currentOrderItems;
            }
        }

        private void LoadMenuItems()
        {
            using var context = new ApplicationDbContext();
            _menuItems = context.MenuItemModels.ToList();
            MenuItemsControl.ItemsSource = _menuItems;
        }

        private void AddItemToOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int itemId)
            {
                var menuItem = _menuItems.FirstOrDefault(m => m.ItemID == itemId);
                if (menuItem != null)
                {
                    var existingItem = _currentOrderItems.FirstOrDefault(i => i.ItemID == itemId);

                    if (existingItem != null)
                    {
                        existingItem.Quantity++;
                        existingItem.LineTotal = existingItem.Quantity * existingItem.Price;
                    }
                    else
                    {
                        _currentOrderItems.Add(new OrderItemDisplay
                        {
                            ItemID = menuItem.ItemID,
                            Name = menuItem.Name,
                            Price = menuItem.Price,
                            Quantity = 1,
                            ImagePath = menuItem.ImagePath,
                            LineTotal = menuItem.Price
                        });
                    }

                    RefreshOrderItems();
                    UpdateOrderTotal();
                }
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
                    item.LineTotal = item.Quantity * item.Price;
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
                        item.LineTotal = item.Quantity * item.Price;
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
            using var context = new ApplicationDbContext();
            using var transaction = context.Database.BeginTransaction();

            try
            {
                // Get the existing order
                var order = context.OrderModels
                    .Include(o => o.OrderDetails)
                    .FirstOrDefault(o => o.OrderID == _orderId);

                if (order != null)
                {
                    // Remove any deleted items
                    var existingDetails = order.OrderDetails.ToList();
                    foreach (var existingDetail in existingDetails)
                    {
                        if (!_currentOrderItems.Any(i => i.OrderDetailID == existingDetail.OrderDetailID))
                        {
                            context.OrderDetailModels.Remove(existingDetail);
                        }
                    }

                    // Update or add items
                    foreach (var item in _currentOrderItems)
                    {
                        var existingDetail = order.OrderDetails.FirstOrDefault(od => od.OrderDetailID == item.OrderDetailID);

                        if (existingDetail != null)
                        {
                            // Update existing item
                            existingDetail.Quantity = item.Quantity;
                        }
                        else
                        {
                            // Add new item
                            order.OrderDetails.Add(new OrderDetailModel
                            {
                                ItemID = item.ItemID,
                                Quantity = item.Quantity,
                                UnitPrice = item.Price
                            });
                        }
                    }

                    context.SaveChanges();
                    transaction.Commit();

                    MessageBox.Show("Order updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }

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