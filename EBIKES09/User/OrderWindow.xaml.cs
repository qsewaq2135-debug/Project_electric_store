using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class OrderPage : Page
    {
        private readonly Bdebikes09Context _context;

        public OrderPage(Bdebikes09Context context)
        {
            InitializeComponent();
            _context = new Bdebikes09Context();
            LoadUserData();

            ComboBoxPay.ItemsSource = DatabaseControl.GetPayMethod();
        }
        private void LoadUserData()
        {
            var user = GetLoggedInUser();
            if (user != null)
            {


                string lastName = $"{user.LastName}";
                string firstName = $"{user.FirstName}";
                string telNo = $"{user.PhoneNumber}";
                string email = $"{user.Email}";

                FirstName_box.Text = firstName;
                LastName_box.Text = lastName;
                TelNo_Box.Text = telNo;
                Email_box.Text = email;
            }
            else
            {
                MessageBox.Show($"Вы вошли как незарегистрированный пользователь. Пожалуйста, заполните все поля, чтобы мы могли связаться с вами и обработать вашу заявку.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                StackGUID.Visibility = Visibility.Visible;
            }

        }

        private void ButtonGuid_Click(object sender, RoutedEventArgs e)
        {
            var enterGuid = GuidBox.Text;
            var GUID = _context.Users.FirstOrDefault(x => x.Guid == enterGuid);
            if (GUID != null)
            {
                string lastNameG = $"{GUID.LastName}";
                string firstNameG = $"{GUID.FirstName}";
                string telNoG = $"{GUID.PhoneNumber}";
                string emailG = $"{GUID.Email}";

                FirstName_box.Text = firstNameG;
                LastName_box.Text = lastNameG;
                TelNo_Box.Text = telNoG;
                Email_box.Text = emailG;

                MessageBox.Show($"Мы нашли ваш GUID! Удостоверьтесь, что все данные верны с прошлого заказа, и выберите способ оплаты, а так же укажите коментарий, если необходимо...");
            }
            else
            {
                MessageBox.Show("Неверный GUID! попробуйте еще раз...");
            }

        }
        private User GetLoggedInUser()
        {
            if (SessionManager.Instance.IsLoggedIn)
            {
                var userId = SessionManager.Instance.UserId;
                return _context.Users.Find(userId);
            }
            return null;
        }

        private void CheckedPersonal(object sender, RoutedEventArgs e)
        {
            ButtonEnter.Visibility = Visibility.Visible;
        }

        private void UnCheckedPersonal(object sender, RoutedEventArgs e)
        { ButtonEnter.Visibility = Visibility.Hidden;}


        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(FirstName_box.Text) || string.IsNullOrWhiteSpace(LastName_box.Text) ||
                string.IsNullOrWhiteSpace(TelNo_Box.Text) || string.IsNullOrWhiteSpace(Email_box.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string payType = ComboBoxPay.SelectedItem as string;
            try
            {
                
                var guid = Guid.NewGuid().ToString();
                string name = FirstName_box.Text;
                string lastName = LastName_box.Text;
                string phone = TelNo_Box.Text;
                string email = Email_box.Text;
                string paymentType = payType;
                string comment = description.Text;

                AddOrderToDatabase(name, lastName, phone, email, paymentType, comment, guid);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EnsureCartExists(int userId)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                _context.Carts.Add(new Cart { UserId = userId });
                _context.SaveChanges();
            }
        }


        private void AddOrderToDatabase(string name, string lastName, string phone, string email, string paymentType, string comment, string guid )
        {
            if (SessionManager.Instance.IsLoggedIn)
            {
                
                var loggedUser = GetLoggedInUser();
                if (loggedUser == null)
                {
                    MessageBox.Show("Пользователь не вошел в систему.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService.GoBack();
                    return;
                }

                int userIdFromSession = SessionManager.Instance.UserId;
                EnsureCartExists(userIdFromSession);

              
                var userToUpdate = _context.Users.Find(userIdFromSession);
                if (userToUpdate != null)
                {
                    if (userToUpdate.FirstName != name) userToUpdate.FirstName = name;
                    if (userToUpdate.LastName != lastName) userToUpdate.LastName = lastName;
                    if (userToUpdate.PhoneNumber != phone) userToUpdate.PhoneNumber = phone;
                    if (userToUpdate.Email != email) userToUpdate.Email = email;
                    _context.SaveChanges();
                }

               
                var paymentTypeRecord = _context.Payments.FirstOrDefault(pt => pt.PaymentName == paymentType);
                if (paymentTypeRecord == null)
                {
                    throw new Exception($"Тип оплаты '{paymentType}' не найден.");
                }

               
                var newOrder = new Order
                {
                    UserId = userIdFromSession,
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    StatusId = 1, 
                    TotalAmount = CalculateTotalAmount(userIdFromSession),
                    PaymentId = paymentTypeRecord.PaymentId,
                    Discription = comment
                };

                _context.Orders.Add(newOrder);
                _context.SaveChanges();

               
                AddOrderItemsToDatabase(newOrder.OrderId, userIdFromSession);

                
                ClearBasket(userIdFromSession);

                MessageBox.Show("Заказ успешно оформлен!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            else
            {
                try
                {
                    
                    var sessionId = Guid.NewGuid().ToString(); 
                var cartItems = SessionManager.Instance.GetTemporaryCart("temp_session");
                
                        var newguid  = GuidBox.Text;
                    var newGUID = _context.Users.FirstOrDefault(u => u.Guid == newguid);

                    if (newGUID == null)
                    {
                      
                        var newUser = new User
                        {
                            FirstName = name,
                            LastName = lastName,
                            PhoneNumber = phone,
                            Email = email,
                            RoleId = 4,
                            Guid = guid, 

                        };
                        _context.Users.Add(newUser);
                        _context.SaveChanges();


                       
                        var newOrder = new Order
                        {
                            UserId = newUser.UserId,
                            OrderDate = DateOnly.FromDateTime(DateTime.Now),
                            StatusId = 1,
                            TotalAmount = cartItems.Sum(ci => (ci.UnitPrice ?? 0) * ci.Quantity),
                            PaymentId = _context.Payments.FirstOrDefault(pt => pt.PaymentName == paymentType)?.PaymentId ?? 1,
                            Discription = comment
                        };

                        _context.Orders.Add(newOrder);
                        _context.SaveChanges();


                        
                        foreach (var cartItem in cartItems)
                        {
                            var product = _context.Products.Find(cartItem.ProductId);
                            if (product != null)
                            {
                                if (product.StockQuantity >= cartItem.Quantity)
                                {
                                    product.StockQuantity -= cartItem.Quantity;
                                }
                                else
                                {
                                    MessageBox.Show($"Недостаточно товара на складе: {product.ProductName}. Доступно: {product.StockQuantity}, запрошено: {cartItem.Quantity}", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                            }

                            var orderItem = new OrderDetil
                            {
                                OrderId = newOrder.OrderId,
                                ProductId = cartItem.ProductId,
                                Quantity = cartItem.Quantity,
                                UnitPrice = cartItem.UnitPrice ?? 0
                            };
                            _context.OrderDetils.Add(orderItem);
                        }

                        _context.SaveChanges();

                        
                        SessionManager.Instance.ClearTemporaryCart(sessionId);

                        CostomeMessageBox costomeMessageBox = new CostomeMessageBox($"Заказ успешно оформлен! Ваш уникальный идентификатор (GUID): {guid}, и ваш номер заказа: {newOrder.OrderId}. Мы также отправили его на вашу почту, чтобы вы могли легко использовать его для будущих заказов. Просто введите этот GUID в специальное поле при следующем оформлении. Что бы скопировать данные, нажмите кнопку \"Копировать\".", "Сообщение", $"GUID: {guid}. Номер заказа: {newOrder.OrderId}", true);
                        bool? dialogeResult = costomeMessageBox.ShowDialog();


                        if (dialogeResult == true)
                        {
                            NavigationService.GoBack();
                        }
                        
                    }
                    else if (newGUID != null)
                    {
                      
                        var newOrder = new Order
                        {
                            UserId = newGUID.UserId,
                            OrderDate = DateOnly.FromDateTime(DateTime.Now),
                            StatusId = 1, 
                            TotalAmount = cartItems.Sum(ci => (ci.UnitPrice ?? 0) * ci.Quantity),
                            PaymentId = _context.Payments.FirstOrDefault(pt => pt.PaymentName == paymentType)?.PaymentId ?? 1,
                            Discription = comment
                        };

                        _context.Orders.Add(newOrder);
                        _context.SaveChanges();


                       
                        foreach (var cartItem in cartItems)
                        {
                            var product = _context.Products.Find(cartItem.ProductId);
                            if (product != null)
                            {
                                if (product.StockQuantity >= cartItem.Quantity)
                                {
                                    product.StockQuantity -= cartItem.Quantity;
                                }
                                else
                                {
                                    MessageBox.Show($"Недостаточно товара на складе: {product.ProductName}. Доступно: {product.StockQuantity}, запрошено: {cartItem.Quantity}");
                                    return;
                                }
                            }

                            var orderItem = new OrderDetil
                            {
                                OrderId = newOrder.OrderId,
                                ProductId = cartItem.ProductId,
                                Quantity = cartItem.Quantity,
                                UnitPrice = cartItem.UnitPrice ?? 0
                            };
                            _context.OrderDetils.Add(orderItem);
                        }

                        _context.SaveChanges();

                       
                        SessionManager.Instance.ClearTemporaryCart("temp_session");

                        MessageBox.Show($"Заказ успешно оформлен! Ваш уникальный идентификатор (GUID): {guid}. Мы также отправили его на вашу почту, чтобы вы могли легко использовать его для будущих заказов. Просто введите этот GUID в специальное поле при следующем оформлении.");
                        NavigationService.GoBack();
                    }

                }
                catch (Exception ex)
                {
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}" +
                            $"Тип исключения: {ex.GetType().Name}" +
                            $"Трассировка стека: {ex.StackTrace}"
                            , "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void PrivacyPolicyLink_Click(object sender, EventArgs e)
        {
            string privacyPolicyUrl = "https://disk.yandex.ru/i/htUv3vPUKQ-7Bg";

            try
            {
                Process.Start(new ProcessStartInfo(privacyPolicyUrl) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть ссылку: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddOrderItemsToDatabase(int orderId, int userId)
        {
            var cartItems = _context.CartItems
                .Include(ci => ci.Product) 
                .Where(ci => ci.Cart.UserId == userId)
                .ToList();

            foreach (var cartItem in cartItems)
            {
              
                var product = _context.Products.Find(cartItem.ProductId);
                if (product != null)
                {
                    if (product.StockQuantity >= cartItem.Quantity)
                    {
                        product.StockQuantity -= cartItem.Quantity; 
                    }
                    else
                    {
                        MessageBox.Show($"Недостаточно товара на складе: {product.ProductName}. Доступно: {product.StockQuantity}, запрошено: {cartItem.Quantity}");
                        return;
                    }
                }

                
                var orderItem = new OrderDetil
                {
                    OrderId = orderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice ?? 0
                };
                _context.OrderDetils.Add(orderItem);
            }

            _context.SaveChanges();
        }

        private decimal CalculateTotalAmount(int userId)
        {
            return _context.CartItems
                .Where(ci => ci.Cart.UserId == userId)
                .Sum(ci => (ci.UnitPrice ?? 0) * ci.Quantity);
        }

        private void ClearBasket(int userId)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(_context.CartItems.Where(ci => ci.CartId == cart.CartId));
                _context.SaveChanges();
            }
            else
            {
                MessageBox.Show("Не работает");
            }
        }
    }
}