using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace EBIKES09
{
    public partial class AddPage : Page
    {
        private Bdebikes09Context _context;
        private string _imagePath;

        public AddPage()
        {
            InitializeComponent();
            var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                 .UseNpgsql()
                 .Options;
            _context = new Bdebikes09Context(dbContextOptions);
            LoadAll();

            CategoryComboBox.SelectionChanged += CategoryComboBox_SelectionChanged;
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem is string selectedCategory)
            {
                if (selectedCategory == "Электровелосипеды")
                {
                    StackPanel1.Visibility = Visibility.Visible;
                    StackPanel2.Visibility = Visibility.Collapsed;
                }
                else if (selectedCategory == "Аккумуляторы")
                {
                    StackPanel1.Visibility = Visibility.Collapsed;
                    StackPanel2.Visibility = Visibility.Visible;
                }
                else
                {
                    StackPanel1.Visibility = Visibility.Collapsed;
                    StackPanel2.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void LoadAll()
        {
            if (_context == null) return;

            try
            {
                CategoryComboBox.ItemsSource = _context.Categories.Select(c => c.CategoryName).ToList();
                BrandComboBox.ItemsSource = _context.Brands.Select(c => c.BrandName).ToList();
                FrameMaterialComboBox.ItemsSource = _context.FrameMaterials.Select(c => c.FrameMaterialName).ToList();
                FrontBrakesComboBox.ItemsSource = _context.FrontBrakes.Select(c => c.FrontBrakesName).ToList();
                RearBrakesComboBox.ItemsSource = _context.RearBrakes.Select(c => c.RearBrakesName).ToList();
                DriveComboBox.ItemsSource = _context.Drives.Select(c => c.DriveName).ToList();
                VoltageComboBox.ItemsSource = _context.Voltages.Select(c => c.VoltageName).ToList();
                BatteryTypeComboBox.ItemsSource = _context.BatteryTypes.Select(bt => bt.BatteryType1).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonAddProduct_Click(object sender, RoutedEventArgs e)
        {
            List<string> errors = new List<string>();

            try
            {
                
                if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
                    errors.Add("Поле 'Название товара' не должно быть пустым.");
                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                    errors.Add("Поле 'Описание' не должно быть пустым.");
                if (string.IsNullOrWhiteSpace(PriceTextBox.Text) || !decimal.TryParse(PriceTextBox.Text, out _))
                    errors.Add("Поле 'Цена' должно быть числом.");
                if (string.IsNullOrWhiteSpace(StockQuantityTextBox.Text) || !int.TryParse(StockQuantityTextBox.Text, out _))
                    errors.Add("Поле 'Количество на складе' должно быть целым числом.");
                if (string.IsNullOrWhiteSpace(ImageUrlTextBox.Text))
                    errors.Add("Поле 'Изображение' не должно быть пустым.");
                if (CategoryComboBox.SelectedItem == null)
                    errors.Add("Необходимо выбрать 'Категорию'.");
                if (BrandComboBox.SelectedItem == null)
                    errors.Add("Необходимо выбрать 'Бренд'.");

                if (CategoryComboBox.SelectedItem is string selectedCategory)
                {
                    if (selectedCategory == "Электровелосипеды")
                    {
                        
                        if (!int.TryParse(MaximumSpeedTextBox.Text, out _))
                            errors.Add("Поле 'Максимальная скорость' должно быть целым числом.");
                        if (!int.TryParse(MaximumRangeTextBox.Text, out _))
                            errors.Add("Поле 'Максимальный пробег' должно быть целым числом.");
                        if (!int.TryParse(MaxLoadTextBox.Text, out _))
                            errors.Add("Поле 'Максимальная нагрузка' должно быть целым числом.");
                        if (!int.TryParse(MotorPowerTextBox.Text, out _))
                            errors.Add("Поле 'Мощность мотора' должно быть целым числом.");
                        if (!int.TryParse(VoltageTextBox.Text, out _))
                            errors.Add("Поле 'Напряжение' должно быть целым числом.");
                        if (!int.TryParse(CapacityTextBox.Text, out _))
                            errors.Add("Поле 'Емкость' должно быть целым числом.");
                        if (!int.TryParse(FullChargeTimeTextBox.Text, out _))
                            errors.Add("Поле 'Время полной зарядки' должно быть целым числом.");
                        if (!int.TryParse(WheelDiameterTextBox.Text, out _))
                            errors.Add("Поле 'Диаметр колеса' должно быть целым числом.");
                        if (!int.TryParse(NumberOfSpeedsTextBox.Text, out _))
                            errors.Add("Поле 'Количество скоростей' должно быть целым числом.");

                        if (FrameMaterialComboBox.SelectedItem == null)
                            errors.Add("Необходимо выбрать 'Материал рамы'.");
                        if (FrontBrakesComboBox.SelectedItem == null)
                            errors.Add("Необходимо выбрать 'Передние тормоза'.");
                        if (RearBrakesComboBox.SelectedItem == null)
                            errors.Add("Необходимо выбрать 'Задние тормоза'.");
                        if (DriveComboBox.SelectedItem == null)
                            errors.Add("Необходимо выбрать 'Привод'.");
                    }
                    else if (selectedCategory == "Аккумуляторы")
                    {
                       
                        if (!int.TryParse(NominalCapacityTextBox.Text, out _))
                            errors.Add("Поле 'Емкость' должно быть целым числом.");
                        if (VoltageComboBox.SelectedItem == null)
                            errors.Add("Необходимо выбрать 'Напряжение'.");
                        if (BatteryTypeComboBox.SelectedItem == null)
                            errors.Add("Необходимо выбрать 'Тип аккумулятора'.");
                        if (string.IsNullOrWhiteSpace(ServiceLifeBoxTextBox.Text) || !int.TryParse(ServiceLifeBoxTextBox.Text, out _))
                            errors.Add("Поле 'Срок службы' должно быть целым числом.");
                    }
                }

                if (errors.Count > 0)
                {
                    MessageBox.Show(string.Join("\n", errors), "Ошибка добавления товара", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                
                var category = _context.Categories.FirstOrDefault(c => c.CategoryName == CategoryComboBox.SelectedItem.ToString());
                var brand = _context.Brands.FirstOrDefault(b => b.BrandName == BrandComboBox.SelectedItem.ToString());

                if (category == null || brand == null)
                {
                    MessageBox.Show("Не удалось найти выбранную категорию или бренд в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                
                Product newProduct = new Product
                {
                    ProductName = ProductNameTextBox.Text,
                    Description = DescriptionTextBox.Text,
                    Price = decimal.Parse(PriceTextBox.Text),
                    StockQuantity = int.Parse(StockQuantityTextBox.Text),
                    ImageUrl = ImageUrlTextBox.Text,
                    CategoryId = category.CategoryId,
                    BrandId = brand.BrandId,
                    IsActive = true,
                };

                _context.Products.Add(newProduct);
                _context.SaveChanges();

                if (CategoryComboBox.SelectedItem is string sselectedCategory)
                {
                    if (sselectedCategory == "Электровелосипеды")
                    {
                        var frameMaterial = _context.FrameMaterials.FirstOrDefault(f => f.FrameMaterialName == FrameMaterialComboBox.SelectedItem.ToString());
                        var frontBrakes = _context.FrontBrakes.FirstOrDefault(fb => fb.FrontBrakesName == FrontBrakesComboBox.SelectedItem.ToString());
                        var rearBrakes = _context.RearBrakes.FirstOrDefault(rb => rb.RearBrakesName == RearBrakesComboBox.SelectedItem.ToString());
                        var drive = _context.Drives.FirstOrDefault(d => d.DriveName == DriveComboBox.SelectedItem.ToString());

                        if (frameMaterial == null || frontBrakes == null || rearBrakes == null || drive == null)
                        {
                            MessageBox.Show("Не удалось найти выбранные характеристики для электровелосипеда.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        BikeCharacteristic newBikeCharacteristic = new BikeCharacteristic
                        {
                            MaximumSpeed = int.Parse(MaximumSpeedTextBox.Text),
                            MaximumRange = int.Parse(MaximumRangeTextBox.Text),
                            MaxLoad = int.Parse(MaxLoadTextBox.Text),
                            MotorPower = int.Parse(MotorPowerTextBox.Text),
                            Voltage = int.Parse(VoltageTextBox.Text),
                            Capacity = int.Parse(CapacityTextBox.Text),
                            FullChargeTime = int.Parse(FullChargeTimeTextBox.Text),
                            WheelDiameter = int.Parse(WheelDiameterTextBox.Text),
                            NumberOfSpeeds = int.Parse(NumberOfSpeedsTextBox.Text),
                            Weight = WeightTextBox.Text,
                            Dimensions = DimensionsTextBox.Text,
                            PackageDimensions = PackageDimensionsTextBox.Text,
                            SoundSignal = SoundSignalCheckBox.IsChecked ?? false,
                            Wings = WingsCheckBox.IsChecked ?? false,
                            FrameMaterialId = frameMaterial.FrameMaterialId,
                            FrontBrakesId = frontBrakes.FrontBrakesId,
                            RearBrakesId = rearBrakes.RearBrakesId,
                            ProductId = newProduct.ProductId,
                            DriveId = drive.DriveId
                        };

                        _context.BikeCharacteristics.Add(newBikeCharacteristic);
                    }
                    else if (sselectedCategory == "Аккумуляторы")
                    {
                        var voltage = _context.Voltages.FirstOrDefault(v => v.VoltageName == VoltageComboBox.SelectedItem.ToString());
                        var batteryType = _context.BatteryTypes.FirstOrDefault(b => b.BatteryType1 == BatteryTypeComboBox.SelectedItem.ToString());

                        if (voltage == null || batteryType == null)
                        {
                            MessageBox.Show("Не удалось найти выбранное напряжение или тип аккумулятора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        BatteryCharacteristic newBatteryCharacteristic = new BatteryCharacteristic
                        {
                            ProductId = newProduct.ProductId,
                            VoltageId = voltage.VoltageId,
                            BatteryTypeId = batteryType.BatteryTypeId,
                            NominalCapacity = int.Parse(NominalCapacityTextBox.Text),
                            ServiceLife = int.Parse(ServiceLifeBoxTextBox.Text),
                        };

                        _context.BatteryCharacteristics.Add(newBatteryCharacteristic);
                    }
                }

                _context.SaveChanges();
                MessageBox.Show("Товар добавлен!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _imagePath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(_imagePath));
                ImageProduct.Source = bitmap;
                ImageUrlTextBox.Text = _imagePath;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonAddProduct_Click(sender, e);
            }
        }
    }
}