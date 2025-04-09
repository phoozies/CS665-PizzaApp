using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CS665_PizzaRestaurantApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void CreateOrder_Click(object sender, RoutedEventArgs e)
    {
        var createWindow = new CreateOrder(); // Make sure you use the correct namespace
        createWindow.ShowDialog();
    }

    private void ViewOrders_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("This would show order list - not implemented yet.");
    }

    private void ManageCustomers_Click(object sender, RoutedEventArgs e)
    {
        var customerWindow = new Views.CustomerPage();
        customerWindow.ShowDialog();
    }

    private void ManageMenuItems_Click(object sender, RoutedEventArgs e)
    {
        var menuItemWindow = new Views.MenuItemPage(); // Adjust namespace if needed
        menuItemWindow.ShowDialog(); // Or .Show() if you want it modeless
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

}