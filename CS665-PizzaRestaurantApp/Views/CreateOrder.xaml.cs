using CS665_PizzaRestaurantApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace CS665_PizzaRestaurantApp
{
    public partial class CreateOrder : Window
    {
        private List<OrderItem> _currentOrderItems = new List<OrderItem>();

        public CreateOrder()
        {
            InitializeComponent();
            LoadCustomers();
            LoadMenuItems();
            UpdateOrderTotal();
        }

        private void LoadCustomers()
        {
            using var context = new ApplicationDbContext();
            CustomerComboBox.ItemsSource = context.CustomerModels.ToList();
        }

        private void LoadMenuItems()
        {
            using var context = new ApplicationDbContext();
            var items = context.MenuItemModels.ToList();
            MenuItemsControl.ItemsSource = items;
        }

        private void MenuItemImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image && image.Tag is int itemId)
            {
                AddItemToOrder(itemId);
            }
        }

        private void AddToOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int itemId)
            {
                AddItemToOrder(itemId);
            }
        }

        private void AddItemToOrder(int itemId)
        {
            using var context = new ApplicationDbContext();
            var menuItem = context.MenuItemModels.FirstOrDefault(m => m.ItemID == itemId);

            if (menuItem != null)
            {
                var existingItem = _currentOrderItems.FirstOrDefault(i => i.ItemID == itemId);

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    _currentOrderItems.Add(new OrderItem
                    {
                        ItemID = menuItem.ItemID,
                        Name = menuItem.Name,
                        Price = menuItem.Price,
                        ImagePath = menuItem.ImagePath,
                        Quantity = 1
                    });
                }

                RefreshOrderItems();
                UpdateOrderTotal();
            }
        }

        private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int itemId)
            {
                var itemToRemove = _currentOrderItems.FirstOrDefault(i => i.ItemID == itemId);
                if (itemToRemove != null)
                {
                    if (itemToRemove.Quantity > 1)
                    {
                        itemToRemove.Quantity--;
                    }
                    else
                    {
                        _currentOrderItems.Remove(itemToRemove);
                    }

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
            decimal total = _currentOrderItems.Sum(item => item.LineTotal);
            OrderTotalTextBlock.Text = total.ToString("C");
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerComboBox.SelectedItem is not CustomerModel selectedCustomer)
            {
                MessageBox.Show("Please select a customer.", "Incomplete Order",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentOrderItems.Count == 0)
            {
                MessageBox.Show("Please add at least one menu item to the order.", "Incomplete Order",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new ApplicationDbContext();
            using var transaction = context.Database.BeginTransaction();

            try
            {
                // Create new order
                var newOrder = new OrderModel
                {
                    CustomerID = selectedCustomer.CustomerID,
                    OrderDate = DateTime.Now,
                };
                context.OrderModels.Add(newOrder);
                context.SaveChanges();

                // Add all order details
                foreach (var item in _currentOrderItems)
                {
                    var newDetail = new OrderDetailModel
                    {
                        OrderID = newOrder.OrderID,
                        ItemID = item.ItemID,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    };
                    context.OrderDetailModels.Add(newDetail);
                }

                context.SaveChanges();
                transaction.Commit();

                MessageBox.Show($"Order #{newOrder.OrderID} created successfully with {_currentOrderItems.Count} items!",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show($"Error creating order: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    // Helper class for displaying order items
    public class OrderItem
    {
        public int ItemID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => Price * Quantity;
    }
}