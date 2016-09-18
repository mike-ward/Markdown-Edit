using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace MarkdownEdit.SpellCheck
{
    public class SpellCheckBackgroundRenderer : IBackgroundRenderer
    {
        public SpellCheckBackgroundRenderer() { ErrorSegments = new TextSegmentCollection<TextSegment>(); }

        public TextSegmentCollection<TextSegment> ErrorSegments { get; }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            const int timeout = 100;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var pen = new Pen(new SolidColorBrush(Colors.Red), 0.6);
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