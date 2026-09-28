using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EBIKES09.Models;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;

namespace EBIKES09
{
    public partial class BasketPage : Page
    {
        private ObservableCollection<CartItemViewModel> items;
        private CartItemViewModel model;
        private int _userId;
        private ObservableCollection<CartItem> _cartItems;

        public string ConnectionString { get; set; }
        private User _currentUser;
        private int _cartId;
        private Bdebikes09Context _dbContext;

        public BasketPage( int userId)
        {
            items = new ObservableCollection<CartItemViewModel>(); 
            InitializeComponent();
            
            _dbContext = new Bdebikes09Context();
            LoadCurrentUser();
            LoadCartData();

            this.DataContext = this;
        }
        private void LoadCurrentUser()
        {
            if (SessionManager.Instance.IsLoggedIn)
            {
                
                _currentUser = _dbContext.Users.Find(SessionManager.Instance.UserId);
            }
            else
            {
                
                _currentUser = null; 
            }

            UpdateBasketDataGrid();
        }
        public void ClearTemporaryUsers()
        {
            var temporaryUsers = _dbContext.Users
                .Where(u => u.Email.StartsWith("tempuser_"))
                .ToList();

            _dbContext.Users.RemoveRange(temporaryUsers);
            _dbContext.SaveChanges();
        }

        private void UpdateBasketDataGrid()
        {
            if (SessionManager.Instance.IsLoggedIn)
            {
               
                BasketGuestDataGrid.Visibility = Visibility.Collapsed;
                BasketDataGrid.Visibility = Visibility.Visible;

                var cart = _dbContext.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(c => c.UserId == _currentUser.UserId);

                if (cart != null)
                {
                    _cartId = cart.CartId;
                    BasketDataGrid.ItemsSource = cart.CartItems.ToList();
                    UpdateTotalPrice(cart);
                }
                else
                {
                    BasketDataGrid.ItemsSource = new List<CartItem>();
                    TotalPrice_Box.Text = $"Итого: 0";
                }
            }
            else
            {
                
                BasketGuestDataGrid.Visibility = Visibility.Visible;
                BasketDataGrid.Visibility = Visibility.Collapsed;

                
                var cartItems = SessionManager.Instance.GetTemporaryCart("temp_session");

                if (cartItems == null || !cartItems.Any())
                {
                    
                    return;
                }

                
                items.Clear();
                foreach (var item in cartItems)
                {
                    

                    var product = GetProductById(item.ProductId);
                    if (product == null)
                    {
                        continue;
                    }

                    items.Add(new CartItemViewModel
                    {
                        ProductId = item.ProductId,
                        ProductName = product.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        ImageUrl = product.ImageUrl
                    });
                }

               
                BasketGuestDataGrid.ItemsSource = items;

               
                UpdateTotalPrice(cartItems);
            }
        }

        private Product GetProductById(int productId)
        {
            using (var context = new Bdebikes09Context())
            {
                return context.Products
                    .Include(p => p.Brand) 
                    .Include(p => p.Category) 
                    .FirstOrDefault(p => p.ProductId == productId);
            }
        }
        private void UpdateTotalPrice(Cart cart)
        {
            decimal totalPrice = cart.CartItems.Sum(item => (item.UnitPrice ?? 0) * item.Quantity);
            TotalPrice_Box.Text = $"Итого: {totalPrice:C}";
        }
        public void AddToBasket(Product product)
        {
            var cart = _dbContext.Carts
                .FirstOrDefault(c => c.UserId == _currentUser.UserId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = _currentUser.UserId,
                    CreatedAt = DateTime.Now
                };
                _dbContext.Carts.Add(cart);
                _dbContext.SaveChanges();
            }
            var cartItem = new CartItem()
            {
                CartId = cart.CartId,
                ProductId = product.ProductId,
                Quantity = 1,
                UnitPrice = product.Price
            };

            _dbContext.CartItems.Add(cartItem);
            _dbContext.SaveChanges();
            UpdateBasketDataGrid();

        }
        
        
        private void ClearBasket_Click(object sender, RoutedEventArgs e)
        {

            var cart = _dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == _currentUser.UserId);
            if (cart != null)
            {
                _dbContext.CartItems.RemoveRange(cart.CartItems);
                _dbContext.SaveChanges();
            }

