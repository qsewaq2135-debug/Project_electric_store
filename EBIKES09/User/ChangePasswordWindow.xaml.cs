using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class ChangePasswordPage : Page
    {
        private User _user;
        private UserAuth _userAuth;
        private Bdebikes09Context _context;

        public ChangePasswordPage(User user)
        {
            InitializeComponent();
            _user = user;
            
            _context = new Bdebikes09Context();

            _userAuth = _context.UserAuths.FirstOrDefault(ua => ua.UserId == _user.UserId);
        }


        private void ButtonSaveChanges_Click(object sender, RoutedEventArgs e)
        {

            var testPassword = _userAuth.Password;
            string oldPassword = OldPasswordTextBox.Password;
            string newPassword = NewPasswordTextBox.Password;
            string confirmPassword = ConfirmPasswordTextBox.Password;

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Новый пароль и подтверждение не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (newPassword == testPassword)
            {
                MessageBox.Show("Новый пароль не должен совпадать со старым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }


            
            if (CheckOldPassword(oldPassword))
            {
                UpdatePassword(newPassword);
                MessageBox.Show("Пароль успешно изменен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Неверный старый пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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