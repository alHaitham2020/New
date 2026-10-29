//using DevExpress.Drawing;
//using DevExpress.Xpf.RichEdit;
//using DevExpress.XtraRichEdit.API.Layout;
//using DevExpress.XtraRichEdit.API.Native;
//using ISoft.RichReportDesigner.Core.Fields.Core;
//using ISoft.RichReportDesigner.Core.Fields.Display;
//using ISoft.RichReportDesigner.Core.RichEdit;
//using ISoft.RichReportDesigner.Core.RichEdit.Services;
//using ISoft.RichReportDesigner.Core.RichEdit.Services.Implementation;
//using ISoft.RichReportDesigner.UI.Controls;
//using ISoft.Utils.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;

//namespace ISoft.RichReportDesigner.UI.RichEdit.Helpers
//{
//    public sealed class RichHighlightInfo
//    {
//        public RichHighlightInfo(FixedRange range, IRichDisplayField field)
//        {
//            Range = range;
//            Field = field;
//        }

//        public FixedRange Range { get; }

//        public IRichDisplayField Field { get; }
//    }

//    public sealed class RichFieldsHighlightPainter : PagePainter
//    {
//        private const int HorizontalPadding = 3;
//        private const int VerticalPadding = 0;   // don't bleed vertically
//        private const int GradientSteps = 16;

//        private readonly IRichReportDesignerControl _editControl;
//        private readonly Dictionary<Color, RichEditBrush> _brushCache = new();

//        // Sorted by start position → enables binary search
//        private RichHighlightInfo[] _highlightInfos = Array.Empty<RichHighlightInfo>();

//        public RichFieldsHighlightPainter(IRichReportDesignerControl editControl)
//        {
//            _editControl = editControl ?? throw new ArgumentNullException(nameof(editControl));
//            Refresh();
//        }

//        /// <summary>
//        /// Rebuilds the highlight list. Call this when the document's fields change.
//        /// </summary>
//        public void Refresh()
//        {
//            var service = new RichFieldManagerService();
//            if (service == null)
//            {
//                _highlightInfos = Array.Empty<RichHighlightInfo>();
//                return;
//            }

//            var results = service.FindAllDocumentFields(_editControl.Editor.Document);

//            _highlightInfos = results
//                .Select(re =>
//                {
//                    var field = _editControl.DisplayFields.FindFieldByKey(re.FieldCode.FieldKey);
//                    return field == null
//                        ? null
//                        : new RichHighlightInfo(re.Field.Range.ToFixedRange(), field);
//                })
//                .Where(info => info != null)
//                .OrderBy(info => info.Range.Start)
//                .ToArray();
//        }

//        //public override void DrawPlainTextBox(PlainTextBox plainTextBox)
//        //{
//        //    HighlightElement(plainTextBox);
//        //    base.DrawPlainTextBox(plainTextBox);
//        //}

//        //public override void DrawPageNumberBox(PlainTextBox pageNumberBox)
//        //{
//        //    HighlightElement(pageNumberBox);
//        //    base.DrawPageNumberBox(pageNumberBox);
//        //}

//        //public override void DrawSpaceBox(PlainTextBox spaceBox)
//        //{
//        //    HighlightElement(spaceBox);
//        //    base.DrawSpaceBox(spaceBox);
//        //}

//        private void HighlightElement(PlainTextBox plainTextBox)
//        {
//            var info = FindHighlight(plainTextBox.Range);
//            if (info != null)
//            {
//                Rectangle bounds = plainTextBox.Bounds;

//                // Pad only horizontally + slight vertical, avoids overlapping adjacent lines
//                bounds.Inflate(10, 10);

//                var brush = GetBrush(info.Field.BackgroundColor);
//                Canvas.FillRectangle(brush, bounds);
//            }
//        }

//        //public override void DrawFieldHighlightAreaBox(FieldHighlightAreaBox fieldHighlightAreaBox)
//        //{
//        //    try
//        //    {
//        //        var info = FindHighlight(fieldHighlightAreaBox.Range);
//        //        if (info != null)
//        //        {
//        //            Rectangle bounds = fieldHighlightAreaBox.Bounds;
//        //            bounds.Inflate(50, 50);

