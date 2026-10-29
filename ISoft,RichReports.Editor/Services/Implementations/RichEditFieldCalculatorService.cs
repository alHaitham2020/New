using DevExpress.XtraRichEdit.Fields;
using ISoft.RichReports.Editor.Internal;
using ISoft.RichReports.Editor.Internal.Fields;

namespace ISoft.RichReports.Editor.Services.Implementation
{
    public class RichEditFieldCalculatorService : FieldCalculatorService
    {
        static RichEditFieldCalculatorService()
        {
            RegisterFieldType(RichEditCustomPropertyField.FieldType, RichEditCustomPropertyField.Create);
            RegisterFieldType(RichExpressionEditorCalculatedField.FieldType, RichExpressionEditorCalculatedField.Create);
        }

        public RichEditFieldCalculatorService()
        {
            RichEditInitializer.TryInitialize();
            //throw new System.InvalidOperationException("fields not Initialized");
        }

        //protected override CalculatedFieldBase CreateInitializedCalculatedField(PieceTable pieceTable, Token firstToken, FieldScanner scanner)
        //{
        //    //CalculatedFieldBase calculatedFieldBase = CreateFieldInternal(firstToken.Value);
        //    //if (calculatedFieldBase == null)
        //    //{
        //    return base.CreateInitializedCalculatedField(pieceTable, firstToken, scanner);
        //    //}

        //    //InstructionCollection switches = ParseInstructions(scanner, calculatedFieldBase);
        //    //calculatedFieldBase.Initialize(pieceTable, switches);
        //    //return calculatedFieldBase;
        //}

        //protected static CalculatedFieldBase CreateFieldCore(string fieldType)
        //{
        //    if (fieldType == MergefieldField.FieldType) return RichMergeField.Create();
        //    else if (fieldType == PageField.FieldType) return RichPageField.Create();
        //    else if (fieldType == NumPagesField.FieldType) return RichNumPagesField.Create();
        //    else if (fieldType == PrintDateField.FieldType) return RichPrintDateField.Create();
        //    else if (fieldType == DocumentPropertyField.FieldType) return RichDocumentPropertyField.Create();
        //    else if (fieldType == DocVariableField.FieldType) return RichDocVariableField.Create();
        //    else if (fieldType == IncludepictureField.FieldType) return RichIncludePictureField.Create();
        //    else if (fieldType == DateField.FieldType) return RichDateField.Create();
        //    else if (fieldType == TimeField.FieldType) return RichTimeField.Create();
        //    else return null;
        //}
    }
}
