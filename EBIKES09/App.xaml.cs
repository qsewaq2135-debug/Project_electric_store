using System.Configuration;
using System.Data;
using System.Windows;

namespace EBIKES09
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            using Bdebikes09Context context = new Bdebikes09Context();
            MainWindow logWindow = new MainWindow(context);
            logWindow.Show();
        }

    }

}