//        //            RichEditBrush brush = GetBrush(info.Field.BackgroundColor);
//        //            Canvas.FillRectangle(brush, bounds);
//        //        }
//        //    }
//        //    catch
//        //    {

//        //    }

//        //    //base.DrawFieldHighlightAreaBox(fieldHighlightAreaBox);
//        //}


//        //public override void DrawPlainTextBox(PlainTextBox plainTextBox)
//        //{
//        //    HighlightElement(plainTextBox);
//        //    base.DrawPlainTextBox(plainTextBox);
//        //}

//        private void HighlightElement2(PlainTextBox plainTextBox)
//        {
//            var info = FindHighlight(plainTextBox.Range);
//            if (info != null)
//            {
//                var currentHighLightPen = new RichEditPen(info.Field.BackgroundColor.ToMediaColor(), 1);

//                //LayoutTextBox parentTextBox = plainTextBox.GetParentByType<LayoutTextBox>();
//                //if (parentTextBox != null)
//                //{
//                //    DXMatrix matrix = parentTextBox.GetRotationMatrix();
//                //    Rectangle bounds = plainTextBox.Bounds;
//                //    PointF[] points = new PointF[] {
//                //                new Point(bounds.X, bounds.Y),
//                //                new Point(bounds.X + bounds.Width, bounds.Y),
//                //                new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height),
//                //                new Point(bounds.X, bounds.Y+bounds.Height),
//                //                new Point(bounds.X, bounds.Y)
//                //            };
//                //    matrix.TransformPoints(points);
//                //    Canvas.DrawLines(currentHighLightPen, PointsFToPoints(points));
//                //}
//                //else
//                //{
//                //    Canvas.DrawRectangle(currentHighLightPen, plainTextBox.Bounds);
//                //}
//                CharacterBoxCollection characterBoxes = _editControl.Editor.DocumentLayout.Split(plainTextBox);
//                CharacterBox firstBox = characterBoxes[0];
//                CharacterBox lastBox = characterBoxes[characterBoxes.Count - 1];
//                foreach (CharacterBox box in characterBoxes)
//                {
//                    if (box.Range.Start == plainTextBox.Range.Start)
//                        firstBox = box;
//                    if (box.Range.Start + box.Range.Length == plainTextBox.Range.Start + plainTextBox.Range.Length)
//                        lastBox = box;
//                }
//                var charactersBoundingBox = Rectangle.FromLTRB(firstBox.Bounds.X, firstBox.Bounds.Y, lastBox.Bounds.Right, lastBox.Bounds.Bottom);
//                Canvas.DrawRectangle(currentHighLightPen, charactersBoundingBox);
//                //DrawGradientBackground(plainTextBox, info.Field.BackgroundColor);
//            }
//        }

//        Point[] PointsFToPoints(PointF[] points)
//        {
//            Point[] result = new Point[points.Length];
//            for (int i = 0; i < points.Length; i++)
//                result[i] = new Point((int)points[i].X, (int)points[i].Y);
//            return result;
//        }

//        private void DrawGradientBackground(PlainTextBox box, Color baseColor)
//        {
//            Rectangle bounds = box.Bounds;
//            bounds.Inflate(HorizontalPadding, VerticalPadding);

//            // 3-stop gradient: edge color → white → edge color
//            Color edge = baseColor;
//            Color center = BlendWith(baseColor, Color.White, 0.85f); // mostly white

//            int totalHeight = bounds.Height;
//            int stepHeight = Math.Max(1, totalHeight / GradientSteps); // avoid 0
//            int lastY = bounds.Y;

//            for (int i = 0; i < GradientSteps; i++)
//            {
//                float t = GradientSteps == 1 ? 0f : (float)i / (GradientSteps - 1);
//                Color stripColor = t < 0.5f
//                    ? BlendWith(edge, center, t * 2f)
//                    : BlendWith(center, edge, (t - 0.5f) * 2f);

