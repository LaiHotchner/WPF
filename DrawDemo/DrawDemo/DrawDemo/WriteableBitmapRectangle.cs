using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GDI = System.Drawing;

namespace DrawDemo
{
    public class WriteableBitmapRectangle : FrameworkElement
    {
        #region DependencyProperties

        public static readonly DependencyProperty LatestQuoteProperty =
            DependencyProperty.Register("LatestQuote", typeof(MinuteQuoteViewModel), typeof(WriteableBitmapRectangle),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnLatestQuotePropertyChanged));

        private static void OnLatestQuotePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WriteableBitmapRectangle trendLine = (WriteableBitmapRectangle)d;
            MinuteQuoteViewModel latestQuote = (MinuteQuoteViewModel)e.NewValue;
            if (latestQuote != null)
            {
                trendLine.DrawTrendLine((float)latestQuote.LastPx);
            }
        }

        public MinuteQuoteViewModel LatestQuote
        {
            get { return (MinuteQuoteViewModel)GetValue(LatestQuoteProperty); }
            set { SetValue(LatestQuoteProperty, value); }
        }

        #endregion

        private int width = 0;
        private int height = 0;

        private WriteableBitmap bitmap;
        
        private GDI.Pen pen = new GDI.Pen(GDI.Color.Orange);
        private GDI.Pen bluePen = new GDI.Pen(GDI.Color.Blue);

        private void DrawTrendLine(float totalCount)
        {
            if (double.IsNaN(totalCount))
                return;

            this.bitmap.Lock();

            using (GDI.Bitmap backBufferBitmap = new GDI.Bitmap(width, height,
                this.bitmap.BackBufferStride, GDI.Imaging.PixelFormat.Format24bppRgb,
                this.bitmap.BackBuffer))
            {
                using (GDI.Graphics backBufferGraphics = GDI.Graphics.FromImage(backBufferBitmap))
                {
                    backBufferGraphics.SmoothingMode = GDI.Drawing2D.SmoothingMode.HighSpeed;
                    backBufferGraphics.CompositingQuality = GDI.Drawing2D.CompositingQuality.HighSpeed;

                    backBufferGraphics.Clear(GDI.Color.Transparent);

                    float interval = width / totalCount;

                    for (int i = 0; i <= totalCount; i++)
                    {
                        if (i % 2 == 0)
                        {
                            backBufferGraphics.DrawRectangle(bluePen, i * interval, 100f, interval, 20f);
                        }
                        else
                        {
                            backBufferGraphics.DrawRectangle(pen, i * interval, 100f, interval, 20f);
                        }
                    }
                    backBufferGraphics.Flush();
                }
            }
            this.bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            this.bitmap.Unlock();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (bitmap == null)
            {
                this.width = (int)RenderSize.Width;
                this.height = (int)RenderSize.Height;
                this.bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);

                this.bitmap.Lock();
                using (GDI.Bitmap backBufferBitmap = new GDI.Bitmap(width, height,
                    this.bitmap.BackBufferStride, GDI.Imaging.PixelFormat.Format24bppRgb,
                    this.bitmap.BackBuffer))
                {
                    using (GDI.Graphics backBufferGraphics = GDI.Graphics.FromImage(backBufferBitmap))
                    {
                        backBufferGraphics.SmoothingMode = GDI.Drawing2D.SmoothingMode.HighSpeed;
                        backBufferGraphics.CompositingQuality = GDI.Drawing2D.CompositingQuality.HighSpeed;

                        backBufferGraphics.Clear(GDI.Color.White);

                        backBufferGraphics.Flush();
                    }
                }
                this.bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
                this.bitmap.Unlock();
            }
            dc.DrawImage(bitmap, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
            base.OnRender(dc);
        }
    }

    public class MinuteQuoteViewModel : INotifyPropertyChanged
    {
        private double lastPx = double.NaN;
        public double LastPx
        {
            get { return this.lastPx; }
            set { if (this.lastPx != value) { this.lastPx = value; this.OnPropertyChanged("LastPx"); } }
        }

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
