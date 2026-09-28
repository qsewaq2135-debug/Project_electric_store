using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EBIKES09
{
    /// <summary>
    /// Логика взаимодействия для CostomeMessageBox.xaml
    /// </summary>
    public partial class CostomeMessageBox : Window
    {
        public CostomeMessageBox(string message, string title = "Сообщение", string copyTitle = "ОШИБКА", bool showCopyButton = true)
        {
            InitializeComponent();

            DataContext = new CustomMessageBoxViewModel(message, title, copyTitle, showCopyButton);
            Title = ((CustomMessageBoxViewModel)DataContext).Title;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Clipboard.SetText(((CustomMessageBoxViewModel)DataContext).CopyTitle);
                System.Windows.MessageBox.Show("Текст скопирован в буфер обмена!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при копировании: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
