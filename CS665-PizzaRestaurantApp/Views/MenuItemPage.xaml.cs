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
using System.IO;
using Microsoft.Win32;

namespace CS665_PizzaRestaurantApp.Views
{
    /// <summary>
    /// Interaction logic for MenuItemPage.xaml
    /// </summary>
    public partial class MenuItemPage : Window
    {
        private string selectedImagePath;

        public MenuItemPage()
        {
            InitializeComponent();
            LoadMenuItems();
        }
        private void LoadMenuItems()
        {
            using var context = new ApplicationDbContext();
            MenuItemListBox.ItemsSource = context.MenuItemModels.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                using var context = new ApplicationDbContext();
                var item = new MenuItemModel
                {
                    Name = NameTextBox.Text,
                    Price = price,
                    Description = DescriptionTextBox.Text,
                    ImagePath = selectedImagePath
                };
                context.MenuItemModels.Add(item);
                context.SaveChanges();
                LoadMenuItems();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Please enter a valid price.");
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemListBox.SelectedItem is MenuItemModel selectedItem &&
                decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                using var context = new ApplicationDbContext();
                var item = context.MenuItemModels.Find(selectedItem.ItemID);
                if (item != null)
                {
                    item.Name = NameTextBox.Text;
                    item.Price = price;
                    item.Description = DescriptionTextBox.Text;
                    context.SaveChanges();
                    LoadMenuItems();
                    ClearFields();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemListBox.SelectedItem is MenuItemModel selectedItem)
            {
                using var context = new ApplicationDbContext();
                var item = context.MenuItemModels.Find(selectedItem.ItemID);
                if (item != null)
                {
                    context.MenuItemModels.Remove(item);
                    context.SaveChanges();
                    LoadMenuItems();
                    ClearFields();
                }
            }
        }

        private void MenuItemListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MenuItemListBox.SelectedItem is MenuItemModel selectedItem)
            {
                NameTextBox.Text = selectedItem.Name;
                PriceTextBox.Text = selectedItem.Price.ToString("F2");
                DescriptionTextBox.Text = selectedItem.Description;

                if (!string.IsNullOrWhiteSpace(selectedItem.ImagePath) && File.Exists(selectedItem.ImagePath))
                {
                    MenuItemImage.Source = new BitmapImage(new Uri(selectedItem.ImagePath));
                    selectedImagePath = selectedItem.ImagePath;
                }
                else
                {
                    MenuItemImage.Source = null;
                    selectedImagePath = null;
                }
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Select Menu Item Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string sourcePath = openFileDialog.FileName;
                string imagesDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                // Create Images directory if it doesn't exist
                if (!Directory.Exists(imagesDirectory))
                    Directory.CreateDirectory(imagesDirectory);

                // Copy image to local folder with a unique filename
                string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(sourcePath);
                string destPath = System.IO.Path.Combine(imagesDirectory, fileName);
                File.Copy(sourcePath, destPath, true);

                // Update path reference and image preview
                selectedImagePath = destPath;
                MenuItemImage.Source = new BitmapImage(new Uri(destPath));
            }
        }

        private void ClearFields()
        {
            NameTextBox.Text = "";
            PriceTextBox.Text = "";
            DescriptionTextBox.Text = "";
            MenuItemListBox.SelectedItem = null;
        }
    }
}
