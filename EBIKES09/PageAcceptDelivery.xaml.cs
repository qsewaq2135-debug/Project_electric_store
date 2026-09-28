using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EBIKES09
{
    /// <summary>
    /// Логика взаимодействия для PageAcceptDelivery.xaml
    /// </summary>
    public partial class PageAcceptDelivery : Page
    {
        private Bdebikes09Context _context;
        private List<ProductDeliveryViewModel> _products;

        public class ProductDeliveryViewModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string CategoryName { get; set; }
            public string BrandName { get; set; }
            public decimal? Price { get; set; }
            public int? CurrentQuantity { get; set; } 
            public int AddedQuantity { get; set; } 
            public string ImageUrl { get; set; }
            public bool? IsActive { get; set; }
        }

        public PageAcceptDelivery()
        {
            InitializeComponent();
            _context = new Bdebikes09Context();
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                if (_context == null) return;

                // Загружаем все товары с категориями и брендами
                _products = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Select(p => new ProductDeliveryViewModel
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        CategoryName = p.Category != null ? p.Category.CategoryName : "Без категории",
                        BrandName = p.Brand != null ? p.Brand.BrandName : "Без бренда",
                        Price = p.Price,
                        CurrentQuantity = p.StockQuantity,
                        AddedQuantity = 0, // По умолчанию 0
                        ImageUrl = p.ImageUrl,
                        IsActive = p.IsActive
                    })
                    .OrderBy(p => p.CategoryName)
                    .ThenBy(p => p.BrandName)
                    .ThenBy(p => p.ProductName)
                    .ToList();

                ProductsDataGrid.ItemsSource = _products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                var selectedProducts = _products
                    .Where(p => p.AddedQuantity > 0) 
                    .Select(p => $"{p.ProductName}: {p.AddedQuantity} шт")
                    .ToList();

                
                if (!selectedProducts.Any())
                {
                    MessageBox.Show("Вы не выбрали ни одного товара для добавления.", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                
                string confirmationMessage =
                    "Вы выбрали следующие товары:\n\n" +
                    string.Join("\n", selectedProducts) +
                    "\n\nПодтвердить добавление?";

              
                MessageBoxResult result = MessageBox.Show(
                    confirmationMessage,
                    "Подтверждение поставки",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

               
                if (result != MessageBoxResult.Yes)
                {
                    MessageBox.Show("Добавление товаров отменено.", "Отмена",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

               
                bool hasChanges = false;

                foreach (var productVm in _products.Where(p => p.AddedQuantity > 0))
                {
                    var product = _context.Products.Find(productVm.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity = (product.StockQuantity ?? 0) + productVm.AddedQuantity;
                        _context.Entry(product).State = EntityState.Modified;
                        hasChanges = true;
                    }
                }

                if (hasChanges)
                {
                    _context.SaveChanges();
                    MessageBox.Show("Товары успешно добавлены в базу данных.", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProducts(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            try
            {
                string searchText = SearchTextBox.Text.ToLower();

                IEnumerable<ProductDeliveryViewModel> filteredProducts = _products;

                if (!string.IsNullOrWhiteSpace(searchText) && searchText != "введите поисковой запрос . . . ")
                {
                    filteredProducts = filteredProducts.Where(p =>
                        (p.ProductName?.ToLower().Contains(searchText) ?? false) ||
                        (p.CategoryName?.ToLower().Contains(searchText) ?? false) ||
                        (p.BrandName?.ToLower().Contains(searchText) ?? false));
                }

                ProductsDataGrid.ItemsSource = filteredProducts.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации товаров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "введите поисковой запрос . . . ")
            {
                SearchTextBox.Text = "";
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "введите поисковой запрос . . . ";
            }
        }
    }
}

