using DevExpress.XtraRichEdit.Fields;
using ISoft.RichReports.Editor.Internal.Fields;
using ISoft.Utils.Helpers;
using System.Collections.Generic;
using System.Reflection;

namespace ISoft.RichReports.Editor.Internal
{
    public static class RichEditInitializer
    {
        private static volatile bool _initialized;
        private static readonly object _lock = new();

        public static bool Initialized => _initialized;

        public static void TryInitialize()
        {
            if (!DesignModeHelper.IsInDesignMode() && !Initialized)
            {
                Initialize();
            }
        }

        public static void Initialize()
        {
            if (_initialized) return;

            lock (_lock)
            {
                if (_initialized) return;

                var fieldTypes = GetFieldTypes();
                if (fieldTypes is null) return;

                Register(fieldTypes, MergefieldField.FieldType, RichEditMergeField.Create);
                Register(fieldTypes, DateField.FieldType, RichEditDateField.Create);
                Register(fieldTypes, TimeField.FieldType, RichEditTimeField.Create);
                Register(fieldTypes, PrintDateField.FieldType, RichEditPrintDateField.Create);
                Register(fieldTypes, IncludepictureField.FieldType, RichEditIncludePictureField.Create);
                Register(fieldTypes, PageField.FieldType, RichEditPageField.Create);
                Register(fieldTypes, NumPagesField.FieldType, RichEditNumPagesField.Create);
                Register(fieldTypes, DocumentPropertyField.FieldType, RichEditDocumentPropertyField.Create);
                Register(fieldTypes, DocVariableField.FieldType, RichEditDocVariableField.Create);

                _initialized = true;
            }
        }

        private static Dictionary<string, CreateFieldDelegate> GetFieldTypes() =>
            typeof(FieldCalculatorService)
                .GetField("fieldTypes", BindingFlags.NonPublic | BindingFlags.Static)
                ?.GetValue(null) as Dictionary<string, CreateFieldDelegate>;

        private static void Register(
            Dictionary<string, CreateFieldDelegate> dic,
            string fieldType,
            CreateFieldDelegate creator) => dic[fieldType] = creator;
    }
}
