using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GDI = System.Drawing;

namespace DrawDemo
{
    public class WriteableBitmapOpacityMask : System.Windows.Controls.Image
    {
        private int _width;
        private int _height;
        private WriteableBitmap _bitmap;

        public void DrawTrendLine(float totalCount)
        {
            if (double.IsNaN(totalCount))
                return;

            var colorBursh = new System.Windows.Media.LinearGradientBrush();
            colorBursh.StartPoint = new Point(0, 0);
            colorBursh.EndPoint = new Point(1, 0);

            var gradientStopCollection = new GradientStopCollection();
            var brush = GetOpacityBackground();
            float interval = _width / totalCount;
            for (int i = 0; i <= totalCount; i++)
            {
                var stopOne = new GradientStop(Color.FromArgb(255, 0, 0, 0), i * interval / _width);
                var stopOneEnd = new GradientStop(Color.FromArgb(255, 0, 0, 0), (i * interval + interval / 2) / _width);
                var stopTwo = new GradientStop(Color.FromArgb(0, 0, 0, 0), (i * interval + interval / 2) / _width);
                var stopTwoEnd = new GradientStop(Color.FromArgb(0, 0, 0, 0), (i * interval + interval) / _width);
                gradientStopCollection.Add(stopOne);
                gradientStopCollection.Add(stopOneEnd);
                gradientStopCollection.Add(stopTwo);
                gradientStopCollection.Add(stopTwoEnd);
            }
            colorBursh.GradientStops = gradientStopCollection;
            OpacityMask = colorBursh;
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_bitmap == null)
            {
                _width = (int)(RenderSize.Width == 0 ? 1000 : RenderSize.Width);
                _height = (int)(RenderSize.Height == 0 ? 100 : RenderSize.Height);
                _bitmap = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Bgr24, null);

                _bitmap.Lock();

                using (GDI.Bitmap backBufferBitmap = new GDI.Bitmap(_width, _height,
                    _bitmap.BackBufferStride, GDI.Imaging.PixelFormat.Format24bppRgb,
                    _bitmap.BackBuffer))
                {
                    using (GDI.Graphics backBufferGraphics = GDI.Graphics.FromImage(backBufferBitmap))
                    {
                        ExecuteClear(backBufferGraphics);
                        backBufferGraphics.Flush();
                    }
                }
                _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
                _bitmap.Unlock();
                Source = _bitmap;
            }
            base.OnRender(dc);
        }

        public void Clear()
        {
            OpacityMask = new SolidColorBrush(Colors.Transparent);
        }

        private GDI.Brush GetOpacityBackground()
        {
            var startColor = GDI.Color.FromArgb(0xCC, 0x18, 0x18, 0x18);
            var endColor = GDI.Color.FromArgb(0xCC, 0x4d, 0x4d, 0x4d);
            var background = new GDI.Drawing2D.LinearGradientBrush(new GDI.PointF(_width, 0), new GDI.PointF(_width, _height), startColor, endColor);
            return background;
        }

        private void ExecuteClear(GDI.Graphics backBufferGraphics)
        {
            backBufferGraphics.SmoothingMode = SmoothingMode.HighSpeed;
            backBufferGraphics.CompositingQuality = CompositingQuality.HighSpeed;
            backBufferGraphics.FillRectangle(GetFillBrush(0), new GDI.RectangleF(0, 0, _width, _height));
        }

        private GDI.Brush GetFillBrush(int index)
        {
            if (index % 2 == 0)
            {
                var startColor = GDI.Color.FromArgb(0xCC, 0xFF, 0xEE, 0x9F);
                var endColor = GDI.Color.FromArgb(0xCC, 0xE2, 0xB4, 0x5F);
                var background = new GDI.Drawing2D.LinearGradientBrush(new GDI.PointF(0.5f, 0f), new GDI.PointF(0.5f, 1.0f), startColor, endColor);
                return background;
            }
            else
            {
                return new GDI.SolidBrush(GDI.Color.Blue);
            }
        }
    }
}
