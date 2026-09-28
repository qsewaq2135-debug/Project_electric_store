using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EBIKES09
{
    public partial class UserInfoPage : Page
    {
        private User _user;
        private UserAuth _userAuth;
        private Bdebikes09Context _context;

        public UserInfoPage(User user)
        {
            InitializeComponent();
            _user = user;
            _context = new Bdebikes09Context();

            LoadUserData();
        }

        private void LoadUserData()
        {


            FirstNameTextBlock.Text = _user.FirstName;
            LastNameTextBlock.Text = _user.LastName;
            EmailTextBlock.Text = _user.Email;
            PhoneNumberTextBlock.Text = _user.PhoneNumber;

            _userAuth = _context.UserAuths.FirstOrDefault(ua => ua.UserId == _user.UserId);
            if (_userAuth != null)
            {
                LoginTextBlock.Text = _userAuth.Login;
                PasswordTextBlock.Text = _userAuth.Password;
            }
            else
            {
                LoginTextBlock.Text = "Не указан";
                PasswordTextBlock.Text = "Не указан";
            }

        }
        
       
        private void ButtonEdit_click(object sender, RoutedEventArgs e)
        {
            EditUserPage editUserWindow = new EditUserPage(_user);
            NavigationService.Navigate(editUserWindow);

        }
    }

}