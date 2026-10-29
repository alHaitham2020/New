namespace ISoft.RichReports.Editor.Fields.Highlighting
{
    //public class RichEditFieldHighlightInfoProvider : IRichEditFieldHighlightInfoProvider
    //{
    //    private readonly IRichDocumentEditControl _editControl;

    //    // Sorted by Range.Start → enables binary search.
    //    private RichEditFieldHighlightInfo[] _highlightInfo;
    //    private bool _isDirty = true;

    //    public DocumentLayoutUnit LayoutUnit => _editControl.LayoutUnit;

    //    public RichEditFieldHighlightInfoProvider(IRichDocumentEditControl editControl)
    //    {
    //        _editControl = editControl;
    //    }

    //    /// <summary>
    //    /// Marks the cache stale. The next call to <see cref="Find(FixedRange)"/>
    //    /// will rebuild the highlight list.
    //    /// </summary>
    //    public void Refresh() => _isDirty = true;

    //    public RichEditFieldHighlightInfo Find(FixedRange range)
    //    {
    //        if (range == null) return null;

    //        try
    //        {
    //            EnsureUpToDate();

    //            return _highlightInfo.FirstOrDefault(x => x.Range.Contains(range));
    //        }
    //        catch (Exception ex)
    //        {
    //            RunTimeLogger.CurrentLogger.LogException(ex);
    //            return null;
    //        }
    //    }

    //    public RichEditBrushBase CreateBrush(Color color)
    //    {
    //        return _editControl.CreateBrush(color);
    //    }

    //    public RichEditPenBase CreatePen(Color color, int thickness)
    //    {
    //        return _editControl.CreatePen(color, thickness);
    //    }

    //    private void EnsureUpToDate()
    //    {
    //        if (!_isDirty && _highlightInfo != null) return;

    //        var service = new RichFieldManagerService();

    //        _highlightInfo = service
    //            .FindAllDocumentFields(_editControl.Document)
    //            .Select(re =>
    //            {
    //                var field = _editControl.Owner.Fields.FindFieldByKey(re.FieldCode.FieldKey);
    //                return field == null
    //                    ? null
    //                    : new RichEditFieldHighlightInfo(re.Field.Range.ToFixedRange(), field.BackgroundColor);
    //            })
    //            .Where(x => x != null)
    //            .OrderBy(x => x.Range.Start)
    //            .ToArray();

    //        _isDirty = false;
    //    }
    //}

    //public sealed class RichEditFieldHighlightInfoProvider
    //    : IRichEditFieldHighlightInfoProvider
    //{
    //    private readonly IRichEditorControl _editControl;

    //    private RichEditFieldHighlightInfo[] _highlightInfo =
    //        Array.Empty<RichEditFieldHighlightInfo>();

    //    private bool _isDirty = true;

    //    public DocumentLayoutUnit LayoutUnit => _editControl.LayoutUnit;

    //    public RichEditFieldHighlightInfoProvider(
    //        IRichEditorControl editControl)
    //    {
    //        _editControl = editControl
    //            ?? throw new ArgumentNullException(nameof(editControl));
    //    }

    //    /// <summary>
    //    /// Invalidates the cached field information.
    //    /// Call after document fields or their background colors change.
    //    /// </summary>
    //    public void Refresh()
    //    {
    //        _isDirty = true;
    //    }

    //    public RichEditFieldHighlightInfo Find(FixedRange range)
    //    {
    //        if (range == null)
    //            return null;

    //        try
    //        {
    //            EnsureUpToDate();

    //            RichEditFieldHighlightInfo[] items = _highlightInfo;
    //            int low = 0;
    //            int high = items.Length - 1;
    //            int candidate = -1;

    //            // Find the last field whose start is <= the requested start.
    //            while (low <= high)
    //            {
    //                int mid = low + ((high - low) >> 1);

    //                if (items[mid].Range.Start.CompareTo(range.Start) <= 0)
    //                {
    //                    candidate = mid;
    //                    low = mid + 1;
    //                }
    //                else
    //                {
    //                    high = mid - 1;
    //                }
    //            }

    //            // Check candidates backwards because ranges may overlap.
    //            for (int i = candidate; i >= 0; i--)
    //            {
    //                if (items[i].Range.Contains(range))
    //                    return items[i];
    //            }

    //            return null;
    //        }
    //        catch (Exception ex)
    //        {
    //            RunTimeLogger.CurrentLogger.LogException(ex);
    //            return null;
    //        }
    //    }

    //    public RichEditBrushBase CreateBrush(Color color)
    //    {
    //        return _editControl.CreateBrush(color);
    //    }

    //    public RichEditPenBase CreatePen(Color color, int thickness)
    //    {
    //        return _editControl.CreatePen(color, thickness);
    //    }

    //    private void EnsureUpToDate()
    //    {
    //        if (!_isDirty)
    //            return;

    //        var service = _editControl.Owner.GetService<IRichDocumentFieldManagerService>();
    //        var source = new List<RichEditFieldHighlightInfo>();

    //        foreach (var result in service.FindAllDocumentFields(_editControl.Document))
    //        {
    //            var field = _editControl.Owner.DisplayFields.FindFieldByKey(result.FieldCode.FieldKey);

    //            if (field == null)
    //                continue;

    //            source.Add(new RichEditFieldHighlightInfo(
    //                result.Field.Range.ToFixedRange(),
    //                field.BackgroundColor));
    //        }

    //        source.Sort((x, y) =>
    //            x.Range.Start.CompareTo(y.Range.Start));

    //        // Publish only a fully built cache.
    //        _highlightInfo = source.ToArray();
    //        _isDirty = false;
    //    }
    //}

}
