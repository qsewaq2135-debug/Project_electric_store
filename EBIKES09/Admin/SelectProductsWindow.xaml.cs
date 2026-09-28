using EBIKES09.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EBIKES09
{
    public partial class SelectProductsPage : Page
    {
        private Bdebikes09Context _context;
        public List<int> SelectedProductIds { get; private set; }

        public SelectProductsPage()
        {
            InitializeComponent();
            _context = new Bdebikes09Context();
            SelectedProductIds = new List<int>();
            LoadProducts();
        }

        private void LoadProducts()
        {
            var products = _context.Products.ToList();
            ProductsDataGrid.ItemsSource = products;
        }

        private void ButtonSaveProducts_Click(object sender, RoutedEventArgs e)
        {
            SelectedProductIds.Clear();
            foreach (var item in ProductsDataGrid.SelectedItems)
            {
                if (item is Product product)
                {
                    SelectedProductIds.Add(product.ProductId);
                }
            }
            
            NavigationService.GoBack();
        }
    }
}