using System;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichReports.Editor.Fields.Formatting
{
    public enum RichFieldGeneralFormatType
    {
        Upper,
        Lower,
        FirstCap,
        Caps
    }

    public interface IRichFieldGeneralFormatProvider : IRichFieldFormatProvider<RichFieldGeneralFormatType>
    {

    }

    public class RichFieldGeneralFormatProvider : RichFieldFormatProviderBase<RichFieldGeneralFormatType>, IRichFieldGeneralFormatProvider
    {
        public override RichFieldFormatType FormatType => RichFieldFormatType.General;

        public RichFieldGeneralFormatProvider()
        {

        }

        protected override void PopulateFormats()
        {
            // Register general formats
            Register(RichFieldGeneralFormatType.Upper, RichFieldFormat.General("Upper", "Upper", "UPPERCASE"));
            Register(RichFieldGeneralFormatType.Lower, RichFieldFormat.General("Lower", "Lower", "lowercase"));
            Register(RichFieldGeneralFormatType.FirstCap, RichFieldFormat.General("FirstCap", "FirstCap", "First Cap"));
            Register(RichFieldGeneralFormatType.Caps, RichFieldFormat.General("Caps", "Caps", "CAPS"));
        }

        protected override void PopulateDefaultFormats()
        {
            // Register default formats for types
            //RegisterDefaultFormat(typeof(string), RichFieldGeneralFormatType.FirstCap);
        }

        protected override List<RichFieldFormat> GetAvailableFormatsInternal(Type dataType)
        {
            var formats = new List<RichFieldFormat>();

            if (dataType == null)
                return formats;

            if (dataType == typeof(string))
                return Formats.Values.ToList();

            return formats;
        }
    }
}
