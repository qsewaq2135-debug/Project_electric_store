using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class EditProductsQuantityPage : Page
    {
        private Bdebikes09Context _context;
        private List<Product> _allProducts;

        public EditProductsQuantityPage()
        {
            InitializeComponent();
            var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                 .UseNpgsql()
                 .Options;
            _context = new Bdebikes09Context(dbContextOptions);
            LoadProducts();
        }

        private void LoadProducts()
        {
            if (_context == null) return;
            _allProducts = _context.Products.Include(p => p.Category).ToList();
            ProductsDataGrid.ItemsSource = _allProducts;
        }

        private void ButtonSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductsDataGrid.ItemsSource is List<Product> products)
                {
                    foreach (var product in products)
                    {
                        if (_context.Entry(product).State == EntityState.Modified)
                        {
                            _context.Products.Update(product);
                        }
                    }
                    _context.SaveChanges();
                    MessageBox.Show("Изменения сохранены.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения изменений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters(null, null);
        }

        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            

            IEnumerable<Product> filteredProducts = _allProducts;


            if (!string.IsNullOrWhiteSpace(searchText) && searchText != "введите поисковой запрос . . . ")
            {
                filteredProducts = filteredProducts.Where(p => p.ProductName.ToLower().Contains(searchText));
            }

            ProductsDataGrid.ItemsSource = filteredProducts.ToList();
        }
        private void ButtonDropProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                var selectedProduct = (sender as Button)?.DataContext as Product;

                if (selectedProduct != null)
                {
                   
                    var result = MessageBox.Show(
                        $"Вы точно хотите удалить товар '{selectedProduct.ProductName}'?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                   
                    if (result == MessageBoxResult.Yes)
                    {
                       
                        _context.Products.Remove(selectedProduct);
                        _context.SaveChanges();

                        
                        _allProducts.Remove(selectedProduct);
                        ProductsDataGrid.ItemsSource = null;
                        ProductsDataGrid.ItemsSource = _allProducts;

                        MessageBox.Show("Товар успешно удален.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка удаления товара: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nВнутренняя ошибка: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}