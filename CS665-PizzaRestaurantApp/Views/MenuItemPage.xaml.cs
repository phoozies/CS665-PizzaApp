using CS665_PizzaRestaurantApp.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace CS665_PizzaRestaurantApp.Views
{
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
            MenuItemsDataGrid.ItemsSource = context.MenuItemModels.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Please enter a valid price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please enter a name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemsDataGrid.SelectedItem is MenuItemModel selectedItem)
            {
                if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
                {
                    MessageBox.Show("Please enter a valid price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Please enter a name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var context = new ApplicationDbContext();
                var item = context.MenuItemModels.Find(selectedItem.ItemID);
                if (item != null)
                {
                    item.Name = NameTextBox.Text;
                    item.Price = price;
                    item.Description = DescriptionTextBox.Text;
                    item.ImagePath = selectedImagePath;
                    context.SaveChanges();
                    LoadMenuItems();
                    ClearFields();
                }
            }
            else
            {
                MessageBox.Show("Please select a menu item to update.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemsDataGrid.SelectedItem is MenuItemModel selectedItem)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {selectedItem.Name}?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using var context = new ApplicationDbContext();
                    var item = context.MenuItemModels.Find(selectedItem.ItemID);
                    if (item != null)
                    {
                        // Delete the associated image file if it exists
                        if (!string.IsNullOrEmpty(item.ImagePath) && File.Exists(item.ImagePath))
                        {
                            try
                            {
                                File.Delete(item.ImagePath);
                            }
                            catch { /* Ignore if file deletion fails */ }
                        }

                        context.MenuItemModels.Remove(item);
                        context.SaveChanges();
                        LoadMenuItems();
                        ClearFields();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a menu item to delete.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuItemsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuItemsDataGrid.SelectedItem is MenuItemModel selectedItem)
            {
                NameTextBox.Text = selectedItem.Name;
                PriceTextBox.Text = selectedItem.Price.ToString("F2");
                DescriptionTextBox.Text = selectedItem.Description;
                selectedImagePath = selectedItem.ImagePath;

                if (!string.IsNullOrWhiteSpace(selectedItem.ImagePath) && File.Exists(selectedItem.ImagePath))
                {
                    MenuItemImage.Source = new BitmapImage(new Uri(selectedItem.ImagePath));
                }
                else
                {
                    MenuItemImage.Source = null;
                }
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Select Menu Item Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string sourcePath = openFileDialog.FileName;
                string imagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                // Create Images directory if it doesn't exist
                if (!Directory.Exists(imagesDirectory))
                    Directory.CreateDirectory(imagesDirectory);

                // Copy image to local folder with a unique filename
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(sourcePath);
                string destPath = Path.Combine(imagesDirectory, fileName);
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
            MenuItemImage.Source = null;
            selectedImagePath = null;
            MenuItemsDataGrid.SelectedItem = null;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = "";
            PriceTextBox.Text = "";
            DescriptionTextBox.Text = "";
            MenuItemImage.Source = null;
            selectedImagePath = null;
            MenuItemsDataGrid.SelectedItem = null;
        }
    }
}