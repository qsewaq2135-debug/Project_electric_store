using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class ReviewsPage : Page
    {
        private User _user;
       
        private int _productId;
        private Bdebikes09Context _context;
        private List<Review> _allReviews;

        public ReviewsPage(int productId)
        {
            InitializeComponent();
            _context = new Bdebikes09Context();
            _productId = productId;
            LoadAll();
        }

        private void LoadAll()
        {
            
            _allReviews = _context.Reviews
                .Include(r => r.User)  
                .Include(r => r.Product) 
                .Where(r => r.ProductId == _productId)
                .ToList();

            var _product = _context.Products.FirstOrDefault(r => r.ProductId == _productId);
            var _review = _context.Reviews.Where(re => re.ProductId == _productId).Average(re => re.Rating);
            ProductNameTextBlock.Text = _product.ProductName;
            if (_review != null)
            {

                ProductRatingTextBlock.Text = string.Format("{0:F1}", _review);
            }
            else
            {
                ProductRatingTextBlock.Text = "Нет оценок";
            }

           
            ReviewsItemsControl.ItemsSource = _allReviews;

           
            RatingFilterComboBox.SelectedIndex = 0; 
        }

        private void ClearFields()
        {
            ReviewTextBox.Text = string.Empty;
            RatingComboBox.SelectedItem = null;
        }

        private void ButtonAddReview_click(object sender, RoutedEventArgs e)
        {
            int userId = SessionManager.Instance.UserId;
            bool hasCompletedOrderWithProduct = _context.OrderDetils.Any(od =>
                    od.ProductId == _productId &&
                    od.Order.UserId == userId &&
                    od.Order.StatusId == 2);

            if (!hasCompletedOrderWithProduct)
            {
                MessageBox.Show("Чтобы оставить отзыв на данный товар, необходимо его купить", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SessionManager.Instance.UserId <= 0)
            {
                MessageBox.Show("Необходимо войти в систему, чтобы оставить отзыв.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (RatingComboBox.SelectedItem != null)
            {
               
                string selectedRatingText = (RatingComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                int selectedRating = selectedRatingText?.Count(c => c == '★') ?? 0;

                if (selectedRating > 0)
                {
                    
                    Review newReview = new Review()
                    {
                        UserId = SessionManager.Instance.UserId,
                        ProductId = _productId,
                        Rating = selectedRating,
                        Comment = ReviewTextBox.Text,
                    };

                    try
                    {
                        _context.Reviews.Add(newReview);
                        _context.SaveChanges();
                        MessageBox.Show("Отзыв успешно добавлен!");

                        ClearFields();
                        LoadAll(); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении отзыва: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите рейтинг из списка.");
                }
            }
        }

        private void RatingFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyRatingFilter();
        }

        private void ApplyRatingFilter()
        {
            if (_allReviews == null) return;

            string selectedRatingText = (RatingFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            int selectedRating = selectedRatingText?.Count(c => c == '★') ?? 0;

            if (selectedRatingText == "Все")
            {
                ReviewsItemsControl.ItemsSource = _allReviews;
            }
            else
            {
                ReviewsItemsControl.ItemsSource = _allReviews
                    .Where(r => r.Rating == selectedRating)
                    .ToList();
            }
        }
    }
}