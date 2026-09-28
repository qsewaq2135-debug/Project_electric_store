using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class EditOrderPage : Page
    {
        private Bdebikes09Context _context;
        private Order _order;
        private User selectedUser;

        public EditOrderPage(int orderId)
        {
            InitializeComponent();

            // Инициализация контекста базы данных
            var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                 .UseNpgsql()
                 .Options;
            _context = new Bdebikes09Context(dbContextOptions);

            // Загрузка данных заказа
            _order = _context.Orders
                .Include(o => o.User) 
                .Include(o => o.Payment)
                .Include(o => o.Status)
                
                .Include(o => o.OrderDetils)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == orderId);

            if (_order != null)
            {
                LoadOrderData(); 
                LoadStatus();    
            }
            else
            {
                MessageBox.Show("Заказ не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService.GoBack();
            }
        }

        public EditOrderPage(User selectedUser)
        {
            this.selectedUser = selectedUser;
        }

        
        private void LoadOrderData()
        {
            if (_order == null) return;

           
            CartIdTextBlock.Text = _order.OrderId.ToString();
            UserIdTextBlock.Text = _order.UserId.ToString();
            CreatedAtTextBlock.Text = _order.OrderDate.ToString();
            EndAtTextBlock.Text = _order.OrderDateEnd.ToString();

         
            if (_order.User != null)
            {
                FirstNameTextBlock.Text = _order.User.FirstName;
                LastNameTextBlock.Text = _order.User.LastName;
                EmailTextBlock.Text = _order.User.Email;
                PhoneNumberTextBlock.Text = _order.User.PhoneNumber;
            }

          
            CartItemsListBox.ItemsSource = _order.OrderDetils;
        }

        
        private void LoadStatus()
{
   
    var currentStatus = _order.Status?.StatusName;

   
    var allStatuses = _context.Statuses.Select(s => s.StatusName).ToList();
    
   
    if (currentStatus == "В обработке")
    {
        StatusComboBox.ItemsSource = allStatuses.Where(s => s == "В обработке" || s == "Готов" || s == "Отменён").ToList();
    }
    else if (currentStatus == "Готов")
    {
        StatusComboBox.ItemsSource = allStatuses.Where(s => s == "Готов" || s == "Завершён" || s == "Отменён").ToList();
    }
    else
    {
        
        StatusComboBox.ItemsSource = new List<string> { currentStatus };
        StatusComboBox.IsEnabled = false;
    }

    
    StatusComboBox.SelectedItem = currentStatus;

    
    if (_order.OrderDateEnd != null)
    {
        StatusComboBox.IsEnabled = false;
        TextOreder.Visibility = Visibility.Visible;
        StackEndDate.Visibility = Visibility.Visible;
    }
}

        
        private void ButtonSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_order == null)
                {
                    MessageBox.Show("Заказ не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                var selectedStatus = StatusComboBox.SelectedItem as string;
                if (!string.IsNullOrEmpty(selectedStatus))
                {

                    var status = _context.Statuses.FirstOrDefault(s => s.StatusName == selectedStatus);
                    var statusString = _context.Statuses.FirstOrDefault(s => s.StatusName == selectedStatus).ToString();

                    if (status != null)
                    {
                        _order.StatusId = status.StatusId; 
                    }

                    if (selectedStatus == "Завершён")
                    {
                        _order.OrderDateEnd = DateOnly.FromDateTime(DateTime.Now);
                    }
                    if (selectedStatus == "Отменён")
                    {
                        _order.OrderDateEnd = DateOnly.FromDateTime(DateTime.Now);
                        ReturnProducts();
                    }
                }
               
                _context.SaveChanges();
                MessageBox.Show("Изменения сохранены.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ReturnProducts()
        {
            if (_order == null) return;

            if (_order.Status.StatusName == "Отменён") return;
            foreach (var orderDetail in _order.OrderDetils)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                if (product != null)
                {
                    product.StockQuantity += orderDetail.Quantity;
                    _context.Products.Update(product);
                }
            }
        }

      
    }
}