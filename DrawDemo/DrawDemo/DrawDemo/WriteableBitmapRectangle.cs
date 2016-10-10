using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GDI = System.Drawing;

namespace DrawDemo
{
    public class WriteableBitmapRectangle : FrameworkElement
    {
        #region DependencyProperties

        public static readonly DependencyProperty DrawCountProperty =
            DependencyProperty.Register("DrawCount", typeof(int), typeof(WriteableBitmapRectangle),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.None, OnDrawCountPropertyyChanged));

        private static void OnDrawCountPropertyyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WriteableBitmapRectangle trendLine = (WriteableBitmapRectangle)d;
            int drawCount;
            if (int.TryParse(e.NewValue.ToString(), out drawCount))
            {
                trendLine.DrawTrendLine(drawCount);
            }
        }

        public int DrawCount
        {
            get { return (int)GetValue(DrawCountProperty); }
            set { SetValue(DrawCountProperty, value); }
        }

        #endregion

        private int _width ;
        private int _height ;
        private WriteableBitmap _bitmap;
        private readonly GDI.Pen _pen = new GDI.Pen(GDI.Color.Orange);
        private readonly GDI.Pen _bluePen = new GDI.Pen(GDI.Color.Blue);

        private void DrawTrendLine(float totalCount)
        {
            if (double.IsNaN(totalCount))
                return;

            _bitmap.Lock();

            using (GDI.Bitmap backBufferBitmap = new GDI.Bitmap(_width, _height,
                _bitmap.BackBufferStride, GDI.Imaging.PixelFormat.Format24bppRgb,
                _bitmap.BackBuffer))
            {
                using (GDI.Graphics backBufferGraphics = GDI.Graphics.FromImage(backBufferBitmap))
                {
                    backBufferGraphics.SmoothingMode = GDI.Drawing2D.SmoothingMode.HighSpeed;
                    backBufferGraphics.CompositingQuality = GDI.Drawing2D.CompositingQuality.HighSpeed;

                    backBufferGraphics.Clear(GDI.Color.Transparent);

                    float interval = _width / totalCount;

                    for (int i = 0; i <= totalCount; i++)
                    {
                        if (i % 2 == 0)
                        {
                            backBufferGraphics.DrawRectangle(_bluePen, i * interval, (float)MainWindow.TopDistance, interval, (float)MainWindow.DrawHeight);
                        }
                        else
                        {
                            backBufferGraphics.DrawRectangle(_pen, i * interval, (float)MainWindow.TopDistance, interval, (float)MainWindow.DrawHeight);
                        }
                    }
                    backBufferGraphics.Flush();
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
            _bitmap.Unlock();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_bitmap == null)
            {
                _width = (int)RenderSize.Width;
                _height = (int)RenderSize.Height;
                _bitmap = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Bgr24, null);

                _bitmap.Lock();
                using (GDI.Bitmap backBufferBitmap = new GDI.Bitmap(_width, _height,
                    _bitmap.BackBufferStride, GDI.Imaging.PixelFormat.Format24bppRgb,
                    _bitmap.BackBuffer))
                {
                    using (GDI.Graphics backBufferGraphics = GDI.Graphics.FromImage(backBufferBitmap))
                    {
                        backBufferGraphics.SmoothingMode = GDI.Drawing2D.SmoothingMode.HighSpeed;
                        backBufferGraphics.CompositingQuality = GDI.Drawing2D.CompositingQuality.HighSpeed;

                        backBufferGraphics.Clear(GDI.Color.White);

                        backBufferGraphics.Flush();
                    }
                }
                _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
                _bitmap.Unlock();
            }
            dc.DrawImage(_bitmap, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
            base.OnRender(dc);
        }
    }
}
