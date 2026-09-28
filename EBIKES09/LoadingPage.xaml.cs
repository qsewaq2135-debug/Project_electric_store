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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EBIKES09
{
    /// <summary>
    /// Логика взаимодействия для LoadingPage.xaml
    /// </summary>
    public partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            InitializeComponent();

            
            StartRotationAnimation();

            
            StartFadeInAnimation();
        }

        private void StartRotationAnimation()
        {
            
            var rotateAnimation = (Storyboard)FindResource("RotateAnimation");
            rotateAnimation.Begin(LoadingImage, true);
        }

        private void StartFadeInAnimation()
        {
            
            var fadeInAnimation = (Storyboard)FindResource("FadeInAnimation");
            fadeInAnimation.Begin(this);
        }

       
        public void UpdateProgress(int progress, string status = null)
        {
            Dispatcher.Invoke(() =>
            {
                
                ProgressText.Text = $"{progress}%";

               
                ProgressBarFill.Width = (progress / 100.0) * ProgressBarBackground.ActualWidth;

               
                if (!string.IsNullOrEmpty(status))
                {
                    LoadingText.Text = status;
                }
            });
        }

        
        public void CloseLoadingWindow()
        {
          
            var fadeOutAnimation = (Storyboard)FindResource("FadeOutAnimation");
            fadeOutAnimation.Completed += (s, e) => NavigationService.GoBack(); 
            fadeOutAnimation.Begin(this);
        }
    }
}
