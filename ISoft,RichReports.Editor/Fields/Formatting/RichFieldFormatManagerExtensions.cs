namespace ISoft.RichReports.Editor.Fields.Formatting
{
    public static class RichFieldFormatManagerExtensions
    {
        public static TProvider GetFormatProvider<TProvider>(this IRichFieldFormatManager manager, RichFieldFormatType formatType) 
            where TProvider : class, IRichFieldFormatProvider
        {
            return manager.GetProvider(formatType).With(x => x as TProvider);
        }

        public static RichFieldFormat? GetFormatById<TId>(this IRichFieldFormatManager manager, TId id) where TId : struct
           => manager.GetFormat(id.ToString());

        public static IRichFieldGeneralFormatProvider GetGeneralFormatProvider(this IRichFieldFormatManager manager)
            => GetFormatProvider<IRichFieldGeneralFormatProvider>(manager, RichFieldFormatType.General);

        public static IRichFieldNumericFormatProvider GetNumericFormatProvider(this IRichFieldFormatManager manager) 
            => GetFormatProvider<IRichFieldNumericFormatProvider>(manager, RichFieldFormatType.Numeric);

        public static IRichFieldDateTimeFormatProvider GetDateTimeFormatProvider(this IRichFieldFormatManager manager) 
            => GetFormatProvider<IRichFieldDateTimeFormatProvider>(manager, RichFieldFormatType.DateTime);
    }
}
