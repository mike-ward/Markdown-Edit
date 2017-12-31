using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using Infrastructure;

namespace EditModule.Features.SpellCheck
{
    public class SpellCheckBackgroundRenderer : ISpellCheckBackgroundRenderer
    {
        public TextSegmentCollection<TextSegment> ErrorSegments { get; }

        public SpellCheckBackgroundRenderer()
        {
            ErrorSegments = new TextSegmentCollection<TextSegment>();
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            const int timeout = 100;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var color = GetSpellCheckColor();
            var pen = new Pen(new SolidColorBrush(color), 0.6);
            pen.Freeze();

            foreach (var current in ErrorSegments)
            {
                foreach (var current2 in BackgroundGeometryBuilder.GetRectsForSegment(textView, current))
                {
                    const double num = 2.0;
                    var bottomLeft = current2.BottomLeft;
                    var bottomRight = current2.BottomRight;
                    var count = Math.Max((int)((bottomRight.X - bottomLeft.X) / num) + 1, 4);
                    var streamGeometry = new StreamGeometry();
                    using (var streamGeometryContext = streamGeometry.Open())
                    {
                        streamGeometryContext.BeginFigure(bottomLeft, false, false);
                        streamGeometryContext.PolyLineTo(CreatePoints(bottomLeft, num, count).ToArray(), true, false);
                    }
                    streamGeometry.Freeze();
                    drawingContext.DrawGeometry(Brushes.Transparent, pen, streamGeometry);

                    if (stopwatch.ElapsedMilliseconds > timeout) break;
                }
                if (stopwatch.ElapsedMilliseconds > timeout) break;
            }

            stopwatch.Stop();
        }

        private static Color GetSpellCheckColor()
        {
            return Colors.Red;
            //Color color;
            //try
            //{
            //    color = (Color)(ColorConverter.ConvertFromString(App.UserSettings.Theme.SpellCheckError ?? "#f00") ?? Colors.Red);
            //}
            //catch (FormatException)
            //{
            //    color = Colors.Red;
            //}
            //return color;
        }

        public KnownLayer Layer => KnownLayer.Selection;

        private static IEnumerable<Point> CreatePoints(Point start, double offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Point(start.X + i * offset, start.Y - ((i + 1) % 2 == 0 ? offset : 0.0));
            }
        }

    }
}
