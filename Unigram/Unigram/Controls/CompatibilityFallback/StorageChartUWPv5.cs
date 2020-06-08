using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Unigram.Controls.CompatibilityFallback
{
    public class StorageChartUWPv5 : Panel
    {
        private const double CIRCLE = 360;
        private Canvas Canvas { get; }
        
        private IList<StorageChartItem> _items;
        public IList<StorageChartItem> Items
        {
            get => _items;
            set => SetItems(value);
        }
        public StorageChartUWPv5()
        {
            Canvas = new Canvas();
            Children.Add(new Viewbox()
            {
                StretchDirection = StretchDirection.Both,
                Stretch = Stretch.UniformToFill,
                Child = Canvas
            });
        }

        //TODO: I'm lazy: Should be private and use events to update (IsVisible, ItemsCollection)
        public void Update()
        {
            const double onePercentOf360Degrees = CIRCLE / 100;
            var angle = 0.0;
            var radius = Height / 2;
            var hole = Height / 2.25;
            var totalAbs = _items.Where(i => i.IsVisible).Sum(i => i.Size);

            Canvas.Height = Height;
            Canvas.Width = Width;
            Canvas.Children.Clear();

            foreach (var item in Items)
            {
                if (!item.IsVisible) continue;
                var percentage = (double)item.Size / totalAbs * 100;
                var sweep = onePercentOf360Degrees * percentage;
                Canvas.Children.Add(GetPath(ref angle, sweep, radius, hole, item.Stroke));
            }

            if (Canvas.Children.Count == 0)
                Canvas.Children.Add(GetPath(ref angle, CIRCLE, radius, hole, Items.FirstOrDefault()?.Stroke ?? Colors.WhiteSmoke));
        }

        private void SetItems(IList<StorageChartItem> items)
        {
            _items = items;
            Update();
        }

        private static Point GetPoint(double angle, double radius, double hole)
        {
            var radians = Math.PI / 180 * (angle - 90);
            var x = hole * Math.Cos(radians);
            var y = hole * Math.Sin(radians);
            return new Point(x + radius, y + radius);
        }

        private static Path GetPath(ref double angle, double sweep, double radius, double hole, Color fill)
        {
            if (sweep >= CIRCLE) sweep -= .00001; //Note: Single Item Workaround
            var isLargeArc = sweep > 180.0;
            var innerArcStart = GetPoint(angle, radius, hole);
            var innerArcEnd = GetPoint(angle + sweep, radius, hole);
            var innerArcSize = new Size(hole, hole);
            var outerArcStart = GetPoint(angle, radius, radius);
            var outerArcEnd = GetPoint(angle + sweep, radius, radius);
            var outerArcSize = new Size(radius, radius);
            var pathGeometry = new PathGeometry();

            var figure = new PathFigure()
            {
                StartPoint = innerArcStart,
                IsClosed = true,
                IsFilled = true
            };
            figure.Segments.Add(new LineSegment {Point = outerArcStart});
            figure.Segments.Add(new ArcSegment
            {
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = isLargeArc,
                Point = outerArcEnd,
                Size = outerArcSize
            });
            figure.Segments.Add(new LineSegment { Point = innerArcEnd });
            figure.Segments.Add(new ArcSegment()
            {
                SweepDirection = SweepDirection.Counterclockwise,
                IsLargeArc = isLargeArc,
                Point = innerArcStart,
                Size = innerArcSize
            });
            pathGeometry.Figures.Add(figure);
            angle += sweep;

            return new Path
            {
                Fill = new SolidColorBrush(fill),
                Data = pathGeometry
            };
        }
    }
}
