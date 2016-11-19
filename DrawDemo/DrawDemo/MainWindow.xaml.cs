using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DrawDemo
{
    /// <summary> 
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public const double DrawHeight = 50;
        public const double TopDistance = 25;
        private readonly Brush _greenBrush = Brushes.LightGreen;
        private readonly Brush _redBrush = Brushes.Red;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            TotalTime.Text = "Calculating ...";
            int count;
            var flag = int.TryParse(DrawCount.Text, out count);
            if (!flag)
            {
                MessageBox.Show("Please input draw count!");
                return;
            }

            int drawTimes;
            flag = int.TryParse(DrawTimes.Text, out drawTimes);
            if (!flag)
            {
                MessageBox.Show("Please input draw times!");
                return;
            }

            Draw(drawTimes, count);
        }

        private void Draw(int drawTimes, int count)
        {
            double drawMultiRectAverageTime;
            double drawMultiElementAverageTime;
            double drawOneElementMultiRectAverageTime;
            double drawOneElementMultiGeometryAverageTime;
            double drawBitAverageTime;

            drawMultiRectAverageTime = ExecuteTime(drawTimes, count, DrawMultiRectangle);
            drawMultiElementAverageTime = ExecuteTime(drawTimes, count, DrawMultiElement);
            drawOneElementMultiRectAverageTime = ExecuteTime(drawTimes, count, DrawOneElementMultiRect);
            drawOneElementMultiGeometryAverageTime = ExecuteTime(drawTimes, count, DrawOneElementMultiGeometry);
            drawBitAverageTime = ExecuteTime(drawTimes, count, DrawWriteableBitmap);

            TotalTime.Text = "Draw Times : " + drawTimes + ", Draw Count : " + count + "\r\n" +
                             "Draw Multi Rect Average Time : " + drawMultiRectAverageTime + "ms \r\n" +
                             "Draw Multi Element Average Time : " + drawMultiElementAverageTime + "ms \r\n" +
                             "Draw One Element Multi Rect Average Time : " + drawOneElementMultiRectAverageTime +
                             "ms \r\n" +
                             "Draw One Element Multi Geometry Average Time : " + drawOneElementMultiGeometryAverageTime +
                             "ms \r\n" +
                             "Draw Bitmap Average Time : " + drawBitAverageTime + "ms \r\n";
        }

        private double ExecuteTime(int times, int count, Action<int> execute)
        {
            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < times; i++)
            {
                execute(count);
            }
            watch.Stop();
            return watch.ElapsedMilliseconds * 1.0 / times;
        }

        private void DrawMultiRectangle(int count)
        {
            Rect.Children.Clear();

            if (IsDrawMultiRect.IsChecked != null && IsDrawMultiRect.IsChecked.Value == false) return;

            var totalWidth = Rect.ActualWidth;
            var interval = totalWidth / count;
            var position = 0.0;
            for (int i = 0; i < count; i++)
            {
                var insertShape = new Rectangle()
                {
                    StrokeThickness = 0,
                    Width = interval,
                    Height = DrawHeight,
                };
                if (i % 2 == 0)
                {
                    insertShape.Fill = _greenBrush;
                }
                else
                {
                    insertShape.Fill = _redBrush;
                }

                Canvas.SetLeft(insertShape, position);
                Canvas.SetTop(insertShape, TopDistance);
                Rect.Children.Add(insertShape);
                position += interval;
            }
        }

        private void DrawMultiElement(int count)
        {
            MultiElement.Children.Clear();
            if (IsDrawMultiElement.IsChecked != null && IsDrawMultiElement.IsChecked.Value == false) return;

            var totalWidth = MultiElement.ActualWidth;
            var interval = totalWidth / count;
            var position = 0.0;
            for (int i = 0; i < count; i++)
            {
                Brush brush = GetTimelineLayerBackground(i);
                var oneVisual = new OneElementOneVisual();
                oneVisual.DrawRectangle(brush, position, TopDistance, interval, DrawHeight);

                position += interval;
                MultiElement.Children.Add(oneVisual);
            }
        }

        private void DrawOneElementMultiRect(int count)
        {
            OneElementMultiRect.Children.Clear();
            if (IsDrawOneElementMultiRect.IsChecked != null && IsDrawOneElementMultiRect.IsChecked.Value == false)
                return;

            var multiVisual = new OneElementVisualMultiRect();
            multiVisual.DrawRectangle(_greenBrush, _redBrush, OneElementMultiRect.ActualWidth, DrawHeight, TopDistance,
                count);
            OneElementMultiRect.Children.Add(multiVisual);
        }

        private void DrawOneElementMultiGeometry(int count)
        {
            OneElementMultiGeometry.Children.Clear();
            if (IsDrawOneElementMultiVisual.IsChecked != null && IsDrawOneElementMultiVisual.IsChecked.Value == false)
                return;

            var multiVisualGeometry = new OneElementVisualMultiGeometry();
            multiVisualGeometry.DrawRectangle(_greenBrush, _redBrush, OneElementMultiGeometry.ActualWidth, DrawHeight,
                TopDistance, count);
            OneElementMultiGeometry.Children.Add(multiVisualGeometry);
        }

        private void DrawWriteableBitmap(int count)
        {
            bitmap.Clear();
            //bitmapOpacity.Clear();

            if (IsDrawBitMap.IsChecked != null && IsDrawBitMap.IsChecked.Value == false) return;

            bitmap.DrawTrendLine(count);
            //bitmapOpacity.DrawTrendLine(count);
        }


        private Brush GetTimelineLayerBackground(int index)
        {
            Brush background;
            Color startColor;
            Color endColor;

            if (index % 2 == 0)
            {
                startColor = Color.FromArgb(0xCC, 0xFF, 0xEE, 0x9F);
                endColor = Color.FromArgb(0xCC, 0xE2, 0xB4, 0x5F);
            }
            else
            {
                startColor = Color.FromArgb(0xCC, 0x45, 0x9E, 0x7E);
                endColor = Color.FromArgb(0xCC, 0x06, 0x73, 0x4A);
            }

            background = new LinearGradientBrush(startColor, endColor, new Point(0.5, 0), new Point(0.5, 1.0));
            if (background.CanFreeze)
            {
                background.Freeze();
            }
            return background;
        }
    }
}
