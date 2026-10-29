using DevExpress.XtraRichEdit.API.Native;

namespace ISoft.RichReports.Editor.Highlighting
{
    public sealed class RichEditFieldHighlightInfo
    {
        public RichEditFieldHighlightInfo(FixedRange range, System.Drawing.Color color)
        {
            Range = range;
            Color = color;
        }

        public FixedRange Range { get; }

        public System.Drawing.Color Color { get; }
    }
}
