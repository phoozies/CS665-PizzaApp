using CS665_PizzaRestaurantApp.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace CS665_PizzaRestaurantApp
{
    public partial class CreateOrder : Window
    {
        private decimal _unitPrice = 0;
        private int _selectedItemId = 0;

        public CreateOrder()
        {
            InitializeComponent();
            LoadCustomers();
            LoadMenuItems();
            UpdateTotal();
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
                var selectedItem = ((MenuItemModel)image.DataContext);
                _selectedItemId = itemId;
                _unitPrice = selectedItem.Price;
                SelectedItemText.Text = selectedItem.Name;
                UpdateTotal();
            }
        }

        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            // Ensure controls exist
            if (QuantityTextBox == null || TotalTextBlock == null)
                return;

            if (int.TryParse(QuantityTextBox.Text, out int quantity) &&
                quantity > 0 &&
                _selectedItemId > 0)
            {
                decimal total = _unitPrice * quantity;
                TotalTextBlock.Text = total.ToString("C");
            }
            else
            {
                TotalTextBlock.Text = "$0.00";
            }
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerComboBox.SelectedItem is not CustomerModel selectedCustomer ||
                _selectedItemId == 0 ||
                !int.TryParse(QuantityTextBox.Text, out int quantity) ||
                quantity <= 0)
            {
                MessageBox.Show("Please select a customer, menu item, and enter valid quantity.",
                    "Incomplete Order", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                // Create order detail
                var newDetail = new OrderDetailModel
                {
                    OrderID = newOrder.OrderID,
                    ItemID = _selectedItemId,
                    Quantity = quantity,
                    UnitPrice = _unitPrice
                };
                context.OrderDetailModels.Add(newDetail);
                context.SaveChanges();

                transaction.Commit();

                MessageBox.Show($"Order #{newOrder.OrderID} created successfully!",
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
}