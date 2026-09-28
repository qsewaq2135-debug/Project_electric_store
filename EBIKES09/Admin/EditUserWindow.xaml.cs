using EBIKES09.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class EditUserPage : Page
    {
        private User _user;
        private UserAuth _userAuth;
        private Bdebikes09Context _context;

        public EditUserPage(User user)
        {
            InitializeComponent();
            _user = user;
            _context = new Bdebikes09Context();
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (_user == null) return;

            
            FirstNameTextBox.Text = _user.FirstName;
            LastNameTextBox.Text = _user.LastName;
            EmailTextBox.Text = _user.Email;
            PhoneNumberTextBox.Text = _user.PhoneNumber;

            
            _userAuth = _context.UserAuths.FirstOrDefault(ua => ua.UserId == _user.UserId);
            if (_userAuth != null)
            {
                LoginTextBox.Text = _userAuth.Login;
                PasswordTextBox.Text = _userAuth.Password;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (_user == null) return;

            List<string> errors = new List<string>();

            
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
                errors.Add("Поле 'Имя' не должно быть пустым.");
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
                errors.Add("Поле 'Фамилия' не должно быть пустым.");
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
                errors.Add("Поле 'Email' не должно быть пустым.");
            if (string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
                errors.Add("Поле 'Телефон' не должно быть пустым.");
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
                errors.Add("Поле 'Логин' не должно быть пустым.");
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Text))
                errors.Add("Поле 'Пароль' не должно быть пустым.");

            
            if (!string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text) && PhoneNumberTextBox.Text.Length != 11)
                errors.Add("Поле 'Телефон' должно содержать 11 цифр.");

            if (!string.IsNullOrWhiteSpace(EmailTextBox.Text) && !IsValidEmail(EmailTextBox.Text))
                errors.Add("Поле 'Email' имеет неверный формат.");

            if (!string.IsNullOrWhiteSpace(PasswordTextBox.Text) && PasswordTextBox.Text.Length < 6)
                errors.Add("Пароль должен содержать не менее 6 символов.");

          
            if (_userAuth == null || _userAuth.Login != LoginTextBox.Text)
            {
                if (!string.IsNullOrWhiteSpace(LoginTextBox.Text) && _context.UserAuths.Any(x => x.Login == LoginTextBox.Text))
                    errors.Add("Логин уже занят.");
            }
            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errors), "Ошибка редактирования пользователя", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

           
            _user.FirstName = FirstNameTextBox.Text;
            _user.LastName = LastNameTextBox.Text;
            _user.Email = EmailTextBox.Text;
            _user.PhoneNumber = PhoneNumberTextBox.Text;

            
            if (_userAuth == null)
            {
                _userAuth = new UserAuth
                {
                    UserId = _user.UserId,
                    Login = LoginTextBox.Text,
                    Password = PasswordTextBox.Text
                };
                _context.UserAuths.Add(_userAuth);
            }
            else
            {
                _userAuth.Login = LoginTextBox.Text;
                _userAuth.Password = PasswordTextBox.Text;
            }

           
            _context.SaveChanges();
            MessageBox.Show("Изменения сохранены.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.GoBack();
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