using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DrawDemo
{
    public class OneElementVisualMultiGeometry : FrameworkElement
    {
        private List<Visual> visuals = new List<Visual>();
        private readonly Pen _pen = new Pen(new SolidColorBrush(Colors.Black), 1.0);

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

            var streamGemotry = new StreamGeometry();
            streamGemotry.FillRule = FillRule.EvenOdd;
            var streamGemotryContext = streamGemotry.Open();

            DrawGemotry(streamGemotryContext, totalWidth, height, topDistance, count);

            streamGemotryContext.Close();
            streamGemotry.Freeze();
            dc.DrawGeometry(greenBrush, _pen, streamGemotry);
            dc.Close();
            AddVisual(visual);

        }

        private void DrawGemotry(StreamGeometryContext streamGeometryContext, double totalWidth, double height, double topDistance, int count)
        {
            var interval = totalWidth / (count * 2);
            var position = 0.0;
            for (int i = 0; i < count; i++)
            {
                var firstPoint = new Point(position, topDistance);
                var secondPoint = new Point(position, height + topDistance);
                var thirdPoint = new Point(position + interval, height + topDistance);
                var fourthPoint = new Point(position + interval, topDistance);

                streamGeometryContext.BeginFigure(firstPoint, true, true);
                streamGeometryContext.LineTo(secondPoint, false, false);
                streamGeometryContext.LineTo(thirdPoint, false, false);
                streamGeometryContext.LineTo(fourthPoint, false, false);

                position += interval * 2;
            }
        }
    }
}
