using CS665_PizzaRestaurantApp.Models;
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
    /// Interaction logic for CustomerPage.xaml
    /// </summary>
    public partial class CustomerPage : Window
    {
        public CustomerPage()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            using var context = new ApplicationDbContext();
            CustomerListBox.ItemsSource = context.CustomerModels.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            using var context = new ApplicationDbContext();
            var newCustomer = new CustomerModel
            {
                Name = NameTextBox.Text,
                Phone = PhoneTextBox.Text,
                Email = EmailTextBox.Text,
                Address = AddressTextBox.Text
            };
            context.CustomerModels.Add(newCustomer);
            context.SaveChanges();
            LoadCustomers();
            ClearForm();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerListBox.SelectedItem is CustomerModel selected)
            {
                using var context = new ApplicationDbContext();
                var customer = context.CustomerModels.Find(selected.CustomerID);
                if (customer != null)
                {
                    customer.Name = NameTextBox.Text;
                    customer.Phone = PhoneTextBox.Text;
                    customer.Email = EmailTextBox.Text;
                    customer.Address = AddressTextBox.Text;
                    context.SaveChanges();
                    LoadCustomers();
                    ClearForm();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerListBox.SelectedItem is CustomerModel selected)
            {
                using var context = new ApplicationDbContext();
                var customer = context.CustomerModels.Find(selected.CustomerID);
                if (customer != null)
                {
                    context.CustomerModels.Remove(customer);
                    context.SaveChanges();
                    LoadCustomers();
                    ClearForm();
                }
            }
        }

        private void CustomerListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CustomerListBox.SelectedItem is CustomerModel selected)
            {
                NameTextBox.Text = selected.Name;
                PhoneTextBox.Text = selected.Phone;
                EmailTextBox.Text = selected.Email;
                AddressTextBox.Text = selected.Address;
            }
        }

        private void ClearForm()
        {
            NameTextBox.Text = "";
            PhoneTextBox.Text = "";
            EmailTextBox.Text = "";
            AddressTextBox.Text = "";
            CustomerListBox.SelectedItem = null;
        }
    }
}
