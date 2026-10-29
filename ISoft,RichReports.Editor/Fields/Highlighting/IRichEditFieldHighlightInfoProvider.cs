namespace ISoft.RichReports.Editor.Fields.Highlighting
{
    public interface IRichEditFieldHighlightInfoProvider
    {
        RichEditFieldHighlightInfo Find(FixedRange range);

        RichEditBrushBase CreateBrush(System.Drawing.Color color);

        RichEditPenBase CreatePen(System.Drawing.Color color, int thickness);

        DocumentLayoutUnit LayoutUnit { get; }

        void Refresh();
    }
}