//                int y = bounds.Y + (i * stepHeight);
//                int h = (i == GradientSteps - 1) ? (bounds.Bottom - y) : stepHeight; // absorb remainder

//                var stripRect = new Rectangle(bounds.X, y, bounds.Width, h);
//                Canvas.FillRectangle(GetBrush(stripColor), stripRect);
//            }
//        }

//        private static Color BlendWith(Color a, Color b, float t)
//        {
//            t = t < 0f ? 0f : (t > 1f ? 1f : t);
//            return Color.FromArgb(
//                (int)(a.A + (b.A - a.A) * t),
//                (int)(a.R + (b.R - a.R) * t),
//                (int)(a.G + (b.G - a.G) * t),
//                (int)(a.B + (b.B - a.B) * t));
//        }

//        private RichEditBrush GetBrush(Color color)
//        {
//            if (!_brushCache.TryGetValue(color, out var brush))
//            {
//                brush = new RichEditBrush(color.ToMediaColor());
//                _brushCache[color] = brush;
//            }
//            return brush;
//        }

//        /// <summary>
//        /// Binary search since _highlightInfos is sorted by Range.Start.
//        /// </summary>
//        private RichHighlightInfo FindHighlight(FixedRange range)
//        {
//            if (range == null) return null;

//            return _highlightInfos.FirstOrDefault(x => x.Range.Contains(range));
//        }

//        private const int BracketWidth = 5;      // thickness of the bracket
//        private const int BracketPadding = 10;    // padding between text and bracket
//        private const int PaddingH = 20;
//        private const int PaddingV = 20;
//        private const int PenThickness = 5;

//        //private readonly Dictionary<FixedRange, Rectangle> _fieldBounds = new Dictionary<FixedRange, Rectangle>();

//        private readonly Dictionary<FixedRange, Dictionary<int, Rectangle>> _fieldLineBounds = new();

//        public override void DrawPlainTextBox(PlainTextBox plainTextBox)
//        {
//            var info = FindHighlight(plainTextBox.Range);
//            if (info != null)
//            {
//                Rectangle lineBounds = plainTextBox.Bounds;
//                lineBounds.Inflate(PaddingH, PaddingV);

//                if (!_fieldLineBounds.TryGetValue(info.Range, out var lines))
//                {
//                    lines = new Dictionary<int, Rectangle>();
//                    _fieldLineBounds[info.Range] = lines;
//                }

//                int lineKey = lineBounds.Y; // group by line Y
//                if (lines.TryGetValue(lineKey, out var existing))
//                    lines[lineKey] = Rectangle.Union(existing, lineBounds);
//                else
//                    lines[lineKey] = lineBounds;
//            }

//            base.DrawPlainTextBox(plainTextBox);
//        }

//        public override void DrawPage(LayoutPage page)
//        {
//            _fieldLineBounds.Clear();
//            base.DrawPage(page);

//            foreach (var kvp in _fieldLineBounds)
//            {
//                var info = FindHighlight(kvp.Key);
//                if (info == null) continue;

//                var pen = new RichEditPen(info.Field.BackgroundColor.ToMediaColor(), PenThickness);
//                foreach (var lineRect in kvp.Value.Values)
//                {
//                    Canvas.DrawRectangle(pen, lineRect);
//                }
//            }
//        }


//    }

//    //public sealed class RichFieldHighlightPainter : PagePainter
//    //{
//    //    private const int PaddingH = 20;
//    //    private const int PaddingV = 20;
//    //    private const int PenThickness = 5;

//    //    private readonly Dictionary<FixedRange, Dictionary<int, Rectangle>> _fieldLineBounds = new();

//    //    private readonly IRichReportDesignerControl _editControl;
//    //    private readonly Dictionary<Color, RichEditBrush> _brushCache = new();

//    //    // Sorted by start position → enables binary search
//    //    private RichHighlightInfo[] _highlightInfos = Array.Empty<RichHighlightInfo>();

