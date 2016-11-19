using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DrawDemo
{
    public class OneElementOneVisual : FrameworkElement
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
        public void DrawRectangle(Brush color, double x, double y, double width, double height)
        {
            DrawingVisual visual = new DrawingVisual();
            DrawingContext dc = visual.RenderOpen();

            var rect = new Rect(x, y, width, height);
            dc.DrawRectangle(color, null, rect);

            dc.Close();
            AddVisual(visual);
        }
    }
}
