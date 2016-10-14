using System.Windows;

namespace DrawDemo
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void MergeWindow_OnClick(object sender, RoutedEventArgs e)
        {
            var mergeWindow = new MergeDemo();
            mergeWindow.ShowDialog();
        }

        private void DrawWindow_OnClick(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow();
            main.ShowDialog();
        }
    }
}