//    //    private readonly RichFieldHighlightMode _mode;

//    //    public RichFieldHighlightPainter(IRichReportDesignerControl editControl, RichFieldHighlightMode mode)
//    //    {
//    //        _editControl = editControl ?? throw new ArgumentNullException(nameof(editControl));
//    //        Refresh();
//    //        _mode = mode;
//    //    }

//    //    /// <summary>
//    //    /// Rebuilds the highlight list. Call this when the document's fields change.
//    //    /// </summary>
//    //    public void Refresh()
//    //    {
//    //        var service = new RichFieldManagerService();
//    //        if (service == null)
//    //        {
//    //            _highlightInfos = Array.Empty<RichHighlightInfo>();
//    //            return;
//    //        }

//    //        var results = service.FindDocumentFields(_editControl.EditControl.Document);

//    //        _highlightInfos = results
//    //            .Select(re =>
//    //            {
//    //                var field = _editControl.Fields.FindFieldByKey(re.FieldCode.FieldKey);
//    //                return field == null
//    //                    ? null
//    //                    : new RichHighlightInfo(re.Field.Range.ToFixedRange(), field);
//    //            })
//    //            .Where(info => info != null)
//    //            .OrderBy(info => info.Range.Start)
//    //            .ToArray();
//    //    }

//    //    private RichEditBrush GetBrush(Color color)
//    //    {
//    //        if (!_brushCache.TryGetValue(color, out var brush))
//    //        {
//    //            brush = new RichEditBrush(color.ToMediaColor());
//    //            _brushCache[color] = brush;
//    //        }
//    //        return brush;
//    //    }

//    //    /// <summary>
//    //    /// Binary search since _highlightInfos is sorted by Range.Start.
//    //    /// </summary>
//    //    private RichHighlightInfo FindHighlightInfo(FixedRange range)
//    //    {
//    //        if (range == null) return null;

//    //        return _highlightInfos.FirstOrDefault(x => x.Range.Contains(range));
//    //    }

//    //    public override void DrawPlainTextBox(PlainTextBox plainTextBox)
//    //    {
//    //        var info = FindHighlightInfo(plainTextBox.Range);
//    //        if (info != null)
//    //        {
//    //            Rectangle lineBounds = plainTextBox.Bounds;
//    //            lineBounds.Inflate(PaddingH, PaddingV);

//    //            if (!_fieldLineBounds.TryGetValue(info.Range, out var lines))
//    //            {
//    //                lines = new Dictionary<int, Rectangle>();
//    //                _fieldLineBounds[info.Range] = lines;
//    //            }

//    //            int lineKey = lineBounds.Y; // group by line Y
//    //            if (lines.TryGetValue(lineKey, out var existing))
//    //                lines[lineKey] = Rectangle.Union(existing, lineBounds);
//    //            else
//    //                lines[lineKey] = lineBounds;
//    //        }

//    //        base.DrawPlainTextBox(plainTextBox);
//    //    }

//    //    public override void DrawPage(LayoutPage page)
//    //    {
//    //        _fieldLineBounds.Clear();
//    //        base.DrawPage(page);

//    //        foreach (var kvp in _fieldLineBounds)
//    //        {
//    //            var info = FindHighlightInfo(kvp.Key);
//    //            if (info == null) continue;

//    //            foreach (var lineRect in kvp.Value.Values)
//    //            {
//    //                DrawHighlight(info.Field.BackgroundColor, lineRect);
//    //            }
//    //        }
//    //    }

//    //    void DrawHighlight(Color highlightColor, Rectangle bounds)
//    //    {
//    //        switch (_mode)
//    //        {
//    //            case RichFieldHighlightMode.None:
//    //                break;

//    //            case RichFieldHighlightMode.Fill:
//    //                var brush = GetBrush(highlightColor);
//    //                Canvas.FillRectangle(brush, bounds);
//    //                break;
//    //            case RichFieldHighlightMode.Outline:
//    //                var pen = new RichEditPen(highlightColor.ToMediaColor(), PenThickness);
//    //                Canvas.DrawRectangle(pen, bounds);
//    //                break;
//    //            default:
//    //                break;
//    //        }
//    //    }
//    //}

