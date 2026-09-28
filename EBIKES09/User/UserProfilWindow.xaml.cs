using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EBIKES09
{

    public partial class UserProfilePage : Page
    {
        
        private bool FirstKlick = true;
        private Window _currentWindow;
        private string ConnectionString;
        public User Users { get; set; }
        public User _user;
        private int _orderId;
        private UserAuth _userAuth;
        private Bdebikes09Context _context;
        public UserProfilePage(User user)
        {
            
            InitializeComponent();
            
            Users = user;
            
            DataContext = Users;
            ConnectionString = null;
            FirstName_box.TextChanged += TextChanged;
            LastName_box.TextChanged += TextChanged;
            PhoneNumber_box.TextChanged += TextChanged;
            Email_box.TextChanged += TextChanged;
            LoadOrders();
            _context = new Bdebikes09Context();
            MouseDown += MainWindow_MouseDown;

            _userAuth = _context.UserAuths.FirstOrDefault(ua => ua.UserId == _user.UserId);


        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                using (var context = new Bdebikes09Context())
                {
                    var user = context.Users.Find(Users.UserId);
                    if (user != null)
                    {
                        if (string.IsNullOrEmpty(FirstName_box.Text) && string.IsNullOrEmpty(LastName_box.Text) && string.IsNullOrEmpty(PhoneNumber_box.Text) && string.IsNullOrEmpty(Email_box.Text))
                        {
                            MessageBox.Show("Поля не должны быть пустыми", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        if (!string.IsNullOrWhiteSpace(PhoneNumber_box.Text) && PhoneNumber_box.Text.Length != 11)
                        {
                            MessageBox.Show("Номер телефона должен содержать 11 цифр", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        if (!string.IsNullOrWhiteSpace(Email_box.Text) && !IsValidEmail(Email_box.Text))
                        {
                            MessageBox.Show("Поле 'Email' имеет неверный формат.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        


                        user.FirstName = FirstName_box.Text;
                        user.LastName = LastName_box.Text;
                        user.PhoneNumber = PhoneNumber_box.Text;
                        user.Email = Email_box.Text;

                        context.SaveChanges();
                        
                        MessageBox.Show("Изменения сохранены успешно!", "Информирование", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}" +
                    $"Тип исключения: {ex.GetType().Name}" +
                    $"Трассировка стека: {ex.StackTrace}"
                    , "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void TextChanged(object sender, RoutedEventArgs e)
        {
            
            
            
        }
        
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            

                if (e.OriginalSource is not TextBox && e.OriginalSource is not PasswordBox)
                {
                     Keyboard.Focus(this); 
    
                }
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }
        private void LoadOrders()
        {

           

            using (var context = new Bdebikes09Context()) 
            {
                {
                    _user = context.Users.Find(SessionManager.Instance.UserId);
                   
                }
                if (_user.RoleId == 1)
                {
                    ButtonEditePassword.Visibility = Visibility.Collapsed;
                    TextBlickDataGrid.Visibility = Visibility.Collapsed;
                    ProfileListBox.Visibility = Visibility.Collapsed;
                    ButtonOrderAdmin.Visibility = Visibility.Visible;
                    ButtonUserAdmin.Visibility = Visibility.Visible;
                    ShowArchivedCheckBox.Visibility = Visibility.Collapsed;
                }
                else if (_user.RoleId == 3) 
                {
                    ButtonEditePassword.Visibility = Visibility.Collapsed;
                    TextBlickDataGrid.Visibility = Visibility.Collapsed;
                    ProfileListBox.Visibility = Visibility.Collapsed;
                        ButtonOrderAdmin.Visibility = Visibility.Visible;
                        ButtonUserAdmin.Visibility = Visibility.Collapsed;
                    ShowArchivedCheckBox.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ButtonEditePassword.Visibility = Visibility.Visible;
                    TextBlickDataGrid.Visibility = Visibility.Visible;
                    ProfileListBox.Visibility = Visibility.Visible;
                    ButtonOrderAdmin.Visibility = Visibility.Collapsed;
                    ButtonUserAdmin.Visibility = Visibility.Collapsed;
                    ShowArchivedCheckBox.Visibility = Visibility.Visible;
                };
                List<Order> orders = null;
                try
                {
                    orders = context.Orders
                       .Include(o => o.Status)
                       .Include(o => o.Payment)
                       .Include(o => o.OrderDetils)
                       .ThenInclude(od => od.Product)
                       .Where(o => o.UserId == Users.UserId)
                       .ToList();

                    if (!ShowArchivedCheckBox.IsChecked ?? true)
                    {
                        orders = orders.Where(o =>
                            o.Status.StatusName != "Отменён" &&
                            o.Status.StatusName != "Завершён").ToList();
                    }

                    ProfileListBox.ItemsSource = orders;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}");
                    return;
                    
                }


                if (orders != null && orders.Count > 0)
                {
                    ProfileListBox.ItemsSource = orders;
                }
                
            }
        }

        private void ShowArchivedCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            LoadOrders(); 
        }

        private void ButtonUserAdmin_click(object sender, RoutedEventArgs e)
        {
            AdminUsersPage adminUsersWindow = new AdminUsersPage();
            NavigationService.Navigate(adminUsersWindow);
        }
       
       
        private void ButtonOrderAdmin_click(object sender, RoutedEventArgs e)
        {
            AdminOrdersPage adminOrdersWindow = new AdminOrdersPage();
            NavigationService.Navigate(adminOrdersWindow);
        }


        private void ButtonExite_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти?", "Подтверждение выхода", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                SessionManager.Instance.Logout();
                ProductPage productPage = new ProductPage(1);
                NavigationService.Navigate(productPage);

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.SearchOrderVoid();
            }
        }

        private void ButtonEditePassword_Click(object sender, RoutedEventArgs e)
        {
            PasswordPopup.IsOpen = true;
        }

        private void ButtonEditePencil_Click(object sender, RoutedEventArgs e)
        {
            if (FirstKlick)
            {
                FirstName_box.IsReadOnly = false;
                LastName_box.IsReadOnly = false;
                PhoneNumber_box.IsReadOnly = false;
                Email_box.IsReadOnly = false;
                LastName_box.BorderThickness = new Thickness(2);
                PhoneNumber_box.BorderThickness = new Thickness(2);
                Email_box.BorderThickness = new Thickness(2);
                FirstName_box.BorderThickness = new Thickness(2);


            }
            else
            {
                FirstName_box.IsReadOnly = true;
                LastName_box.IsReadOnly = true;
                PhoneNumber_box.IsReadOnly = true;
                Email_box.IsReadOnly = true;
                FirstName_box.BorderThickness = new Thickness(0);
                LastName_box.BorderThickness = new Thickness(0);
                PhoneNumber_box.BorderThickness = new Thickness(0);
                Email_box.BorderThickness = new Thickness(0);
            }
            

            using (var context = new Bdebikes09Context())
            {
                var users = context.Users.Find(Users.UserId);
                if (!users.FirstName.Trim().Equals(FirstName_box.Text.Trim(), StringComparison.OrdinalIgnoreCase) ||
    !users.LastName.Trim().Equals(LastName_box.Text.Trim(), StringComparison.OrdinalIgnoreCase) ||
    !users.PhoneNumber.Trim().Equals(PhoneNumber_box.Text.Trim(), StringComparison.OrdinalIgnoreCase) ||
    !users.Email.Trim().Equals(Email_box.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    SaveButton.Visibility = Visibility.Visible;
                }
            }

            FirstKlick = !FirstKlick;
        }
        private void ButtonSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            
                
            var testPassword = _userAuth.Password;
            string oldPassword = OldPasswordTextBox.Password;
            string newPassword = NewPasswordTextBox.Password;
            string confirmPassword = ConfirmPasswordTextBox.Password;

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TextBlockPassword.Text = "Все поля должны быть заполнены!";
                TextBlockPassword.Visibility = Visibility.Visible;
                return;
            }

            if (newPassword != confirmPassword)
            {
                TextBlockPassword.Text = "Новый пароль и подтверждение не совпадают!";
                TextBlockPassword.Visibility = Visibility.Visible;
                return;
            }

            if (newPassword == testPassword)
            {
                TextBlockPassword.Text = "Новый пароль не должен совпадать со старым!";
                TextBlockPassword.Visibility = Visibility.Visible;
                return;
            }



            if (CheckOldPassword(oldPassword))
            {
                UpdatePassword(newPassword);
                MessageBox.Show("Пароль успешно изменен!");
                
            }
            else
            {
                TextBlockPassword.Text = "Неверный старый пароль!";
                TextBlockPassword.Visibility = Visibility.Visible;
            }
        }

        private bool CheckOldPassword(string oldPassword)
        {


            return oldPassword == _userAuth.Password;
        }

        private void UpdatePassword(string newPassword)
        {

            _userAuth.Password = ConfirmPasswordTextBox.Password;
            _context.SaveChanges();
        }
    }
}
