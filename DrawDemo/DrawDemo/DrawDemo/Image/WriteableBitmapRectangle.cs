using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GDI = System.Drawing;

namespace DrawDemo
{
    public class WriteableBitmapRectangle : System.Windows.Controls.Image
    {
        private int _width;
        private int _height;
        private WriteableBitmap _bitmap;

        protected override void OnRender(DrawingContext dc)
        {
            if (_bitmap == null)
            {
                _width = (int)(RenderSize.Width == 0 ? 1920 : RenderSize.Width);
                _height = (int)(RenderSize.Height == 0 ? 100 : RenderSize.Height);
                _bitmap = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Bgra32, null);

                _bitmap.Lock();

                using (GDI.Bitmap backBufferBitmap = new GDI.Bitmap(_width, _height,
                    _bitmap.BackBufferStride, GDI.Imaging.PixelFormat.Format24bppRgb,
                    _bitmap.BackBuffer))
                {
                    using (GDI.Graphics backBufferGraphics = GDI.Graphics.FromImage(backBufferBitmap))
                    {
                        backBufferGraphics.SmoothingMode = SmoothingMode.HighSpeed;
                        backBufferGraphics.CompositingQuality = CompositingQuality.HighSpeed;

                        ExecuteClear(backBufferGraphics);

                        backBufferGraphics.Flush();
                    }
                }
                _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
                _bitmap.Unlock();
                Source = _bitmap;
            }

            //dc.DrawImage(_bitmap, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
            base.OnRender(dc);
        }

        public void DrawTrendLine(float totalCount)
        {
            if (double.IsNaN(totalCount))
                return;

            _bitmap.Lock();

            using (var backBufferBitmap = new GDI.Bitmap(_width, _height,
                _bitmap.BackBufferStride, GDI.Imaging.PixelFormat.Format32bppArgb,
                _bitmap.BackBuffer))
            {
                using (var backBufferGraphics = GDI.Graphics.FromImage(backBufferBitmap))
                {
                    backBufferGraphics.SmoothingMode = SmoothingMode.HighSpeed;
                    backBufferGraphics.CompositingQuality = CompositingQuality.HighSpeed;

                    ExecuteClear(backBufferGraphics);

                    var interval = _width / totalCount;
                    var width = interval;
                    var height = (float)MainWindow.DrawHeight;
                    var top = (float)MainWindow.TopDistance;

                    for (var i = 0; i <= totalCount; i++)
                    {
                        var left = i * interval;
                        var background = GetFillBrush(i, width, height);
                        backBufferGraphics.FillRectangle(background, left, top, width, height);
                    }
                    backBufferGraphics.Flush();
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
            _bitmap.Unlock();
        }

        private GDI.Brush GetFillBrush(int index, float width, float height)
        {
            GDI.Color startColor;
            GDI.Color endColor;
            if (index % 2 == 0)
            {
                startColor = GDI.Color.FromArgb(0xCC, 0xFF, 0xEE, 0x9F);
                endColor = GDI.Color.FromArgb(0xCC, 0xE2, 0xB4, 0x5F);
            }
            else
            {
                startColor = GDI.Color.FromArgb(0xCC, 0x45, 0x9E, 0x7E);
                endColor = GDI.Color.FromArgb(0xCC, 0x06, 0x73, 0x4A);
            }

            var rect = new GDI.RectangleF(0f, 0f, 1, height * 2);
            var background = new GDI.Drawing2D.LinearGradientBrush(rect, startColor, endColor, 90f);
            return background;
        }

        public void Clear()
        {
            if (_bitmap != null)
            {
                _bitmap.Lock();
                using (GDI.Bitmap backBufferBitmap = new GDI.Bitmap(_width, _height,
                    _bitmap.BackBufferStride, GDI.Imaging.PixelFormat.Format32bppArgb,
                    _bitmap.BackBuffer))
                {
                    using (GDI.Graphics backBufferGraphics = GDI.Graphics.FromImage(backBufferBitmap))
                    {
                        ExecuteClear(backBufferGraphics);
                    }
                }
                _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
                _bitmap.Unlock();
            }
        }

        private void ExecuteClear(GDI.Graphics backBufferGraphics)
        {
            backBufferGraphics.Clear(GDI.Color.Transparent);
        }
    }
}