//    public sealed class RichFieldHighlightPainter : PagePainter
//    {
//        #region Fields
//        private const int PaddingH = 8;
//        private const int PaddingV = 2;
//        private const int PenThickness = 2;

//        private readonly IRichReportDesignerControl _editControl;
//        private readonly RichFieldHighlightMode _mode;

//        private readonly Dictionary<Color, RichEditBrush> _brushCache = new();
//        private readonly Dictionary<Color, RichEditPen> _penCache = new();

//        private readonly Dictionary<FixedRange, List<PointF>> _fieldTransformedPoints = new();

//        // Sorted by Range.Start for binary search
//        private RichHighlightInfo[] _highlightInfos = Array.Empty<RichHighlightInfo>();
//        #endregion

//        public RichFieldHighlightPainter(IRichReportDesignerControl editControl, RichFieldHighlightMode mode)
//        {
//            _editControl = editControl ?? throw new ArgumentNullException(nameof(editControl));
//            _mode = mode;
//            Refresh();
//        }

//        /// <summary>Rebuild the highlight list. Call when fields change.</summary>
//        public void Refresh()
//        {
//            var service = new RichFieldManagerService();

//            _highlightInfos = service.FindAllDocumentFields(_editControl.Editor.Document)
//                .Select(re =>
//                {
//                    var field = _editControl.DisplayFields.FindFieldByKey(re.FieldCode.FieldKey);
//                    return field == null
//                        ? null
//                        : new RichHighlightInfo(re.Field.Range.ToFixedRange(), field);
//                })
//                .Where(x => x != null)
//                .OrderBy(x => x.Range.Start)
//                .ToArray();
//        }

//        public override void DrawPlainTextBox(PlainTextBox plainTextBox)
//        {
//            var info = FindHighlightInfo(plainTextBox.Range);
//            if (info != null)
//            {
//                // Get the parent LayoutTextBox for the rotation matrix
//                LayoutTextBox parentTextBox = plainTextBox.GetParentByType<LayoutTextBox>();

//                Rectangle bounds = plainTextBox.Bounds;
//                bounds.Inflate(PaddingH, PaddingV);

//                // Build the four corners of the inflated bounds
//                PointF[] corners =
//                [
//                    new (bounds.Left, bounds.Top),
//                    new (bounds.Right, bounds.Top),
//                    new (bounds.Right, bounds.Bottom),
//                    new (bounds.Left, bounds.Bottom)
//                ];

//                // Transform corners if the text box is rotated
//                if (parentTextBox != null)
//                {
//                    DXMatrix matrix = parentTextBox.GetRotationMatrix();
//                    matrix.TransformPoints(corners);
//                }

//                // Accumulate transformed points for this field
//                if (!_fieldTransformedPoints.TryGetValue(info.Range, out var pointsList))
//                {
//                    pointsList = new List<PointF>();
//                    _fieldTransformedPoints[info.Range] = pointsList;
//                }

//                pointsList.AddRange(corners);
//            }

//            base.DrawPlainTextBox(plainTextBox);
//        }

//        public override void DrawPage(LayoutPage page)
//        {
//            _fieldTransformedPoints.Clear();
//            base.DrawPage(page);

//            foreach (var kvp in _fieldTransformedPoints)
//            {
//                var info = FindHighlightInfo(kvp.Key);
//                if (info == null) continue;

//                var points = kvp.Value;
//                if (points.Count == 0) continue;

//                // Compute the screen-space bounding rectangle from all transformed points
//                float minX = points.Min(p => p.X);
//                float minY = points.Min(p => p.Y);
//                float maxX = points.Max(p => p.X);
//                float maxY = points.Max(p => p.Y);

//                Rectangle screenBounds = Rectangle.FromLTRB(
//                    (int)minX, (int)minY, (int)maxX, (int)maxY);

//                DrawHighlight(info.Field.BackgroundColor, screenBounds);
//            }
//        }

