using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class AddProductsToExistingPromotionPage : Page
    {
        private Bdebikes09Context _context;
        private List<int> _selectedProductIds;
        private int _selectedPromotionId;

        public AddProductsToExistingPromotionPage()
        {
            InitializeComponent();
            _context = new Bdebikes09Context();
            _selectedProductIds = new List<int>();
            LoadPromotions();
            LoadProducts();
        }

        private void LoadPromotions()
        {
            var promotions = _context.Promotions.ToList();
            PromotionsComboBox.ItemsSource = promotions;
            PromotionsComboBox.SelectedIndex = 0;
        }

        private void LoadProducts()
        {
            var products = _context.Products.ToList();
            ProductsDataGrid.ItemsSource = products;
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters(null, null);
        }

        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();


            IEnumerable<Product> filteredProducts = _context.Products.ToList();


            if (!string.IsNullOrWhiteSpace(searchText) && searchText != "введите поисковой запрос . . . ")
            {
                filteredProducts = filteredProducts.Where(p => p.ProductName.ToLower().Contains(searchText));
            }

            ProductsDataGrid.ItemsSource = filteredProducts.ToList();
        }

        private void ButtonAddProducts_Click(object sender, RoutedEventArgs e)
        {
            if (PromotionsComboBox.SelectedItem is Promotion selectedPromotion)
            {
                _selectedPromotionId = selectedPromotion.PromotionId;
                _selectedProductIds.Clear();

                foreach (var item in ProductsDataGrid.SelectedItems)
                {
                    if (item is Product product)
                    {
                        _selectedProductIds.Add(product.ProductId);
                    }
                }

                try
                {
                    foreach (int productId in _selectedProductIds)
                    {
                        var promotionProduct = new PromotionProduct
                        {
                            ProductId = productId,
                            PromotionId = _selectedPromotionId
                        };
                        _context.PromotionProducts.Add(promotionProduct);
                    }
                    _context.SaveChanges();
                    MessageBox.Show("Товары успешно добавлены к акции!");
                    NavigationService.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите акцию.");
            }
        }
    }
}