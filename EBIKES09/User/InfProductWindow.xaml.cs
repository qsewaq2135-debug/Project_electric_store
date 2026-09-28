using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using EBIKES09.Models;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class InfProductPage : Page
    {
        List<CartItemViewModel> cartItems;
        private User _currentUser;
        private PromotionProduct _promotionProduct;
        public string ConnectionString { get; set; } 
        private Product _product;
        private int _productId;
        private Bdebikes09Context _context;

        public InfProductPage(int productId)
        {
            InitializeComponent();

            ConnectionString = null;
            _productId = productId;
            var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                 .UseNpgsql(ConnectionString)
                  .Options;
            _context = new Bdebikes09Context();
            LoadButton();
            LoadAllFK();
            LoadText();
        }

        

        private void LoadAllFK()
        {
            _product = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.BatteryCharacteristic)
                .Include(p => p.BatteryCharacteristic.BatteryType)
                .Include(p => p.BatteryCharacteristic.Voltage)
                .Include(p => p.BikeCharacteristic)
                 .Include(p => p.BikeCharacteristic.RearBrakes)
                 .Include(p => p.BikeCharacteristic.FrontBrakes)
                .Include(p => p.BikeCharacteristic.FrameMaterial)
                 .Include(p => p.BikeCharacteristic.Drive)
                 .Include(p => p.PromotionProducts)
                 .ThenInclude(pp => pp.Promotion)
                 .FirstOrDefault(p => p.ProductId == _productId);
            if (_product.CategoryId == 1)
            {
                StackPanelE1.Visibility = Visibility.Visible;
                StackPanelE2.Visibility = Visibility.Visible;
            }
            else
            {
                StackPanelE1.Visibility = Visibility.Collapsed;
                StackPanelE2.Visibility = Visibility.Collapsed;
                StackPanelB1.Visibility = Visibility.Visible;
            }
            var rating = _context.Reviews.Where(r => r.ProductId == _productId).Average(r => r.Rating);

            if (rating <= 1.49)
            {
                Star_1.Visibility = Visibility.Visible;
                Star_2.Visibility = Visibility.Collapsed;
                Star_3.Visibility = Visibility.Collapsed;
                Star_4.Visibility = Visibility.Collapsed;
                Star_5.Visibility = Visibility.Collapsed;
            }
            else if (rating <= 2.49)
            {
                Star_1.Visibility = Visibility.Visible;
                Star_2.Visibility = Visibility.Visible;
                Star_3.Visibility = Visibility.Collapsed;
                Star_4.Visibility = Visibility.Collapsed;
                Star_5.Visibility = Visibility.Collapsed;
            }
            else if (rating <= 3.49)
            {
                Star_1.Visibility = Visibility.Visible;
                Star_2.Visibility = Visibility.Visible;
                Star_3.Visibility = Visibility.Visible;
                Star_4.Visibility = Visibility.Collapsed;
                Star_5.Visibility = Visibility.Collapsed;
            }
            else if (rating <= 4.49)
            {
                Star_1.Visibility = Visibility.Visible;
                Star_2.Visibility = Visibility.Visible;
                Star_3.Visibility = Visibility.Visible;
                Star_4.Visibility = Visibility.Visible;
                Star_5.Visibility = Visibility.Collapsed;
            }
            else if (rating <= 4.5)
            {
                Star_1.Visibility = Visibility.Visible;
                Star_2.Visibility = Visibility.Visible;
                Star_3.Visibility = Visibility.Visible;
                Star_4.Visibility = Visibility.Visible;
                Star_5.Visibility = Visibility.Visible;
            }
            else
            {
                Star_1.Visibility = Visibility.Collapsed;
                Star_2.Visibility = Visibility.Collapsed;
                Star_3.Visibility = Visibility.Collapsed;
                Star_4.Visibility = Visibility.Collapsed;
                Star_5.Visibility = Visibility.Collapsed;
                NoStar.Visibility = Visibility.Visible;
            }



                if (_product != null)
            {
                this.DataContext = _product;
                DisplayPromotionInfo(_product);
            
            }
            LoadRole();

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
                    EditeButton.Visibility = Visibility.Visible;
                }
                else
                {
                    EditeButton.Visibility = Visibility.Collapsed;
                }
            }


        }

        private void LoadText()
        {
            
                
                var promotionProducts = _context.PromotionProducts
                    .Where(pp => pp.ProductId == _productId)
                    .ToList();

                
                var currentPromotion = promotionProducts
                    .FirstOrDefault(pp => pp.Promotion != null && pp.Promotion.EndDate >= DateTime.UtcNow);

               
                if (currentPromotion != null)
                {
                    Price_box.TextDecorations = TextDecorations.Strikethrough;
                    Price_box.FontSize = 15;
                }
                else
                {
                    Price_box.TextDecorations = null;
                }
            
        }

        private void LoadButton()
        {
            _product = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.BikeCharacteristic)
                 .Include(p => p.BikeCharacteristic.RearBrakes)
                 .Include(p => p.BikeCharacteristic.FrontBrakes)
                .Include(p => p.BikeCharacteristic.FrameMaterial)
                 .Include(p => p.BikeCharacteristic.Drive)
                 .Include(p => p.PromotionProducts)
                 .ThenInclude(pp => pp.Promotion)
                 .FirstOrDefault(p => p.ProductId == _productId);
            if (_product.IsActive == true && _product.StockQuantity != 0)
            {
                ButtonAddBasket.Visibility = Visibility.Visible;
                ButtonReview.Visibility = Visibility.Visible;
            }
            else
            {
                ButtonReview.Visibility = Visibility.Visible;
                ButtonAddBasket.Visibility = Visibility.Collapsed;
            }
        }
        private void DisplayPromotionInfo(Product product)
        {
            if (product == null) return;
            var activePromotion = product.PromotionProducts
                .Where(pp => pp.Promotion != null && pp.Promotion.EndDate >= DateTime.Now && pp.Promotion.StartDate <= DateTime.Now)
                 .FirstOrDefault()?.Promotion;
            if (activePromotion != null)
            {
                PromotionInfoTextBlock.Text = $"Акция: {activePromotion.PromotionName}\n" +
                 $"Описание: {activePromotion.Description}\n" +
                    $"Скидка: {activePromotion.DiscountValue}";
                DiscountIcon.Visibility = Visibility.Visible;
                DiscountedPriceTextBlock.Text = $"{product.DiscountedPrice:C}";
            }
            else
            {
                PromotionInfoTextBlock.Text = "Нет действующих акций.";
                DiscountedPriceTextBlock.Text = "";
                DiscountIcon.Visibility = Visibility.Collapsed;
            }
        }
        
        private void ButtonAddBasket_click(object sender, RoutedEventArgs e)
        {
            AddToCart(_product.ProductId, 1);
        }
        private void AddToCart(int productId, int quantity)
        {
            if (SessionManager.Instance.IsLoggedIn)
            {
               
                var user = GetLoggedInUser();
                if (user == null)
                {
                    MessageBox.Show("Необходимо войти в систему, чтобы добавить товар.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var cart = _context.Carts.FirstOrDefault(c => c.UserId == user.UserId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = user.UserId,
                        CreatedAt = DateTime.Now
                    };
                    _context.Carts.Add(cart);
                    _context.SaveChanges();
                }

                var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CartId == cart.CartId && ci.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    cartItem = new CartItem
                    {
                        CartId = cart.CartId,
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = _product.DiscountedPrice
                    };
                    _context.CartItems.Add(cartItem);
                }
                _context.SaveChanges();
            }
            else
            {
                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    MessageBox.Show("Товар не найден.");
                    return;
                }
                var sessionId = "temp_session"; 
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = _product.DiscountedPrice,
                    Product = product
                };
                SessionManager.Instance.AddToTemporaryCart("temp_session", cartItem);
                
            }

            MessageBox.Show("Товар добавлен в корзину.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        private User GetLoggedInUser()
        {
            if (SessionManager.Instance.IsLoggedIn)
            {
                return _context.Users.Find(SessionManager.Instance.UserId);
            }
            return null;
        }

       

        private void ButtonEdit_click(object sender, RoutedEventArgs e)
        {
            EditeProductPage editeProductWindow = new EditeProductPage(_productId);
            NavigationService.Navigate(editeProductWindow);
        }
        private void ButtonReview_click(object sender, RoutedEventArgs e)
        {
            ReviewsPage reviewsPage = new ReviewsPage(_productId);
            NavigationService.Navigate(reviewsPage);
        }
    }
}