using Microsoft.EntityFrameworkCore;
using System.Windows;
using EBIKES09.Models;
using MahApps.Metro.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Navigation;
using System.Text.RegularExpressions;
using System.Windows.Media.Animation;

namespace EBIKES09
{
    public class NavigationItem
    {
        public string Label { get; set; }
        public Type PageType { get; set; } 
        public string Glyph { get; set; }
        public object Icon { get; set; } 
    }

    public partial class MainWindow : MetroWindow
    {
        bool isExpanded = false;
        private User _currentUser;
        private Bdebikes09Context _dbContext; 
        public ICommand NavigateCommand { get; set; }
        public MainWindow(Bdebikes09Context dbContext)
        {
            InitializeComponent();
            _dbContext = new Bdebikes09Context();
            MainFrame.Navigate(new ProductPage(1));
            SearchOrderVoid();
        }
        public async void SearchOrderVoid()
        {
            _currentUser = await _dbContext.Users.FindAsync(SessionManager.Instance.UserId);
            if (_currentUser != null)
            {
                SearchOrder.Visibility = _currentUser.UserId == 3 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                SearchOrder.Visibility = Visibility.Visible;
            }
        }
        private void ButtonUser_click(object sender, RoutedEventArgs e)
        {
            _currentUser = _dbContext.Users.Find(SessionManager.Instance.UserId);
            if (SessionManager.Instance.IsLoggedIn)
            {
                UserProfilePage userProfilWindow = new UserProfilePage(_currentUser);
                MainFrame.Navigate(userProfilWindow);
                LoginPopup.IsOpen = false;
            }
            else
            {
                LoginTextBox.Text = null;
                PasswordBox.Password = null;

                FirstName_box.Text = null;
                LastName_box.Text = null;
                TelNo_box.Text = null;
                Email_box.Text = null;
                Login_box.Text = null;
                Password_box1.Password = null;
                Password_box.Password = null;
                LoginPopup.IsOpen = true;
            }
        }
        private void ButtonEBike_click(object sender, RoutedEventArgs e)
        {
                OpenProductWindow(1);
        }
        private void TextBlockReg_click(object sender, RoutedEventArgs e)
        {
            LoginPopup.IsOpen = false;
            RegGuidPopup.IsOpen = false;
            RegPopup.IsOpen = true;
        }

        private void TextBlockGuidReg_click(Object sender, RoutedEventArgs e)
        {
            LoginPopup.IsOpen = false;
            RegPopup.IsOpen = false;
            RegGuidPopup.IsOpen = true;
        }

        private void ButtonAKB_click(object sender, RoutedEventArgs e)
        {
            OpenProductWindow(2);
        }
        private void OpenProductWindow(int categoryId)
        {
            LoadingPage loadingPage = new LoadingPage();
            MainFrame.Navigate(loadingPage);
            try
            {
                var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                    .UseNpgsql("Server=localhost;Port=5432;Database=ebikes09;User Id=postgres;Password=admin")
                    .Options;

                using (Bdebikes09Context ctx = new Bdebikes09Context(dbContextOptions))
                {
                    MainFrame.Navigate(new ProductPage(categoryId));
                }
            }
            catch {}
        }
        private void ButtonBasket_click(object sender, RoutedEventArgs e)
        {
            int userId = SessionManager.Instance.UserId; 
                BasketPage basketPage = new BasketPage(userId); 
                MainFrame.Navigate(basketPage);
        }
        private void ButtonCompany_Click(object sender, RoutedEventArgs e)
        {
            CompanuPage companyWindow = new CompanuPage();
            MainFrame.Navigate(companyWindow);
        }

        private void ButtonKontackt_Click(object sender, RoutedEventArgs e)
        {
            ContactPage contactWindow = new ContactPage();
            MainFrame.Navigate (contactWindow);
        }

        private void ButtonSearchOrder_Click(object sender, RoutedEventArgs e)
        {
                Storyboard sb = (Storyboard)this.FindResource("ShowPopup");
                sb.Begin(this);
                SearchOrderPopup.IsOpen = true;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string guidText = GUIDNumberTextBox.Text.Trim();
            string orderNumberText = OrderNumberTextBox.Text.Trim();

            if (string.IsNullOrEmpty(guidText) || string.IsNullOrEmpty(orderNumberText))
            {
                OrderInfoTextBlock.Text = "Пожалуйста, введите GUID заказа и номер заказа.";
                return;
            }

            if (!Guid.TryParse(guidText, out Guid guid))
            {
                OrderInfoTextBlock.Text = "Пожалуйста, введите GUID заказа в правильном формате.";
                return;
            }

            if (!int.TryParse(orderNumberText, out int orderNumber))
            {
                OrderInfoTextBlock.Text = "Пожалуйста, введите номер заказа в числовом формате.";
                return;
            }

           
            var order = _dbContext.Orders.FirstOrDefault(o => o.OrderId == orderNumber);

            if (order != null)
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.UserId == order.UserId);

