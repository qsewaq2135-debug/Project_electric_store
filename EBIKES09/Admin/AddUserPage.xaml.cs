using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions; // Добавлено для Regex
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class AddUserPage : Page
    {
        public event EventHandler UserAdded;

        private Bdebikes09Context _context;

        public AddUserPage()
        {
            InitializeComponent();
            var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                 .UseNpgsql()
                 .Options;
            _context = new Bdebikes09Context(dbContextOptions);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
          
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string email = EmailTextBox.Text;
            string phoneNumber = PhoneNumberTextBox.Text;
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;

           
            if (!ValidateInput(firstName, lastName, email, phoneNumber, login, password))
            {
                return; 
            }

           
            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };

            
            _context.Users.Add(newUser);
            _context.SaveChanges();

            var newUserAuth = new UserAuth
            {
                UserId = newUser.UserId,
                Login = login,
                Password = password,
            };

            _context.UserAuths.Add(newUserAuth);
            _context.SaveChanges();

            
            UserAdded?.Invoke(this, EventArgs.Empty);

           
            NavigationService.GoBack();
        }

        private bool ValidateInput(string firstName, string lastName, string email, string phoneNumber, string login, string password)
        {
           
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Пожалуйста, введите корректный email.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            
            if (!IsValidPhoneNumber(phoneNumber))
            {
                MessageBox.Show("Пожалуйста, введите корректный номер телефона.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            
            if (login.Length < 5)
            {
                MessageBox.Show("Логин должен содержать не менее 5 символов.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            
            if (!IsValidPassword(password))
            {
                MessageBox.Show("Пароль должен содержать не менее 8 символов, одну цифру и одну букву в верхнем регистре.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true; 
        }

        
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

       
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            
            return Regex.IsMatch(phoneNumber, @"^\+?[0-9\s]+$");
        }

        
        private bool IsValidPassword(string password)
        {
            
            return password.Length >= 8 && Regex.IsMatch(password, "[0-9]") && Regex.IsMatch(password, "[A-Z]");
        }

    }
}
