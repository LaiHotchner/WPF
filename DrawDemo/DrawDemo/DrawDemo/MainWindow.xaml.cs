using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DrawDemo
{
    /// <summary> 
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> _drawType = new ObservableCollection<string>();
        private Stopwatch watch = new Stopwatch();
        private Brush greenBrush = Brushes.LightGreen;
        private Brush redBrush = Brushes.Red;
        private const double HEIGHT = 50;
        private const double TOP_DISTANCE = 50;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _drawType.Add("Draw with rectangle ");
            _drawType.Add("Draw with visual independent, DrawingContext.DrawRectangle");
            _drawType.Add("Draw with visual compositional, DrawingContext.DrawRectangle ");
            _drawType.Add("Draw with visual compositional, DrawingContext.DrawGeometry ");
            _drawType.Add("Draw with WriteableBitmap (GDI+)");
            Model.ItemsSource = _drawType;
            Model.SelectedIndex = 0;
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            var drawModel = (DrawModel)Model.SelectedIndex;
            int count;
            var flag = int.TryParse(Count.Text, out count);
            if (!flag)
            {
                MessageBox.Show("Please input draw count!");
                return;
            }

            DrawCanvas.Children.Clear();

            Draw(drawModel, count);
        }

        private void Draw(DrawModel drawModel, int count)
        {
            watch = new Stopwatch();
            watch.Start();

            switch (drawModel)
            {
                case DrawModel.Rectangle:
                    DrawRectangle(count);
                    break;
                case DrawModel.Independent_DrawRectangle:
                    DrawVisualIndependent_DrawRectangle(count);
                    break;
                case DrawModel.Compositional_DrawRectangle:
                    DrawCompositional_DrawRectangle(count);
                    break;
                case DrawModel.Compositional_DrawGeometry:
                    DrawCompositional_DrawGeometry(count);
                    break;
                case DrawModel.WriteableBitmap:
                    DrawWriteableBitmap(count);
                    break;
            }
            watch.Stop();
            TotalTime.Content = string.Format("Total spend time : {0} ms", watch.ElapsedMilliseconds);
        }

        private void DrawRectangle(int count)
        {
            var totalWidth = DrawCanvas.ActualWidth;
            var interval = totalWidth / count;
            var position = 0.0;
            for (int i = 0; i < count; i++)
            {
                var insertShape = new Rectangle()
                {
                    StrokeThickness = 0,
                    Width = interval,
                    Height = HEIGHT,
                };
                if (i % 2 == 0)
                {
                    insertShape.Fill = greenBrush;
                }
                else
                {
                    insertShape.Fill = redBrush;
                }

                Canvas.SetLeft(insertShape, position);
                Canvas.SetTop(insertShape, TOP_DISTANCE);
                DrawCanvas.Children.Add(insertShape);
                position += interval;
            }
        }

        private void DrawVisualIndependent_DrawRectangle(int count)
        {
            var totalWidth = DrawCanvas.ActualWidth;
            var interval = totalWidth / count;
            var position = 0.0;
            for (int i = 0; i < count; i++)
            {
                Brush brush;
                if (i % 2 == 0)
                {
                    brush = greenBrush;
                }
                else
                {
                    brush = redBrush;
                }
                var oneVisual = new OneElementOneVisual();
                oneVisual.DrawRectangle(brush, position, TOP_DISTANCE, interval, HEIGHT);

                position += interval;
                DrawCanvas.Children.Add(oneVisual);
            }
        }

        private void DrawCompositional_DrawRectangle(int count)
        {
            var multiVisual = new OneElementMultiVisual();
            multiVisual.DrawRectangle(greenBrush, redBrush, DrawCanvas.ActualWidth, HEIGHT, TOP_DISTANCE, count);
            DrawCanvas.Children.Add(multiVisual);
        }

        private void DrawCompositional_DrawGeometry(int count)
        {
            var multiVisualGeometry = new OneElementMultiVisualGeometry();
            multiVisualGeometry.DrawRectangle(greenBrush, redBrush, DrawCanvas.ActualWidth, HEIGHT, TOP_DISTANCE, count);
            DrawCanvas.Children.Add(multiVisualGeometry);
        }

        private void DrawWriteableBitmap(int count)
        {
            var vm = new MinuteQuoteViewModel();
            vm.LastPx = count;
            bitmap.LatestQuote = vm;
        }

    }

    public enum DrawModel
    {
        Rectangle = 0,
        Independent_DrawRectangle = 1,
        Compositional_DrawRectangle = 2,
        Compositional_DrawGeometry = 3,
        WriteableBitmap = 4,
    }
}
