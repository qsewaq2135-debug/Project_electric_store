using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class AdminUsersPage : Page
    {
        private Bdebikes09Context _context;
        private List<User> _allUsers;

        public AdminUsersPage()
        {
            InitializeComponent();
            var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                 .UseNpgsql()
                 .Options;
            _context = new Bdebikes09Context(dbContextOptions);
            LoadUsers();
        }
        private void ButtonAddUser_Click(object sender, RoutedEventArgs e)
        {
            
            AddUserPage addUserPage = new AddUserPage();
            addUserPage.UserAdded += AddUserPage_UserAdded; 
            NavigationService.Navigate(addUserPage);
        }

        private void AddUserPage_UserAdded(object sender, EventArgs e)
        {
           
            LoadUsers();
        }
        private void LoadUsers()
        {
            if (_context == null) return;

            
            _allUsers = _context.Users.ToList();

            
            UsersListBox.ItemsSource = _allUsers;
        }

        private void ButtonApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            IEnumerable<User> filteredUsers = _allUsers;

            
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredUsers = filteredUsers.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(searchText)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(searchText)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchText)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(searchText))
                );
            }

            
            UsersListBox.ItemsSource = filteredUsers.ToList();
        }

        private void UsersListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is User selectedUser)
            {
                
                UserInfoPage userInfoWindow = new UserInfoPage(selectedUser);
                NavigationService.Navigate(userInfoWindow);
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedItem is User selectedUser)
            {
                
                EditOrderPage editUserWindow = new EditOrderPage(selectedUser);
                NavigationService.Navigate(editUserWindow);

               
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}