//        private void DrawHighlight(Color color, Rectangle bounds)
//        {
//            switch (_mode)
//            {
//                case RichFieldHighlightMode.Fill:
//                    Canvas.FillRectangle(GetBrush(color), bounds);
//                    break;

//                case RichFieldHighlightMode.Outline:
//                    Canvas.DrawRectangle(GetPen(color), bounds);
//                    break;

               
//            }
//        }

//        private RichEditBrush GetBrush(Color color)
//        {
//            if (!_brushCache.TryGetValue(color, out var brush))
//            {
//                brush = new RichEditBrush(color.ToMediaColor());
//                _brushCache[color] = brush;
//            }
//            return brush;
//        }

//        private RichEditPen GetPen(Color color)
//        {
//            if (!_penCache.TryGetValue(color, out var pen))
//            {
//                pen = new RichEditPen(color.ToMediaColor(), PenThickness);
//                _penCache[color] = pen;
//            }
//            return pen;
//        }

//        /// <summary>
//        /// Binary search since _highlightInfos is sorted by Range.Start.
//        /// </summary>
//        private RichHighlightInfo FindHighlightInfo(FixedRange range)
//        {
//            if (range == null) return null;

//            return _highlightInfos.FirstOrDefault(x => x.Range.Contains(range));
//        }

//        private Point[] PointsFToPoints(PointF[] points)
//        {
//            Point[] result = new Point[points.Length];
//            for (int i = 0; i < points.Length; i++)
//                result[i] = new Point((int)points[i].X, (int)points[i].Y);
//            return result;
//        }
//    }

//    //public sealed class RichFieldHighlightPainter2 : PagePainter
//    //{
//    //    private const int PaddingH = 4;
//    //    private const int PaddingV = 2;
//    //    private const int PenThickness = 2;

//    //    private readonly IRichReportDesignerControl _editControl;
//    //    private readonly RichFieldHighlightMode _mode;

//    //    private readonly Dictionary<Color, RichEditBrush> _brushCache = new();
//    //    private readonly Dictionary<Color, RichEditPen> _penCache = new();

//    //    // Per-page accumulation: field range → list of transformed corner points
//    //    // Each entry is a set of 4 corners (a quadrilateral) for one PlainTextBox run.
//    //    private readonly Dictionary<FixedRange, List<PointF[]>> _fieldQuads = new();

//    //    private RichHighlightInfo[] _highlightInfos = Array.Empty<RichHighlightInfo>();

//    //    public RichFieldHighlightPainter2(IRichReportDesignerControl editControl, RichFieldHighlightMode mode)
//    //    {
//    //        _editControl = editControl ?? throw new ArgumentNullException(nameof(editControl));
//    //        _mode = mode;
//    //        Refresh();
//    //    }

//    //    public void Refresh()
//    //    {
//    //        var service = new RichFieldManagerService();

//    //        _highlightInfos = service.FindDocumentFields(_editControl.EditControl.Document)
//    //            .Select(re =>
//    //            {
//    //                var field = _editControl.Fields.FindFieldByKey(re.FieldCode.FieldKey);
//    //                return field == null
//    //                    ? null
//    //                    : new RichHighlightInfo(re.Field.Range.ToFixedRange(), field);
//    //            })
//    //            .Where(x => x != null)
//    //            .OrderBy(x => x.Range.Start)
//    //            .ToArray();
//    //    }

//    //    public override void DrawPlainTextBox(PlainTextBox plainTextBox)
//    //    {
//    //        var info = FindHighlightInfo(plainTextBox.Range);
//    //        if (info != null)
//    //        {
//    //            Rectangle bounds = plainTextBox.Bounds;
//    //            bounds.Inflate(PaddingH, PaddingV);

//    //            // Build the 4 corners of the inflated bounds
//    //            PointF[] corners =
//    //            {
//    //                new PointF(bounds.Left,  bounds.Top),
//    //                new PointF(bounds.Right, bounds.Top),
//    //                new PointF(bounds.Right, bounds.Bottom),
//    //                new PointF(bounds.Left,  bounds.Bottom)
//    //            };

