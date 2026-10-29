using System.Drawing;

namespace ISoft.RichReports.Core.Abstractions.Fields
{
    public interface IRichFieldHighlightInfo
    {
        Color BackgroundColor { get; }

        Color FontColor { get; }
    }
}
