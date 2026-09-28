using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace EBIKES09
{
    
    public partial class EditeProductPage : Page
    {
        private Bdebikes09Context _context;
        private Product _product;
        private Product _productB;
        private string _imagePath;
        public EditeProductPage(int productId)
        {
            InitializeComponent();
            var dbContextOptions = new DbContextOptionsBuilder<Bdebikes09Context>()
                 .UseNpgsql()
                 .Options;
            _context = new Bdebikes09Context(dbContextOptions);
            _product = _context.Products
              .Include(p => p.BikeCharacteristic)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.BatteryCharacteristic)
                .Include(p => p.BatteryCharacteristic.BatteryType)
                .Include(p => p.BatteryCharacteristic.Voltage)
                .Include(p => p.BikeCharacteristic.FrameMaterial)
                .Include(p => p.BikeCharacteristic.FrontBrakes)
                .Include(p => p.BikeCharacteristic.RearBrakes)
                 .Include(p => p.BikeCharacteristic.Drive)
                .FirstOrDefault(p => p.ProductId == productId);

            if (_product != null)
            {
                LoadProductData();
                LoadAll();
            }
            
            else
            {
                MessageBox.Show("Товар не найден");
                NavigationService.GoBack();
            }

            if (_product.CategoryId == 1)
            {
                StackPanel2.Visibility = Visibility.Collapsed;
                StackPanel1.Visibility = Visibility.Visible;
            }
            else
            {
                StackPanel2.Visibility = Visibility.Visible;
                StackPanel1.Visibility = Visibility.Collapsed;
            }
        }
        private void LoadAll()
        {
            if (_context == null) return;
            var category = _context.Categories.Select(c => c.CategoryName).ToList();
            CategoryComboBox.ItemsSource = category;
            var brand = _context.Brands.Select(c => c.BrandName).ToList();
            BrandComboBox.ItemsSource = brand;
            var frameMaterial = _context.FrameMaterials.Select(c => c.FrameMaterialName).ToList();
            FrameMaterialComboBox.ItemsSource = frameMaterial;
            var frontBrakes = _context.FrontBrakes.Select(c => c.FrontBrakesName).ToList();
            FrontBrakesComboBox.ItemsSource = frontBrakes;
            var rearBrakes = _context.RearBrakes.Select(c => c.RearBrakesName).ToList();
            RearBrakesComboBox.ItemsSource = rearBrakes;
            var drive = _context.Drives.Select(c => c.DriveName).ToList();
            DriveComboBox.ItemsSource = drive;
            var volage = _context.Voltages.Select(c => c.VoltageName).ToList();
            VoltageComboBox.ItemsSource = volage;
            var batteryType = _context.BatteryTypes.Select(c => c.BatteryType1).ToList();
            BatteryTypeComboBox.ItemsSource= batteryType;
        }
        private void LoadProductData()
        {
            if (_product == null) return;
            
            ProductNameTextBox.Text = _product.ProductName;
            DescriptionTextBox.Text = _product.Description;
            PriceTextBox.Text = _product.Price.ToString();
            StockQuantityTextBox.Text = _product.StockQuantity.ToString();
            ImageUrlTextBox.Text = _product.ImageUrl;
            CategoryComboBox.SelectedItem = _product.Category.CategoryName;
            BrandComboBox.SelectedItem = _product.Brand.BrandName;
            if (_product.ImageUrl != null)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_product.ImageUrl);
                bitmap.EndInit();
                ImageProduct.Source = bitmap;
            }
            if (_product.BatteryCharacteristic != null)
            {
                NominalCapacityTextBox.Text = _product.BatteryCharacteristic.NominalCapacity.ToString();
                ServiceLifeBoxTextBox.Text = _product.BatteryCharacteristic.ServiceLife.ToString();

                if (_product.BatteryCharacteristic.Voltage != null)
                {
                    VoltageComboBox.SelectedItem = _product.BatteryCharacteristic.Voltage.VoltageName;
                }

                if (_product.BatteryCharacteristic.BatteryType != null)
                {
                    BatteryTypeComboBox.SelectedItem = _product.BatteryCharacteristic.BatteryType.BatteryType1;
                }
            }
            if (_product.BikeCharacteristic != null)
            {

                MaximumSpeedTextBox.Text = _product.BikeCharacteristic.MaximumSpeed.ToString();
                MaximumRangeTextBox.Text = _product.BikeCharacteristic.MaximumRange.ToString();
                MaxLoadTextBox.Text = _product.BikeCharacteristic.MaxLoad.ToString();
                MotorPowerTextBox.Text = _product.BikeCharacteristic.MotorPower.ToString();
                VoltageTextBox.Text = _product.BikeCharacteristic.Voltage.ToString();
                CapacityTextBox.Text = _product.BikeCharacteristic.Capacity.ToString();
                FullChargeTimeTextBox.Text = _product.BikeCharacteristic.FullChargeTime.ToString();
                WheelDiameterTextBox.Text = _product.BikeCharacteristic.WheelDiameter.ToString();
                NumberOfSpeedsTextBox.Text = _product.BikeCharacteristic.NumberOfSpeeds.ToString();
                if (_product.BikeCharacteristic.FrameMaterial != null)
                {
                    FrameMaterialComboBox.SelectedItem = _product.BikeCharacteristic.FrameMaterial.FrameMaterialName;
                }
                if (_product.BikeCharacteristic.FrontBrakes != null)
                {
                    FrontBrakesComboBox.SelectedItem = _product.BikeCharacteristic.FrontBrakes.FrontBrakesName;
                }
                if (_product.BikeCharacteristic.RearBrakes != null)
                {
                    RearBrakesComboBox.SelectedItem = _product.BikeCharacteristic.RearBrakes.RearBrakesName;
                }
                if (_product.BikeCharacteristic.Drive != null)
                {
                    DriveComboBox.SelectedItem = _product.BikeCharacteristic.Drive.DriveName;
                }

                WeightTextBox.Text = _product.BikeCharacteristic.Weight;
                DimensionsTextBox.Text = _product.BikeCharacteristic.Dimensions;
                PackageDimensionsTextBox.Text = _product.BikeCharacteristic.PackageDimensions;
                SoundSignalCheckBox.IsChecked = _product.BikeCharacteristic.SoundSignal;
                WingsCheckBox.IsChecked = _product.BikeCharacteristic.Wings;
            }
        }
        private void ButtonSaveChanges_Click(object sender, RoutedEventArgs e)
{
    try
    {
        if (_product == null)
        {
            MessageBox.Show("Продукт не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var category = _context.Categories.FirstOrDefault(c => c.CategoryName == CategoryComboBox.SelectedItem.ToString());
        var brand = _context.Brands.FirstOrDefault(b => b.BrandName == BrandComboBox.SelectedItem.ToString());

        _product.ProductName = ProductNameTextBox.Text;
        _product.Description = DescriptionTextBox.Text;
        _product.Price = decimal.Parse(PriceTextBox.Text);
        _product.StockQuantity = int.Parse(StockQuantityTextBox.Text);
        _product.ImageUrl = ImageUrlTextBox.Text;
        _product.CategoryId = category.CategoryId;
        _product.BrandId = brand.BrandId;

        if (_product.Category.CategoryName == "Электровелосипед") 
        {
            var frameMaterial = _context.FrameMaterials.FirstOrDefault(f => f.FrameMaterialName == FrameMaterialComboBox.SelectedItem.ToString());
            var frontBrakes = _context.FrontBrakes.FirstOrDefault(fb => fb.FrontBrakesName == FrontBrakesComboBox.SelectedItem.ToString());
            var rearBrakes = _context.RearBrakes.FirstOrDefault(rb => rb.RearBrakesName == RearBrakesComboBox.SelectedItem.ToString());
            var drive = _context.Drives.FirstOrDefault(d => d.DriveName == DriveComboBox.SelectedItem.ToString());

            if (_product.BikeCharacteristic == null)
            {
                _product.BikeCharacteristic = new BikeCharacteristic();
            }

            _product.BikeCharacteristic.MaximumSpeed = int.Parse(MaximumSpeedTextBox.Text);
            _product.BikeCharacteristic.MaximumRange = int.Parse(MaximumRangeTextBox.Text);
            _product.BikeCharacteristic.MaxLoad = int.Parse(MaxLoadTextBox.Text);
            _product.BikeCharacteristic.MotorPower = int.Parse(MotorPowerTextBox.Text);
            _product.BikeCharacteristic.Voltage = int.Parse(VoltageTextBox.Text);
            _product.BikeCharacteristic.Capacity = int.Parse(CapacityTextBox.Text);
            _product.BikeCharacteristic.FullChargeTime = int.Parse(FullChargeTimeTextBox.Text);
            _product.BikeCharacteristic.WheelDiameter = int.Parse(WheelDiameterTextBox.Text);
            _product.BikeCharacteristic.NumberOfSpeeds = int.Parse(NumberOfSpeedsTextBox.Text);
            _product.BikeCharacteristic.Weight = WeightTextBox.Text;
            _product.BikeCharacteristic.Dimensions = DimensionsTextBox.Text;
            _product.BikeCharacteristic.PackageDimensions = PackageDimensionsTextBox.Text;
            _product.BikeCharacteristic.SoundSignal = SoundSignalCheckBox.IsChecked;
            _product.BikeCharacteristic.Wings = WingsCheckBox.IsChecked;
            _product.BikeCharacteristic.FrameMaterialId = frameMaterial?.FrameMaterialId;
            _product.BikeCharacteristic.FrontBrakesId = frontBrakes?.FrontBrakesId;
            _product.BikeCharacteristic.RearBrakesId = rearBrakes?.RearBrakesId;
            _product.BikeCharacteristic.DriveId = drive?.DriveId;
        }
        else if (_product.Category.CategoryName == "Аккумулятор") 
        {
                    var selectedVoltageId = (int?)VoltageComboBox.SelectedValue;
                    var voltage = _context.Voltages.FirstOrDefault(v => v.VoltageId == selectedVoltageId);
                    var batteryType = _context.BatteryTypes.FirstOrDefault(bt => bt.BatteryType1 == BatteryTypeComboBox.SelectedItem.ToString());

            if (_product.BatteryCharacteristic == null)
            {
                _product.BatteryCharacteristic = new BatteryCharacteristic();
            }

            _product.BatteryCharacteristic.NominalCapacity = int.Parse(NominalCapacityTextBox.Text);
            _product.BatteryCharacteristic.ServiceLife = int.Parse(ServiceLifeBoxTextBox.Text);
            _product.BatteryCharacteristic.VoltageId = voltage?.VoltageId;
            _product.BatteryCharacteristic.BatteryTypeId = batteryType?.BatteryTypeId;
        }

        _context.SaveChanges();
        MessageBox.Show("Изменения сохранены.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        NavigationService.GoBack();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка сохранения товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonSaveChanges_Click(sender, e);
            }
        }
        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                _imagePath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_imagePath);
                bitmap.EndInit();
                ImageProduct.Source = bitmap;
                ImageUrlTextBox.Text = _imagePath;
            }
        }
    }
}