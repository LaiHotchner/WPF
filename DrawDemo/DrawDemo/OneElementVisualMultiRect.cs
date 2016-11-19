using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DrawDemo
{
    public class OneElementVisualMultiRect : FrameworkElement
    {
        private List<Visual> visuals = new List<Visual>();

        //获取Visual的个数
        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }

        //获取Visual
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        //添加Visual
        public void AddVisual(Visual visual)
        {
            visuals.Add(visual);

            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        //删除Visual
        public void RemoveVisual(Visual visual)
        {
            visuals.Remove(visual);

            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }

        //命中测试
        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, point);
            return hitResult.VisualHit as DrawingVisual;
        }

        //使用DrawVisual画Polyline
        public void DrawRectangle(Brush greenBrush, Brush redBrush, double totalWidth, double height, double topDistance, int count)
        {
            DrawingVisual visual = new DrawingVisual();
            DrawingContext dc = visual.RenderOpen();

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
                var rect = new Rect(position, topDistance, interval, height);
                dc.DrawRectangle(brush, null, rect);
                position += interval;
            }
            dc.Close();
            AddVisual(visual);
        }
    }
}
