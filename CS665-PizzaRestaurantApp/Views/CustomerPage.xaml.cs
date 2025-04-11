using CS665_PizzaRestaurantApp.Models;
using System.Windows;
using System.Linq;

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
            CustomerDataGrid.ItemsSource = context.CustomerModels.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please enter a name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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
            if (CustomerDataGrid.SelectedItem is CustomerModel selected)
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Please enter a name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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
            else
            {
                MessageBox.Show("Please select a customer to update.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is CustomerModel selected)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {selected.Name}?", "Confirm Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
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
            else
            {
                MessageBox.Show("Please select a customer to delete.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CustomerDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is CustomerModel selected)
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
            CustomerDataGrid.SelectedItem = null;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
    }
}