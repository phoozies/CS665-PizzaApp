using CS665_PizzaRestaurantApp.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CS665_PizzaRestaurantApp
{
    public partial class CreateOrder : Window
    {
        private decimal _unitPrice = 0;

        public CreateOrder()
        {
            InitializeComponent();
            LoadCustomers();
            LoadMenuItems();
        }

        private void LoadCustomers()
        {
            using var context = new ApplicationDbContext();
            // TODO: FIX CONNECTION
            var customers = context.CustomerModels.ToList();
            CustomerComboBox.ItemsSource = customers;
        }

        private void LoadMenuItems()
        {
            using var context = new ApplicationDbContext();
            var items = context.MenuItemModels.ToList();
            MenuItemComboBox.ItemsSource = items;
        }

        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotal();
        }

        private void MenuItemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuItemComboBox.SelectedItem is MenuItemModel selectedItem)
            {
                _unitPrice = selectedItem.Price;
                UpdateTotal();
            }
            else
            {
                _unitPrice = 0;
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            if (int.TryParse(QuantityTextBox.Text, out int quantity) && quantity > 0)
            {
                decimal total = _unitPrice * quantity;
                TotalTextBlock.Text = $"${total:F2}";
            }
            else
            {
                TotalTextBlock.Text = "$0.00";
            }
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerComboBox.SelectedItem is not CustomerModel selectedCustomer ||
                MenuItemComboBox.SelectedItem is not MenuItemModel selectedItem ||
                !int.TryParse(QuantityTextBox.Text, out int quantity) ||
                quantity <= 0)
            {
                MessageBox.Show("Please fill in all fields correctly.");
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
                    TotalAmount = _unitPrice * quantity
                };
                context.OrderModels.Add(newOrder);
                context.SaveChanges();

                // Create order detail
                var newDetail = new OrderDetailModel
                {
                    OrderID = newOrder.OrderID,
                    ItemID = selectedItem.ItemID,
                    Quantity = quantity,
                    UnitPrice = _unitPrice
                };
                context.OrderDetailModels.Add(newDetail);
                context.SaveChanges();

                transaction.Commit();

                MessageBox.Show("Order created successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show("Error creating order: " + ex.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
