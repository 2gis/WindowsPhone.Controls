using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void RunTest_Click(object sender, RoutedEventArgs e)
        {
            for (double progress = 0; progress <= 1; progress += 0.1)
            {
                DoubleGisProgress.Progress = progress;
                ProgressRing.Progress = progress;
                await Task.Delay(500);
            }
        }
    }
}