            UpdateBasketDataGrid();
        }
        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            
            if (BasketDataGrid.Items.Count == 0 && BasketGuestDataGrid.Items.Count == 0)
            {
                
                return;
            }
            using (Bdebikes09Context ctx = new Bdebikes09Context())
            {
                OrderPage checkoutWindow = new OrderPage(ctx);
                NavigationService.Navigate(checkoutWindow);
            }
        }
        private void LoadCartData()
        {
            if (SessionManager.Instance.IsLoggedIn)
            {
               
                var cart = _dbContext.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(c => c.UserId == _currentUser.UserId);

                if (cart != null)
                {
                    _cartId = cart.CartId;
                    BasketDataGrid.ItemsSource = cart.CartItems.ToList();
                    UpdateTotalPrice(cart);
                }
                else
                {
                    BasketDataGrid.ItemsSource = new List<CartItem>();
                    TotalPrice_Box.Text = $"Итого: 0";
                }
            }
            else
            {
               
                var sessionId = "temp_session";
                var cartItems = SessionManager.Instance.GetTemporaryCart(sessionId);

                
                items.Clear();
                foreach (var item in cartItems)
                {
                    var product = GetProductById(item.ProductId);
                    if (product != null)
                    {
                        items.Add(new CartItemViewModel
                        {
                            ProductId = item.ProductId,
                            ProductName = product.ProductName,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            ImageUrl = product.ImageUrl
                        });
                    }
                }

                BasketGuestDataGrid.ItemsSource = items; 
                UpdateTotalPrice(cartItems); 
            }
        }

        private void UpdateTotalPrice(List<CartItem> cartItems)
        {
            decimal totalPrice = cartItems.Sum(item => (item.UnitPrice ?? 0) * item.Quantity);
            TotalPrice_Box.Text = $"Итого: {totalPrice:C}";
        }



        private void DeleteCartItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is CartItem cartItem)
            {

                
                if (cartItem.Quantity >= 2)
                {
                    cartItem.Quantity = cartItem.Quantity + -1;
                    _dbContext.SaveChanges();
                    UpdateBasketDataGrid();
                }
                else if (cartItem.Quantity <=1)
                {
                    _dbContext.CartItems.Remove(cartItem);
                    _dbContext.SaveChanges();
                    UpdateBasketDataGrid();
                    
                }
            }
        }
        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.Instance.IsLoggedIn)
            {
               
                if (sender is Button button && button.CommandParameter is CartItem cartItem)
                {
                    var stockQuvantitu = _dbContext.Products.Find(cartItem.ProductId);
                    if (cartItem.Quantity < stockQuvantitu.StockQuantity)
                    {
                        cartItem.Quantity += 1;
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show("Товара больше нет в наличии", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    
                }
            }
            else
            {
               
                var selectedItem = BasketGuestDataGrid.SelectedItem as CartItemViewModel;
                if (selectedItem != null)
                {
                    var stockQuvantitu = _dbContext.Products.Find(selectedItem.ProductId);
                    if (selectedItem.Quantity < stockQuvantitu.StockQuantity)
                    {
                        selectedItem.Quantity += 1;
                    }
                    else
                    {
                        MessageBox.Show("Товара больше нет в наличии", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                   

                   
                    var sessionId = "temp_session"; 
                    SessionManager.Instance.UpdateTemporaryCart(sessionId, items.Select(i => new CartItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList());
                }
            }

            
            UpdateBasketDataGrid();
           
        }


        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
           
            if (SessionManager.Instance.IsLoggedIn)
            {

               
                if (sender is Button button && button.CommandParameter is CartItem cartItem)
                {
                    
                     if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity -= 1; 
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        _dbContext.CartItems.Remove(cartItem); 
                        _dbContext.SaveChanges();
                    }
                }
            }
            else
            {
                
                var selectedItem = BasketGuestDataGrid.SelectedItem as CartItemViewModel;
                if (selectedItem != null)
                {
                    if (selectedItem.Quantity > 1)
                    {
                        selectedItem.Quantity -= 1; 
                    }
                    else
                    {
                        items.Remove(selectedItem); 
                    }

                    
                    var sessionId = "temp_session"; 
                    SessionManager.Instance.UpdateTemporaryCart(sessionId, items.Select(i => new CartItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList());
                }
            }

            UpdateBasketDataGrid();
            LoadCartData();
        }


        private void ButtonCompany_Click(object sender, RoutedEventArgs e)
        {
            CompanuPage companyWindow = new CompanuPage();
            NavigationService.Navigate(companyWindow);
        }
        private void ButtonKontackt_Click(object sender, RoutedEventArgs e)
        {
            ContactPage contactWindow = new ContactPage();
            NavigationService.Navigate(contactWindow);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}