//    //            // Transform to screen space if the parent LayoutTextBox is rotated
//    //            var parentTextBox = plainTextBox.GetParentByType<LayoutTextBox>();
//    //            if (parentTextBox != null)
//    //            {
//    //                DXMatrix matrix = parentTextBox.GetRotationMatrix();
//    //                matrix.TransformPoints(corners);
//    //            }

//    //            if (!_fieldQuads.TryGetValue(info.Range, out var quads))
//    //            {
//    //                quads = new List<PointF[]>(4);
//    //                _fieldQuads[info.Range] = quads;
//    //            }
//    //            quads.Add(corners);
//    //        }

//    //        base.DrawPlainTextBox(plainTextBox);
//    //    }

//    //    public override void DrawPage(LayoutPage page)
//    //    {
//    //        _fieldQuads.Clear();
//    //        base.DrawPage(page);

//    //        foreach (var kvp in _fieldQuads)
//    //        {
//    //            var info = FindHighlightInfo(kvp.Key);
//    //            if (info == null) continue;

//    //            var color = info.Field.BackgroundColor;

//    //            // 1) Merge all quads for this field into a single convex hull
//    //            var hull = ComputeConvexHull(kvp.Value);
//    //            if (hull.Count < 3) continue;

//    //            // 2) Draw using the appropriate mode
//    //            DrawRotatedHighlight(color, hull);
//    //        }
//    //    }

//    //    // ---------- Convex Hull (Andrew's Monotone Chain) ----------
//    //    private static List<PointF> ComputeConvexHull(List<PointF[]> quads)
//    //    {
//    //        // Flatten all corners into one point list
//    //        var points = new List<PointF>(quads.Count * 4);
//    //        foreach (var quad in quads)
//    //            points.AddRange(quad);

//    //        // Deduplicate roughly identical points (within 0.5 units)
//    //        points = DeduplicatePoints(points, 0.5f);

//    //        if (points.Count <= 2) return points;

//    //        // Sort by X, then Y
//    //        points.Sort((a, b) =>
//    //        {
//    //            int cmp = a.X.CompareTo(b.X);
//    //            return cmp != 0 ? cmp : a.Y.CompareTo(b.Y);
//    //        });

//    //        var hull = new List<PointF>(points.Count * 2);

//    //        // Lower hull
//    //        foreach (var p in points)
//    //        {
//    //            while (hull.Count >= 2 && Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
//    //                hull.RemoveAt(hull.Count - 1);
//    //            hull.Add(p);
//    //        }

//    //        // Upper hull
//    //        int lowerCount = hull.Count + 1;
//    //        for (int i = points.Count - 2; i >= 0; i--)
//    //        {
//    //            var p = points[i];
//    //            while (hull.Count >= lowerCount && Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
//    //                hull.RemoveAt(hull.Count - 1);
//    //            hull.Add(p);
//    //        }

//    //        hull.RemoveAt(hull.Count - 1); // last point == first
//    //        return hull;
//    //    }

//    //    private static float Cross(PointF o, PointF a, PointF b)
//    //    {
//    //        return (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);
//    //    }

//    //    private static List<PointF> DeduplicatePoints(List<PointF> points, float tolerance)
//    //    {
//    //        var result = new List<PointF>(points.Count);
//    //        foreach (var p in points)
//    //        {
//    //            bool duplicate = false;
//    //            for (int i = 0; i < result.Count; i++)
//    //            {
//    //                var q = result[i];
//    //                if (Math.Abs(p.X - q.X) <= tolerance && Math.Abs(p.Y - q.Y) <= tolerance)
//    //                {
//    //                    duplicate = true;
//    //                    break;
//    //                }
//    //            }
//    //            if (!duplicate) result.Add(p);
//    //        }
//    //        return result;
//    //    }

//    //    // ---------- Drawing ----------