                if (user != null)
                {
                   
                    if (user.Guid == guidText) 
                    {
                        var stingStatus = _dbContext.Statuses.FirstOrDefault(o => o.StatusId == order.StatusId);
                        if (stingStatus != null)
                        {
                            OrderInfoTextBlock.Text = $"Номер заказа: {order.OrderId}\n" +
                                                      $"Статус заказа: {stingStatus.StatusName}\n" +
                                                      $"Сумма заказа: {order.TotalAmount}\n" +
                                                      $"Аддрес магазина: Улица Бабушкина, 45, Екатеринбург, Свердловская область, 620017";
                        }
                        else
                        {
                            OrderInfoTextBlock.Text = "Не удалось получить статус заказа.";
                        }
                    }
                    else
                    {
                        OrderInfoTextBlock.Text = "Неверный GUID или номер заказа. Пожалуйста, проверьте введенные данные и попробуйте еще раз.";
                    }
                }
                else
                {
                    OrderInfoTextBlock.Text = "Не удалось найти пользователя, связанного с этим заказом.";
                }
            }
            else
            {
                OrderInfoTextBlock.Text = "Заказ не найден. Пожалуйста, проверьте введенные GUID и номер заказа и попробуйте еще раз.";
            }
        }

        private void ButtonBack_click(object sender, EventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }
        
        private void ButtonPopupEnter_click(object sender, EventArgs e)
        {
            var enter_login = LoginTextBox.Text.ToString();
            string bd_login = LoginTextBox.Text;
            var enter_password = PasswordBox.Password.ToString();
            string bd_password = PasswordBox.Password;

            var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                       .UseNpgsql()
            .Options;

            using (Bdebikes09Context context = new Bdebikes09Context(dbContextOptions))
            {
                var userAuth = context.UserAuths
                                .FirstOrDefault(u => u.Password == bd_password && u.Login == bd_login);
                if (string.IsNullOrEmpty(LoginTextBox.Text) && string.IsNullOrEmpty(PasswordBox.Password))
                {
                    LogPopup.Text = "Ошибка пустые поля!";
                    LogPopup.Visibility = Visibility.Visible;
                    return;
                }

                if (userAuth != null)
                {

                    var user = context.Users.FirstOrDefault(u => u.UserId == userAuth.UserId);

                    if (user != null)
                    {

                        SessionManager.Instance.Login(user.UserId, $"{user.FirstName} {user.LastName}");
                        _currentUser = _dbContext.Users.Find(SessionManager.Instance.UserId);
                        UserProfilePage userProfilePage = new UserProfilePage(_currentUser);
                        MainFrame.Navigate(userProfilePage);

                        SearchOrderVoid();
                        LoginPopup.IsOpen = false;
                        LogPopup.Text = null;
                    }
                    else
                    {
                        LogPopup.Text = "Ошибка пользователя не найден!";
                        LogPopup.Visibility = Visibility.Visible;
                        return;
                    }
                }
                else
                {
                    LogPopup.Text = "Неверный логин или пароль!";
                    LogPopup.Visibility = Visibility.Visible;
                    return;
                }
            }
        }
        private void ButtonPopupExit_click(object sender, EventArgs e)
        {
            LoginPopup.IsOpen = false;
        }
        private void ButtonPopupReg_click(object sender, EventArgs e)
        {
            try
            {
                List<string> errors = new List<string>();


                if (string.IsNullOrWhiteSpace(FirstName_box.Text))
                    errors.Add("Поле 'Имя' не должно быть пустым.");
                if (string.IsNullOrWhiteSpace(LastName_box.Text))
                    errors.Add("Поле 'Фамилия' не должно быть пустым.");
                if (string.IsNullOrWhiteSpace(Email_box.Text))
                    errors.Add("Поле 'Email' не должно быть пустым.");
                if (string.IsNullOrWhiteSpace(TelNo_box.Text))
                    errors.Add("Поле 'Телефон' не должно быть пустым.");
                if (string.IsNullOrWhiteSpace(Login_box.Text))
                    errors.Add("Поле 'Логин' не должно быть пустым.");
                if (Password_box.Password.Length == 0)
                {
                    errors.Add("Поле 'Повторный пароль' не должно быть пустым.");
                }
                if (Password_box1.Password.Length == 0)
                    errors.Add("Поле 'Пароль' не должно быть пустым.");
                if (Password_box.Password.Length == 0)
                    errors.Add("Поле 'Подтверждение пароля' не должно быть пустым.");


                if (!string.IsNullOrWhiteSpace(TelNo_box.Text) && TelNo_box.Text.Length != 11)
                    errors.Add("Поле 'Номер телефона' должно содержать 11 цифр.");


                if (Password_box1.Password != Password_box.Password)
                    errors.Add("Пароли не совпадают.");


                if (!string.IsNullOrWhiteSpace(Email_box.Text) && !IsValidEmail(Email_box.Text))
                    errors.Add("Поле 'Email' имеет неверный формат.");


                if (!string.IsNullOrWhiteSpace(Password_box.Password) && Password_box.Password.Length < 6)
                    errors.Add("Пароль должен содержать не менее 6 символов.");


                if (!string.IsNullOrWhiteSpace(Login_box.Text) && _dbContext.UserAuths.Any(x => x.Login == Login_box.Text))
                    errors.Add("Логин уже занят.");

                if (errors.Count > 0)
                {
                    TextBlockRegPopup.Text = string.Join("\n", errors);
                    TextBlockRegPopup.Visibility = Visibility.Visible;
                    return;
                }
                var GUID = Guid.NewGuid().ToString();

                var newUser = new User
                {
                    FirstName = FirstName_box.Text,
                    LastName = LastName_box.Text,
                    Email = Email_box.Text,
                    RoleId = 2,
                    PhoneNumber = TelNo_box.Text,
                    Guid = GUID,
                };

                _dbContext.Users.Add(newUser);
                _dbContext.SaveChanges();

                var newAuth = new UserAuth
                {
                    UserId = newUser.UserId,
                    Password = Password_box.Password,
                    Login = Login_box.Text,
                };
                _dbContext.UserAuths.Add(newAuth);
                _dbContext.SaveChanges();

                MessageBox.Show("Регистрация прошла успешно, войдите в аккаунт для авторизации в приложении");

                RegPopup.IsOpen = false;
                LoginPopup.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ButtonPopupBack_click(object sender, EventArgs e)
        {
            GuidTextBox.Text = null;
            LoginG_box.Text = null;
            PasswordG_box.Password = null;
            PasswordG_box1.Password = null;
            TextBlockRegPopup.Text = null;
            Erortext.Text = null;

            Erortext.Visibility=Visibility.Collapsed;
            nextButton.Visibility= Visibility.Visible;
            regButton.Visibility= Visibility.Collapsed;

            LoginGuidStackPanel.Visibility = Visibility.Collapsed;
            GuidStackPanel.Visibility = Visibility.Visible;
            


            RegPopup.IsOpen = false;
            RegGuidPopup.IsOpen = false;
            LoginPopup.IsOpen = true;

        }

        private void ButtonPopupNextG_click(object sender, EventArgs e)
        {
            try
            {
                var enterGuid = GuidTextBox.Text;
                var GUID = _dbContext.Users.FirstOrDefault(x => x.Guid == enterGuid);

                if (GUID != null)
                {
                    var aurtGUID = _dbContext.UserAuths.FirstOrDefault(x => x.UserId == GUID.UserId);
                    if (aurtGUID == null)
                    {
                        GuidStackPanel.Visibility = Visibility.Collapsed;
                        nextButton.Visibility = Visibility.Collapsed;
                        regButton.Visibility = Visibility.Visible;
                        LoginGuidStackPanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Erortext.Visibility = Visibility.Visible;
                        Erortext.Text = "Данный GUID уже зарегистрирован!";
                    }
                }
                else
                { Erortext.Visibility= Visibility.Visible;
                    Erortext.Text= "Не верный GUID, попробуйте еще раз!"; }
                
            }
            catch (Exception ex)
            {
                Erortext.Text =$"Ошибка при регистрации по GUID{ex.Message}";
            }
        }

        private void ButtonPopupRegG_click(object sender, EventArgs e)
        {
            var enterGuid = GuidTextBox.Text;
            var GUID = _dbContext.Users.FirstOrDefault(x => x.Guid == enterGuid);

            if ( string.IsNullOrEmpty(PasswordG_box.Password) && string.IsNullOrEmpty(LoginG_box.Text))
            {
                erorTextBlock.Text = "Поля не должны быть пустыми!";
                return;
            }
            bool checkLogin = _dbContext.UserAuths.Any(x => x.Login == LoginG_box.Text);
            if (checkLogin)
            {
                erorTextBlock.Text = "Данный логин уже занят, введите другой!";
                return ;
            }
            var newAuth = new UserAuth
            {
                UserId = GUID.UserId,
                Password = PasswordG_box.Password,
                Login = LoginG_box.Text,
            };
            _dbContext.UserAuths.Add(newAuth);
            _dbContext.SaveChanges();

            MessageBox.Show("Регистрация прошла успешно, войдите в аккаунт для авторизации в приложении", "Успех",MessageBoxButton.OK,MessageBoxImage.Information);

            RegPopup.IsOpen = false;
            RegGuidPopup.IsOpen = false;
            LoginPopup.IsOpen = true;
        }


        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }
    }
}