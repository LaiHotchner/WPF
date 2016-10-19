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

                    for (var i = 0; i <= totalCount; i++)
                    {
                        if (i % 2 == 0)
                        {
                            backBufferGraphics.FillRectangle(GetFillBrush(i), i * interval, (float)MainWindow.TopDistance, interval / 2, (float)MainWindow.DrawHeight);
                        }
                        else
                        {
                            backBufferGraphics.FillRectangle(GetFillBrush(i), i * interval, (float)MainWindow.TopDistance, interval / 2, (float)MainWindow.DrawHeight);
                        }
                    }
                    backBufferGraphics.Flush();
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
            _bitmap.Unlock();
        }
        private GDI.Brush GetFillBrush(int index)
        {
            if (index % 2 == 0)
            {
                return new GDI.SolidBrush(GDI.Color.FromArgb(0xff, 0xff, 0x00, 0x00));
                var startColor = GDI.Color.FromArgb(0xff, 0xFF, 0xEE, 0x9F);
                var endColor = GDI.Color.FromArgb(0xff, 0xE2, 0xB4, 0x5F);
                var background = new GDI.Drawing2D.LinearGradientBrush(new GDI.PointF(0.5f, 0f), new GDI.PointF(0.5f, 1.0f), startColor, endColor);
                return background;
            }
            else
            {
                return new GDI.SolidBrush(GDI.Color.FromArgb(0xff, 0x00, 0xff, 0x00));
                //return new GDI.SolidBrush(GDI.Color.Blue);
            }
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
            //backBufferGraphics.FillRectangle(GetBackground(), new GDI.RectangleF(0, 0, _width, _height));
        }

        //private GDI.Brush GetBackground()
        //{
        //    var startColor = GDI.Color.FromArgb(0xCC, 0x18, 0x18, 0x18);
        //    var endColor = GDI.Color.FromArgb(0xCC, 0x4d, 0x4d, 0x4d);
        //    var background = new GDI.Drawing2D.LinearGradientBrush(new GDI.PointF(_width, 0), new GDI.PointF(_width, _height), startColor, endColor);
        //    return background;
        //}
    }
}