//    //    Point[] PointsFToPoints(PointF[] points)
//    //    {
//    //        Point[] result = new Point[points.Length];
//    //        for (int i = 0; i < points.Length; i++)
//    //            result[i] = new Point((int)points[i].X, (int)points[i].Y);
//    //        return result;
//    //    }

//    //    private void DrawRotatedHighlight(Color color, List<PointF> hull)
//    //    {
//    //        switch (_mode)
//    //        {
//    //            case RichFieldHighlightMode.Outline:
//    //                {
//    //                    // Close the polygon by repeating the first point
//    //                    var closed = new PointF[hull.Count + 1];
//    //                    hull.CopyTo(closed, 0);
//    //                    closed[hull.Count] = hull[0];

//    //                    Canvas.DrawLines(GetPen(color), PointsFToPoints(closed));
//    //                    break;
//    //                }

//    //            case RichFieldHighlightMode.Fill:
//    //                {
//    //                    // Fill the polygon using the scan-line technique:
//    //                    // draw horizontal strips between min Y and max Y, clipping to edges.
//    //                    FillPolygon(color, hull);
//    //                    break;
//    //                }

//    //            case RichFieldHighlightMode.None:
//    //            default:
//    //                break;
//    //        }
//    //    }

//    //    /// <summary>
//    //    /// Fills a convex polygon by drawing horizontal strips.
//    //    /// Works because the hull is convex — each scan line crosses exactly 2 edges.
//    //    /// </summary>
//    //    private void FillPolygon(Color color, List<PointF> hull)
//    //    {
//    //        var brush = GetBrush(color);

//    //        float minY = hull.Min(p => p.Y);
//    //        float maxY = hull.Max(p => p.Y);
//    //        int yStart = (int)Math.Floor(minY);
//    //        int yEnd = (int)Math.Ceiling(maxY);

//    //        for (int y = yStart; y < yEnd; y++)
//    //        {
//    //            float yMid = y + 0.5f;
//    //            float? leftX = null;
//    //            float? rightX = null;

//    //            // Find intersections of the horizontal scan line with polygon edges
//    //            for (int i = 0; i < hull.Count; i++)
//    //            {
//    //                var a = hull[i];
//    //                var b = hull[(i + 1) % hull.Count];

//    //                if ((a.Y <= yMid && b.Y > yMid) || (b.Y <= yMid && a.Y > yMid))
//    //                {
//    //                    float t = (yMid - a.Y) / (b.Y - a.Y);
//    //                    float x = a.X + t * (b.X - a.X);

//    //                    if (leftX == null || x < leftX) leftX = x;
//    //                    if (rightX == null || x > rightX) rightX = x;
//    //                }
//    //            }

//    //            if (leftX.HasValue && rightX.HasValue && rightX > leftX)
//    //            {
//    //                var rect = Rectangle.FromLTRB(
//    //                    (int)Math.Floor(leftX.Value),
//    //                    y,
//    //                    (int)Math.Ceiling(rightX.Value),
//    //                    y + 1);

//    //                Canvas.FillRectangle(brush, rect);
//    //            }
//    //        }
//    //    }

//    //    // ---------- Caches ----------

//    //    private RichEditBrush GetBrush(Color color)
//    //    {
//    //        if (!_brushCache.TryGetValue(color, out var brush))
//    //        {
//    //            brush = new RichEditBrush(color.ToMediaColor());
//    //            _brushCache[color] = brush;
//    //        }
//    //        return brush;
//    //    }

//    //    private RichEditPen GetPen(Color color)
//    //    {
//    //        if (!_penCache.TryGetValue(color, out var pen))
//    //        {
//    //            pen = new RichEditPen(color.ToMediaColor(), PenThickness);
//    //            _penCache[color] = pen;
//    //        }
//    //        return pen;
//    //    }

//    //    // ---------- Lookup ----------

//    //    private RichHighlightInfo FindHighlightInfo(FixedRange range)
//    //    {
//    //        if (range == null) return null;

//    //        return _highlightInfos.FirstOrDefault(x => x.Range.Contains(range));
//    //    }

//    //}
//}
