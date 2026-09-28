using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace EBIKES09
{
    
    public partial class ProductPage : Page
    {
        private int _categoryId;
        private User _currentUser;
        private List<int?> _promotionProduct;
        private List<int> _intProduct;

        public string ConnectionString { get; set; }
        private List<Product> _allProducts;
        private Bdebikes09Context _context;
        public ProductPage(int categoryId)
        {
            InitializeComponent();
            ConnectionString = null;
            _categoryId = categoryId;
            var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                .UseNpgsql(ConnectionString)
                .Options;
            _context = new Bdebikes09Context();
            IsPromotionFilterCheckBox.Checked += ApplyFilters;
            IsPromotionFilterCheckBox.Unchecked += ApplyFilters;

            LoadProducts();
            LoadBrands();
            LoadRole();

           
            
        }

        private void NoPhoto()
        {
            var newImageUrl = _context.Products.Select(p => p.ImageUrl).FirstOrDefault();
            if (newImageUrl == null)
            {
                
            }
        }
        private void LoadRole()
        {
            
            using (Bdebikes09Context ctx = new Bdebikes09Context())
            {
                _currentUser = ctx.Users.Find(SessionManager.Instance.UserId);
            }
            if (_currentUser != null)
            {
                if (_currentUser.RoleId == 1)
                {
                    AddProductButton.Visibility = Visibility.Visible;
                    EditProductButton.Visibility = Visibility.Visible;
                    PromotionButton.Visibility = Visibility.Visible;
                    ExistingPromotion.Visibility = Visibility.Visible;
                    acceptDelivery.Visibility = Visibility.Visible;

                }
                else
                {
                    AddProductButton.Visibility = Visibility.Collapsed;
                    EditProductButton.Visibility = Visibility.Collapsed;
                    PromotionButton.Visibility = Visibility.Collapsed;
                    ExistingPromotion.Visibility = Visibility.Collapsed;
                    acceptDelivery.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BrandFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters(null, null);
        }
        private void LoadBrands()
        {
            if (_context == null) return;
            List<string> brands = _context.Products
                 .Where(p => p.CategoryId == _categoryId && p.Brand != null && p.IsActive == true)
                .Select(p => p.Brand.BrandName)
                 .Distinct()
                .ToList();
            brands.Insert(0, "Все");
            BrandFilterComboBox.ItemsSource = brands;
            BrandFilterComboBox.SelectedItem = "Все";
        }

        private void LoadProducts()
        {
            if (_context == null)
            {
                MessageBox.Show("Контекст базы данных не инициализирован.");
                return;
            }

            _allProducts = _context.Products
                .Where(p => p.CategoryId == _categoryId && p.IsActive == true)
                .Include(p => p.BikeCharacteristic)
                .Include(p => p.Brand)
                .Include(p => p.PromotionProducts)
                    .ThenInclude(pp => pp.Promotion)
                .Include(p => p.Reviews)
                .ToList();

            if (_allProducts == null || _allProducts.Count == 0)
            {
                MessageBox.Show("Продукты не найдены для категории: " + _categoryId);
                return;
            }

           
            decimal minPrice = (decimal)_allProducts.Min(p => p.Price);
            decimal maxPrice = (decimal)_allProducts.Max(p => p.Price);

            PriceSlider.Minimum = (double)minPrice;
            PriceSlider.Maximum = (double)maxPrice;
            PriceSlider.Value = (double)minPrice;

           
            MinPriceText.Text = minPrice.ToString("N0");
            MaxPriceText.Text = maxPrice.ToString("N0");
            CurrentPriceText.Text = minPrice.ToString("N0");

           
            ProductItemsControl.ItemsSource = _allProducts;
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters(null, null);
        }
      
        private void BrandFilterListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters(null, null);
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters(null, null);
        }

        private void PriceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CurrentPriceText != null)
            {
                CurrentPriceText.Text = PriceSlider.Value.ToString("N0");
                ApplyFilters(null, null);
            }
        }

        private void MenuItemEBike_Click(object sender, RoutedEventArgs e)
        {
            _categoryId = 1;
            LoadProducts();
            LoadBrands();
        }
        private void MenuItemAKB_Click(object sender, RoutedEventArgs e)
        {
            _categoryId = 2;
            LoadProducts();
            LoadBrands();
        }
        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            string selectedBrand = BrandFilterComboBox.SelectedItem as string;
            bool isPromotionFilter = IsPromotionFilterCheckBox.IsChecked ?? false;
            bool isInStockFilter = InStockFilterCheckBox.IsChecked ?? false;
            decimal minPrice = (decimal)PriceSlider.Value;

            IEnumerable<Product> filteredProducts = _allProducts;

            if (!string.IsNullOrWhiteSpace(searchText) && searchText != "введите поисковой запрос . . . ")
            {
                filteredProducts = filteredProducts.Where(p => p.ProductName.ToLower().Contains(searchText));
            }

          
            filteredProducts = filteredProducts.Where(p => p.Price >= minPrice);

            if (selectedBrand != null && selectedBrand != "Все")
            {
                filteredProducts = filteredProducts.Where(p => p.Brand != null && p.Brand.BrandName == selectedBrand);
            }

            if (isPromotionFilter)
            {
                filteredProducts = filteredProducts.Where(p => p.PromotionProducts.Any(pp => pp.Promotion != null && pp.Promotion.EndDate >= DateTime.Now && pp.Promotion.StartDate <= DateTime.Now));
            }

            if (isInStockFilter)
            {
                filteredProducts = filteredProducts.Where(p => p.StockQuantity > 0);
            }

            ProductItemsControl.ItemsSource = filteredProducts.ToList();
        }

        
        private void ButtonProductInf_click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Product product)
            {
                NavigationService.Navigate(new InfProductPage(product.ProductId));
            }
        }



        private User GetLoggedInUser()
        {
            if (SessionManager.Instance.IsLoggedIn)
            {
                return _context.Users.Find(SessionManager.Instance.UserId);
            }
            return null;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddPage addWindow = new AddPage();
            NavigationService.Navigate(addWindow);
        }



        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            EditProductsQuantityPage productWindow = new EditProductsQuantityPage();
            NavigationService.Navigate(productWindow);
        }

        private void ButtonPromotion_Click(object sender, RoutedEventArgs e)
        {
            AddPromotionPage promotionWindow = new AddPromotionPage();
            NavigationService.Navigate(promotionWindow);
        }
        private void ButtonAddToExistingPromotion_Click(object sender, RoutedEventArgs e)
        {
            AddProductsToExistingPromotionPage window = new AddProductsToExistingPromotionPage();
            NavigationService.Navigate(window);
        }
        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }
        private void Window_key(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                ButtonUpdate_Click(sender, e);
            }
        }

        private void ButtonAcceptDelivery_Click(object sender, RoutedEventArgs e)
        {
            PageAcceptDelivery pageAcceptDelivery = new PageAcceptDelivery();
            NavigationService.Navigate(pageAcceptDelivery);
        }

    }
}