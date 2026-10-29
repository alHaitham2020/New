using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.XtraRichEdit.API.Native;
using ISoft.RichReports.Core.Fields;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ISoft.RichReports.Editor.Highlighting
{
    //public sealed class RichEditFieldHighlightPainter : PagePainter
    //{
    //    #region Fields
    //    private const int PaddingH = 4;
    //    private const int PaddingV = 2;
    //    private const int PenThickness = 2;

    //    private int PaddingHConverted;
    //    private int PaddingVConverted;
    //    private int PenThicknessConverted;
    //    private bool isConvetred;

    //    private readonly IRichEditFieldHighlightInfoProvider _provider;
    //    private readonly RichFieldHighlightMode _mode;

    //    private readonly Dictionary<Color, RichEditBrushBase> _brushCache = new();
    //    private readonly Dictionary<Color, RichEditPenBase> _penCache = new();

    //    private readonly Dictionary<FixedRange, List<PointF>> _fieldTransformedPoints = new();

    //    #endregion

    //    public RichEditFieldHighlightPainter(IRichEditFieldHighlightInfoProvider provider, RichFieldHighlightMode mode)
    //    {
    //        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    //        _mode = mode;
    //    }

    //    #region Ovverieds
    //    public override void DrawPlainTextBox(PlainTextBox plainTextBox)
    //    {
    //        EnsureConverted();

    //        var info = FindHighlightInfo(plainTextBox.Range);
    //        if (info != null)
    //        {
    //            // Get the parent LayoutTextBox for the rotation matrix
    //            LayoutTextBox parentTextBox = plainTextBox.GetParentByType<LayoutTextBox>();

    //            Rectangle bounds = plainTextBox.Bounds;
    //            bounds.Inflate(PaddingHConverted, PaddingVConverted);

    //            // Build the four corners of the inflated bounds
    //            PointF[] corners =
    //            [
    //                new (bounds.Left, bounds.Top),
    //                new (bounds.Right, bounds.Top),
    //                new (bounds.Right, bounds.Bottom),
    //                new (bounds.Left, bounds.Bottom)
    //            ];

    //            // Transform corners if the text box is rotated
    //            if (parentTextBox != null)
    //            {
    //                DXMatrix matrix = parentTextBox.GetRotationMatrix();
    //                matrix.TransformPoints(corners);
    //            }

    //            // Accumulate transformed points for this field
    //            if (!_fieldTransformedPoints.TryGetValue(info.Range, out var pointsList))
    //            {
    //                pointsList = new List<PointF>();
    //                _fieldTransformedPoints[info.Range] = pointsList;
    //            }

    //            pointsList.AddRange(corners);
    //        }

    //        base.DrawPlainTextBox(plainTextBox);
    //    }

    //    public override void DrawPage(LayoutPage page)
    //    {
    //        _fieldTransformedPoints.Clear();
    //        EnsureConverted();
    //        base.DrawPage(page);

    //        foreach (var kvp in _fieldTransformedPoints)
    //        {
    //            var info = FindHighlightInfo(kvp.Key);
    //            if (info == null) continue;

    //            var points = kvp.Value;
    //            if (points.Count == 0) continue;

    //            // Compute the screen-space bounding rectangle from all transformed points
    //            float minX = points.Min(p => p.X);
    //            float minY = points.Min(p => p.Y);
    //            float maxX = points.Max(p => p.X);
    //            float maxY = points.Max(p => p.Y);

    //            Rectangle screenBounds = Rectangle.FromLTRB(
    //                (int)minX, (int)minY, (int)maxX, (int)maxY);

    //            DrawHighlight(info.Color, screenBounds);
    //        }
    //    }
    //    #endregion

    //    #region Actions
    //    private void DrawHighlight(Color color, Rectangle bounds)
    //    {
    //        switch (_mode)
    //        {
    //            case RichFieldHighlightMode.Fill:
    //                Canvas.FillRectangle(GetBrush(color), bounds);
    //                break;

    //            case RichFieldHighlightMode.Outline:
    //                Canvas.DrawRectangle(GetPen(color), bounds);
    //                break;
    //        }
    //    }

    //    private RichEditBrushBase GetBrush(Color color)
    //    {
    //        if (!_brushCache.TryGetValue(color, out var brush))
    //        {
    //            brush = _provider.CreateBrush(color);
    //            _brushCache[color] = brush;
    //        }
    //        return brush;
    //    }

    //    private RichEditPenBase GetPen(Color color)
    //    {
    //        if (!_penCache.TryGetValue(color, out var pen))
    //        {
    //            pen = _provider.CreatePen(color, PenThicknessConverted);
    //            _penCache[color] = pen;
    //        }
    //        return pen;
    //    }

    //    /// <summary>
    //    /// Binary search since _highlightInfos is sorted by Range.Start.
    //    /// </summary>
    //    private RichEditFieldHighlightInfo FindHighlightInfo(FixedRange range)
    //    {
    //        return _provider.Find(range);
    //    }

    //    private void EnsureConverted()
    //    {
    //        if (isConvetred || Canvas == null) return;

    //        PaddingHConverted = Convert(PaddingH);
    //        PaddingVConverted = Convert(PaddingV);
    //        PenThicknessConverted = Convert(PenThickness);
    //    }

    //    private int Convert(int value)
    //    {
    //        return Canvas.ConvertToDrawingLayoutUnits(value, _provider.LayoutUnit);
    //    }

    //    private Point[] PointsFToPoints(PointF[] points)
    //    {
    //        Point[] result = new Point[points.Length];
    //        for (int i = 0; i < points.Length; i++)
    //            result[i] = new Point((int)points[i].X, (int)points[i].Y);
    //        return result;
    //    }
    //    #endregion
    //}

    //public sealed class RichEditFieldHighlightPainter : PagePainter
    //{
    //    #region Constants

    //    private const int PaddingH = 4;
    //    private const int PaddingV = 2;
    //    private const int PenThickness = 2;

    //    #endregion

    //    #region Fields

    //    private readonly IRichEditFieldHighlightInfoProvider _provider;
    //    private readonly RichFieldHighlightMode _mode;

    //    private readonly Dictionary<Color, RichEditBrushBase> _brushes =
    //        new Dictionary<Color, RichEditBrushBase>();

    //    private readonly Dictionary<Color, RichEditPenBase> _pens =
    //        new Dictionary<Color, RichEditPenBase>();

    //    // Key = (field range, region) so that a field split across a header
    //    // and the body produces separate outline rectangles instead of one
    //    // giant rectangle spanning both regions.
    //    private readonly Dictionary<FieldKey, FieldPoints> _fieldPoints =
    //        new Dictionary<FieldKey, FieldPoints>();

    //    // Reused synchronously while processing each text fragment.
    //    private readonly PointF[] _cornerBuffer = new PointF[4];

    //    private int _paddingH;
    //    private int _paddingV;
    //    private int _penThickness;

    //    private bool _isConverted;

    //    /// <summary>
    //    /// Identity of a highlighted field within a specific layout region
    //    /// (header / body / footer / footnote / ...).
    //    /// </summary>

    //    private readonly struct FieldKey : IEquatable<FieldKey>
    //    {
    //        public readonly FixedRange Range;
    //        public readonly object Region;

    //        public FieldKey(FixedRange range, object region)
    //        {
    //            Range = range;
    //            Region = region;
    //        }

    //        public bool Equals(FieldKey other)
    //        {
    //            return Equals(Range, other.Range)
    //                && ReferenceEquals(Region, other.Region);
    //        }

    //        public override bool Equals(object obj)
    //        {
    //            return obj is FieldKey && Equals((FieldKey)obj);
    //        }

    //        public override int GetHashCode()
    //        {
    //            unchecked
    //            {
    //                int rangeHash = Range == null ? 0 : Range.GetHashCode();

    //                int regionHash = Region == null
    //                    ? 0
    //                    : System.Runtime.CompilerServices.RuntimeHelpers
    //                        .GetHashCode(Region);

    //                return (rangeHash * 397) ^ regionHash;
    //            }
    //        }
    //    }


    //    private sealed class FieldPoints
    //    {
    //        public readonly Color Color;

    //        public bool HasPoints { get; private set; }

    //        public float MinX { get; private set; }
    //        public float MinY { get; private set; }
    //        public float MaxX { get; private set; }
    //        public float MaxY { get; private set; }

    //        public FieldPoints(Color color)
    //        {
    //            Color = color;
    //        }

    //        public void Add(PointF point)
    //        {
    //            if (!HasPoints)
    //            {
    //                MinX = MaxX = point.X;
    //                MinY = MaxY = point.Y;
    //                HasPoints = true;
    //                return;
    //            }

    //            if (point.X < MinX) MinX = point.X;
    //            if (point.Y < MinY) MinY = point.Y;
    //            if (point.X > MaxX) MaxX = point.X;
    //            if (point.Y > MaxY) MaxY = point.Y;
    //        }

    //        public Rectangle GetBounds()
    //        {
    //            return Rectangle.FromLTRB(
    //                (int)Math.Floor(MinX),
    //                (int)Math.Floor(MinY),
    //                (int)Math.Ceiling(MaxX),
    //                (int)Math.Ceiling(MaxY));
    //        }
    //    }

    //    #endregion

    //    #region Constructor

    //    public RichEditFieldHighlightPainter(
    //        IRichEditFieldHighlightInfoProvider provider,
    //        RichFieldHighlightMode mode)
    //    {
    //        if (provider == null)
    //            throw new ArgumentNullException(nameof(provider));

    //        if (mode != RichFieldHighlightMode.Fill &&
    //            mode != RichFieldHighlightMode.Outline)
    //        {
    //            throw new ArgumentOutOfRangeException(nameof(mode));
    //        }

    //        _provider = provider;
    //        _mode = mode;
    //    }

    //    #endregion

    //    #region Page Rendering

    //    public override void DrawPage(LayoutPage page)
    //    {
    //        if (_mode == RichFieldHighlightMode.Fill)
    //        {
    //            // Fill backgrounds are rendered before the associated text.
    //            base.DrawPage(page);
    //            return;
    //        }

    //        // Geometry must not leak from a previously rendered page.
    //        _fieldPoints.Clear();

    //        EnsureConverted();

    //        // DrawPlainTextBox / DrawSpaceBox collect outline geometry here.
    //        base.DrawPage(page);

    //        // Render outlines after all page text has been drawn.
    //        DrawCollectedOutlines();
    //    }

    //    #endregion

    //    #region Text Rendering

    //    public override void DrawSpaceBox(PlainTextBox spaceBox)
    //    {
    //        ApplyHighlight(spaceBox);

    //        // Keep the text rendering after background painting.
    //        base.DrawSpaceBox(spaceBox);
    //    }

    //    public override void DrawPlainTextBox(PlainTextBox plainTextBox)
    //    {
    //        ApplyHighlight(plainTextBox);

    //        // Keep the text rendering after background painting.
    //        base.DrawPlainTextBox(plainTextBox);
    //    }

    //    private void ApplyHighlight(PlainTextBox textBox)
    //    {
    //        EnsureConverted();

    //        if (!_isConverted)
    //            return;

    //        RichEditFieldHighlightInfo info = _provider.Find(textBox.Range);

    //        if (info == null)
    //            return;

    //        if (_mode == RichFieldHighlightMode.Fill)
    //        {
    //            DrawFillBackground(textBox.Bounds, info.Color);
    //        }
    //        else
    //        {
    //            CollectPoints(textBox, info);
    //        }
    //    }

    //    private void DrawFillBackground(Rectangle bounds, Color color)
    //    {
    //        bounds.Inflate(_paddingH, _paddingV);

    //        Canvas.FillRectangle(GetBrush(color), bounds);
    //    }

    //    #endregion

    //    #region Outline Geometry

    //    private void CollectPoints(
    //        PlainTextBox textBox,
    //        RichEditFieldHighlightInfo info)
    //    {
    //        RectangleF bounds = textBox.Bounds;
    //        bounds.Inflate(_paddingH, _paddingV);

    //        // Resolve the region this text box belongs to. Two fragments of the
    //        // same field that live in different regions (e.g. header vs. body)
    //        // must end up in different buckets so they render as separate
    //        // rectangles.
    //        object region = ResolveRegion(textBox);

    //        FieldPoints field = GetOrCreateField(
    //            new FieldKey(info.Range, region),
    //            info.Color);

    //        LayoutTextBox parent = textBox.GetParentByType<LayoutTextBox>();

    //        if (parent == null)
    //        {
    //            AddRectanglePoints(field, bounds);
    //            return;
    //        }

    //        PointF[] corners = _cornerBuffer;

    //        corners[0] = new PointF(bounds.Left, bounds.Top);
    //        corners[1] = new PointF(bounds.Right, bounds.Top);
    //        corners[2] = new PointF(bounds.Right, bounds.Bottom);
    //        corners[3] = new PointF(bounds.Left, bounds.Bottom);

    //        parent.GetRotationMatrix().TransformPoints(corners);

    //        // Add the transformed coordinates immediately. The reusable
    //        // buffer is safe because no references to it are retained.
    //        for (int i = 0; i < corners.Length; i++)
    //            field.Add(corners[i]);
    //    }

    //    /// <summary>
    //    /// Determines the layout region a text box belongs to. Fragments of a
    //    /// field in different regions are drawn as separate outlines.
    //    /// </summary>
    //    /// <remarks>
    //    /// Adjust the ancestor types checked here to match your layout model.
    //    /// The fallback (last non-null ancestor) keeps behavior sane even if
    //    /// none of the specific region types are found.
    //    /// </remarks>
    //    private static object ResolveRegion(PlainTextBox textBox)
    //    {
    //        // Prefer the most specific region containers first.
    //        object region =
    //            (object)textBox.GetParentByType<LayoutHeader>() ??
    //            (object)textBox.GetParentByType<LayoutFooter>() ??
    //            (object)textBox.GetParentByType<LayoutPageArea>() ??
    //            (object)textBox.GetParentByType<LayoutPage>();

    //        return region;
    //    }

    //    private void AddRectanglePoints(FieldPoints field, RectangleF bounds)
    //    {
    //        field.Add(new PointF(bounds.Left, bounds.Top));
    //        field.Add(new PointF(bounds.Right, bounds.Top));
    //        field.Add(new PointF(bounds.Right, bounds.Bottom));
    //        field.Add(new PointF(bounds.Left, bounds.Bottom));
    //    }

    //    private FieldPoints GetOrCreateField(FieldKey key, Color color)
    //    {
    //        FieldPoints field;

    //        if (_fieldPoints.TryGetValue(key, out field))
    //            return field;

    //        field = new FieldPoints(color);
    //        _fieldPoints.Add(key, field);

    //        return field;
    //    }

    //    private void DrawCollectedOutlines()
    //    {
    //        foreach (KeyValuePair<FieldKey, FieldPoints> pair in _fieldPoints)
    //        {
    //            FieldPoints field = pair.Value;

    //            if (!field.HasPoints)
    //                continue;

    //            Rectangle bounds = field.GetBounds();

    //            if (bounds.Width <= 0 || bounds.Height <= 0)
    //                continue;

    //            Canvas.DrawRectangle(GetPen(field.Color), bounds);
    //        }
    //    }

    //    #endregion

    //    #region Brush and Pen Caching

    //    private RichEditBrushBase GetBrush(Color color)
    //    {
    //        RichEditBrushBase brush;

    //        if (_brushes.TryGetValue(color, out brush))
    //            return brush;

    //        brush = _provider.CreateBrush(color);
    //        _brushes.Add(color, brush);

    //        return brush;
    //    }

    //    private RichEditPenBase GetPen(Color color)
    //    {
    //        RichEditPenBase pen;

    //        if (_pens.TryGetValue(color, out pen))
    //            return pen;

    //        pen = _provider.CreatePen(color, _penThickness);
    //        _pens.Add(color, pen);

    //        return pen;
    //    }

    //    #endregion

    //    #region Unit Conversion

    //    private void EnsureConverted()
    //    {
    //        if (_isConverted || Canvas == null)
    //            return;

    //        _paddingH = Canvas.ConvertToDrawingLayoutUnits(
    //            PaddingH,
    //            _provider.LayoutUnit);

    //        _paddingV = Canvas.ConvertToDrawingLayoutUnits(
    //            PaddingV,
    //            _provider.LayoutUnit);

    //        _penThickness = Canvas.ConvertToDrawingLayoutUnits(
    //            PenThickness,
    //            _provider.LayoutUnit);

    //        _isConverted = true;
    //    }

    //    #endregion
    //}

    public sealed class RichEditFieldHighlightPainter : PagePainter
    {
        #region Constants

        private const int PaddingH = 4;
        private const int PaddingV = 2;
        private const int PenThickness = 2;

        #endregion

        #region Fields

        private readonly IRichEditFieldHighlightInfoProvider _provider;
        private readonly RichFieldHighlightMode _mode;

        private readonly Dictionary<Color, RichEditBrushBase> _brushes =
            new Dictionary<Color, RichEditBrushBase>();

        private readonly Dictionary<Color, RichEditPenBase> _pens =
            new Dictionary<Color, RichEditPenBase>();

        // Only aggregate bounds are required for outline rendering.
        //private readonly Dictionary<FixedRange, FieldPoints> _fieldPoints =
        //    new Dictionary<FixedRange, FieldPoints>();

        private readonly Dictionary<GeometryKey, FieldPoints> _fieldPoints = new Dictionary<GeometryKey, FieldPoints>();

        // Reused synchronously while processing each text fragment.
        private readonly PointF[] _cornerBuffer = new PointF[4];

        private int _paddingH;
        private int _paddingV;
        private int _penThickness;

        private bool _isConverted;


        private struct GeometryKey : IEquatable<GeometryKey>
        {
            private readonly FixedRange _range;
            private readonly object _container;

            public GeometryKey(FixedRange range, object container)
            {
                _range = range;
                _container = container;
            }

            public bool Equals(GeometryKey other)
            {
                return _range.Equals(other._range) &&
                       ReferenceEquals(_container, other._container);
            }

            public override bool Equals(object obj)
            {
                return obj is GeometryKey &&
                       Equals((GeometryKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int rangeHash = _range.GetHashCode();
                    int containerHash = _container == null
                        ? 0
                        : RuntimeHelpers.GetHashCode(_container);

                    return (rangeHash * 397) ^ containerHash;
                }
            }
        }


        private sealed class FieldPoints
        {
            public readonly Color Color;

            public bool HasPoints { get; private set; }

            public float MinX { get; private set; }
            public float MinY { get; private set; }
            public float MaxX { get; private set; }
            public float MaxY { get; private set; }

            public FieldPoints(Color color)
            {
                Color = color;
            }

            public void Add(PointF point)
            {
                if (!HasPoints)
                {
                    MinX = MaxX = point.X;
                    MinY = MaxY = point.Y;
                    HasPoints = true;
                    return;
                }

                if (point.X < MinX) MinX = point.X;
                if (point.Y < MinY) MinY = point.Y;
                if (point.X > MaxX) MaxX = point.X;
                if (point.Y > MaxY) MaxY = point.Y;
            }

            public Rectangle GetBounds()
            {
                return Rectangle.FromLTRB(
                    (int)Math.Floor(MinX),
                    (int)Math.Floor(MinY),
                    (int)Math.Ceiling(MaxX),
                    (int)Math.Ceiling(MaxY));
            }
        }

        #endregion

        #region Constructor

        public RichEditFieldHighlightPainter(
            IRichEditFieldHighlightInfoProvider provider,
            RichFieldHighlightMode mode)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (mode != RichFieldHighlightMode.Fill &&
                mode != RichFieldHighlightMode.Outline)
            {
                throw new ArgumentOutOfRangeException(nameof(mode));
            }

            _provider = provider;
            _mode = mode;
        }

        #endregion

        #region Page Rendering

        public override void DrawPage(LayoutPage page)
        {
            if (_mode == RichFieldHighlightMode.Fill)
            {
                // Fill backgrounds are rendered before the associated text.
                base.DrawPage(page);
                return;
            }

            // Geometry must not leak from a previously rendered page.
            _fieldPoints.Clear();

            EnsureConverted();

            // DrawPlainTextBox and DrawSpaceBox collect outline geometry here.
            base.DrawPage(page);

            // Render outlines after all page text has been drawn.
            DrawCollectedOutlines();
        }

        #endregion

        #region Text Rendering

        public override void DrawSpaceBox(PlainTextBox spaceBox)
        {
            ApplyHighlight(spaceBox);

            // Keep the text rendering after background painting.
            base.DrawSpaceBox(spaceBox);
        }

        public override void DrawPlainTextBox(PlainTextBox plainTextBox)
        {
            ApplyHighlight(plainTextBox);

            // Keep the text rendering after background painting.
            base.DrawPlainTextBox(plainTextBox);
        }

        private void ApplyHighlight(PlainTextBox textBox)
        {
            EnsureConverted();

            if (!_isConverted)
                return;

            RichEditFieldHighlightInfo info = _provider.Find(textBox.Range);

            if (info == null)
                return;

            if (_mode == RichFieldHighlightMode.Fill)
            {
                DrawFillBackground(textBox.Bounds, info.Color);
            }
            else
            {
                CollectPoints(textBox, info);
            }
        }

        private void DrawFillBackground(Rectangle bounds, Color color)
        {
            bounds.Inflate(_paddingH, _paddingV);

            Canvas.FillRectangle(GetBrush(color), bounds);
        }

        #endregion

        #region Outline Geometry
        /// <summary>
        /// Determines the layout region a text box belongs to. Fragments of a
        /// field in different regions are drawn as separate outlines.
        /// </summary>
        /// <remarks>
        /// Adjust the ancestor types checked here to match your layout model.
        /// The fallback (last non-null ancestor) keeps behavior sane even if
        /// none of the specific region types are found.
        /// </remarks>
        private static object ResolveRegion(PlainTextBox textBox)
        {
            // Prefer the most specific region containers first.
            object region =
                (object)textBox.GetParentByType<LayoutHeader>() ??
                (object)textBox.GetParentByType<LayoutFooter>() ??
                (object)textBox.GetParentByType<LayoutPageArea>() ??
                (object)textBox.GetParentByType<LayoutPage>();

            return region;
        }


        private void CollectPoints(
            PlainTextBox textBox,
            RichEditFieldHighlightInfo info)
        {
            RectangleF bounds = textBox.Bounds;
            bounds.Inflate(_paddingH, _paddingV);

            GeometryKey key = new GeometryKey(info.Range, ResolveRegion(textBox));

            FieldPoints field;

            if (!_fieldPoints.TryGetValue(key, out field))
            {
                field = new FieldPoints(info.Color);
                _fieldPoints.Add(key, field);
            }

            LayoutTextBox parent =
                textBox.GetParentByType<LayoutTextBox>();

            if (parent == null)
            {
                AddRectanglePoints(field, bounds);
                return;
            }

            PointF[] corners = _cornerBuffer;

            corners[0] = new PointF(bounds.Left, bounds.Top);
            corners[1] = new PointF(bounds.Right, bounds.Top);
            corners[2] = new PointF(bounds.Right, bounds.Bottom);
            corners[3] = new PointF(bounds.Left, bounds.Bottom);

            parent.GetRotationMatrix().TransformPoints(corners);

            for (int i = 0; i < corners.Length; i++)
                field.Add(corners[i]);
        }


        //private void CollectPoints(
        //    PlainTextBox textBox,
        //    RichEditFieldHighlightInfo info)
        //{
        //    RectangleF bounds = textBox.Bounds;
        //    bounds.Inflate(_paddingH, _paddingV);

        //    FieldPoints field = GetOrCreateField(
        //        info.Range,
        //        info.Color);

        //    LayoutTextBox parent = textBox.GetParentByType<LayoutTextBox>();

        //    if (parent == null)
        //    {
        //        AddRectanglePoints(field, bounds);
        //        return;
        //    }

        //    PointF[] corners = _cornerBuffer;

        //    corners[0] = new PointF(bounds.Left, bounds.Top);
        //    corners[1] = new PointF(bounds.Right, bounds.Top);
        //    corners[2] = new PointF(bounds.Right, bounds.Bottom);
        //    corners[3] = new PointF(bounds.Left, bounds.Bottom);

        //    parent.GetRotationMatrix().TransformPoints(corners);

        //    Add the transformed coordinates immediately.The reusable
        //     buffer is safe because no references to it are retained.
        //    for (int i = 0; i < corners.Length; i++)
        //        field.Add(corners[i]);
        //}

        private void AddRectanglePoints(
            FieldPoints field,
            RectangleF bounds)
        {
            field.Add(new PointF(bounds.Left, bounds.Top));
            field.Add(new PointF(bounds.Right, bounds.Top));
            field.Add(new PointF(bounds.Right, bounds.Bottom));
            field.Add(new PointF(bounds.Left, bounds.Bottom));
        }

        //private FieldPoints GetOrCreateField(
        //    FixedRange range,
        //    Color color)
        //{
        //    FieldPoints field;

        //    if (_fieldPoints.TryGetValue(range, out field))
        //        return field;

        //    field = new FieldPoints(color);
        //    _fieldPoints.Add(range, field);

        //    return field;
        //}

        private void DrawCollectedOutlines()
        {
            foreach (KeyValuePair<GeometryKey, FieldPoints> pair
                in _fieldPoints)
            {
                FieldPoints field = pair.Value;

                if (!field.HasPoints)
                    continue;

                Rectangle bounds = field.GetBounds();

                if (bounds.Width <= 0 || bounds.Height <= 0)
                    continue;

                Canvas.DrawRectangle(GetPen(field.Color), bounds);
            }
        }

        //private void DrawCollectedOutlines()
        //{
        //    foreach (KeyValuePair<FixedRange, FieldPoints> pair in _fieldPoints)
        //    {
        //        FieldPoints field = pair.Value;

        //        if (!field.HasPoints)
        //            continue;

        //        Rectangle bounds = field.GetBounds();

        //        if (bounds.Width <= 0 || bounds.Height <= 0)
        //            continue;

        //        Canvas.DrawRectangle(
        //            GetPen(field.Color),
        //            bounds);
        //    }
        //}

        #endregion

        #region Brush and Pen Caching

        private RichEditBrushBase GetBrush(Color color)
        {
            RichEditBrushBase brush;

            if (_brushes.TryGetValue(color, out brush))
                return brush;

            brush = _provider.CreateBrush(color);
            _brushes.Add(color, brush);

            return brush;
        }

        private RichEditPenBase GetPen(Color color)
        {
            RichEditPenBase pen;

            if (_pens.TryGetValue(color, out pen))
                return pen;

            pen = _provider.CreatePen(color, _penThickness);
            _pens.Add(color, pen);

            return pen;
        }

        #endregion

        #region Unit Conversion

        private void EnsureConverted()
        {
            if (_isConverted || Canvas == null)
                return;

            _paddingH = Canvas.ConvertToDrawingLayoutUnits(
                PaddingH,
                _provider.LayoutUnit);

            _paddingV = Canvas.ConvertToDrawingLayoutUnits(
                PaddingV,
                _provider.LayoutUnit);

            _penThickness = Canvas.ConvertToDrawingLayoutUnits(
                PenThickness,
                _provider.LayoutUnit);

            _isConverted = true;
        }

        #endregion
    }

}
