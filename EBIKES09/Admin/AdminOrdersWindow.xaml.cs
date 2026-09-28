using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class AdminOrdersPage : Page
    {
        private Bdebikes09Context _context;
        private List<Order> _allOrders;
        private User _currentUser;

        public AdminOrdersPage()
        {
            InitializeComponent();
            var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                 .UseNpgsql()
                 .Options;
            _context = new Bdebikes09Context(dbContextOptions);
            LoadOrders();
        }

        private void LoadOrders()
        {
            if (_context == null) return;

            
            _allOrders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.Status)
                .ToList();

       
            OrdersListBox.ItemsSource = _allOrders;

           
            List<string> statuses = _context.Statuses.Select(s => s.StatusName).ToList();
            statuses.Insert(0, "Все"); 
            StatusFilterComboBox.ItemsSource = statuses;
            StatusFilterComboBox.SelectedItem = "Все";
        }

        private void ButtonApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            string statusFilter = StatusFilterComboBox.SelectedItem as string;
            DateOnly? startDate = StartDatePicker.SelectedDate.HasValue
                ? DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value)
                : (DateOnly?)null;
            DateOnly? endDate = EndDatePicker.SelectedDate.HasValue
                ? DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value)
                : (DateOnly?)null;

            IEnumerable<Order> filteredOrders = _allOrders;

            
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredOrders = filteredOrders.Where(o => o.OrderId.ToString().Contains(searchText));
            }

           
            if (statusFilter != null && statusFilter != "Все")
            {
                filteredOrders = filteredOrders.Where(o => o.Status.StatusName == statusFilter);
            }

          
            if (startDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.OrderDateEnd.HasValue && o.OrderDate.Value <= endDate.Value);
            }

            
            OrdersListBox.ItemsSource = filteredOrders.ToList();
        }
        private void OrdersListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is Order selectedOrder)
            {
                
                EditOrderPage editOrderWindow = new EditOrderPage(selectedOrder.OrderId);
                NavigationService.Navigate(editOrderWindow);

               
                LoadOrders();
            }
        }

        
    }
}