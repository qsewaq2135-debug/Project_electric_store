using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class AddPromotionPage : Page
    {
        private Bdebikes09Context _context;
        private List<int> _selectedProductIds;

        public AddPromotionPage()
        {
            InitializeComponent();
            var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                 .UseNpgsql()
                 .Options;
            _context = new Bdebikes09Context(dbContextOptions);
            _selectedProductIds = new List<int>();
        }

        private void ButtonAddPromotion_Click(object sender, RoutedEventArgs e)
        {
            List<string> errors = new List<string>();
            try
            {
                string promotionName = PromotionNameTextBox.Text;
                string discountType = (DiscountTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.Now;
                DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;

               
                if (string.IsNullOrWhiteSpace(promotionName))
                    errors.Add("Поле 'Название акции' не должно быть пустым.");
                if (string.IsNullOrWhiteSpace(DiscountValueTextBox.Text))
                    errors.Add("Поле 'Значение скидки' не должно быть пустым.");
                if (string.IsNullOrWhiteSpace(discountType))
                    errors.Add("Необходимо выбрать 'Тип скидки'.");

                
                if (!string.IsNullOrWhiteSpace(DiscountValueTextBox.Text))
                {
                    if (!decimal.TryParse(DiscountValueTextBox.Text, out decimal discountValue))
                        errors.Add("Поле 'Значение скидки' должно быть числом.");
                    else if (discountValue <= 0)
                        errors.Add("Значение скидки должно быть больше 0.");
                }
               
                if (DiscountTypeComboBox.SelectedItem == null)
                    errors.Add("Необходимо выбрать тип скидки.");

               
                if (startDate > endDate)
                    errors.Add("Дата начала не может быть позже даты окончания.");

                
                if (_selectedProductIds.Count == 0)
                    errors.Add("Необходимо выбрать товары для акции.");

                if (errors.Count > 0)
                {
                    MessageBox.Show(string.Join("\n", errors), "Ошибка добавления акции", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                decimal discountValueParse = decimal.Parse(DiscountValueTextBox.Text);

                Promotion newPromotion = new Promotion
                {
                    PromotionName = promotionName,
                    DiscountValue = discountValueParse,
                    DiscountType = discountType,
                    StartDate = startDate,
                    EndDate = endDate,
                    Description = DescriptionTextBox.Text
                };

                _context.Promotions.Add(newPromotion);
                _context.SaveChanges();

                foreach (int productId in _selectedProductIds)
                {
                    PromotionProduct promotionProduct = new PromotionProduct
                    {
                        ProductId = productId,
                        PromotionId = newPromotion.PromotionId
                    };
                    _context.PromotionProducts.Add(promotionProduct);
                }

                _context.SaveChanges();
                MessageBox.Show("Акция успешно добавлена!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}" +
                    $"Тип исключения: {ex.GetType().Name}" +
                    $"Трассировка стека: {ex.StackTrace}"
                    , "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonSelectProducts_Click(object sender, RoutedEventArgs e)
        {
            SelectProductsPage selectProductsWindow = new SelectProductsPage();
            if (NavigationService.Navigate(selectProductsWindow) == true)
            {
                _selectedProductIds = selectProductsWindow.SelectedProductIds;
            }
        }
    }
}