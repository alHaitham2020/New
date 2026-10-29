using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using ISoft.Utils.Extentions;

namespace ISoft.RichEdit.Model
{
    public class RichEditDocumentModelFieldParser
    {
        private readonly FieldCalculatorService _calculatorService;

        public RichEditDocumentModelFieldParser() : this(new FieldCalculatorService())
        {

        }

        public RichEditDocumentModelFieldParser(FieldCalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        public T ParseField<T>(Field field) where T : CalculatedFieldBase
        {
            CalculatedFieldBase calculatedFieldBase = _calculatorService.ParseField(field.PieceTable, field);
            return calculatedFieldBase.CastTo<T>();
        }

        public bool TryParseField<T>(Field field,out T calcField) where T : CalculatedFieldBase
        {
            CalculatedFieldBase calculatedFieldBase = _calculatorService.ParseField(field.PieceTable, field);
            return calculatedFieldBase.TryCast(out calcField);
        }
    }
}
