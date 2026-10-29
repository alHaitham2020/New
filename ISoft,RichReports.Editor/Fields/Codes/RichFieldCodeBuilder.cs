using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.RichReports.Editor.Fields.Instructions;
using ISoft.RichReports.Editor.Fields.Options;
using ISoft.RichReports.Editor.Fields.Regions;
using System;

namespace ISoft.RichReports.Editor.Fields.Codes
{
    /// <summary>
    /// Builds and manipulates field codes programmatically
    /// </summary>
    public class RichFieldCodeBuilder : IRichFieldCodeBuilder
    {
        private readonly IRichFieldInstructionsBuilder _instructionsBuilder;

        public RichFieldCodeBuilder(IRichFieldInstructionsBuilder instructionsBuilder)
        {
            _instructionsBuilder = instructionsBuilder ?? throw new ArgumentNullException(nameof(instructionsBuilder));
        }

        #region IRichFieldCodeBuilder Implemenation
        public IRichFieldCode CreateField(RichEditFieldType fieldType)
        {
            return new RichFieldCode(fieldType);
        }

        /// <summary>
        /// Creates a new field code
        /// </summary>
        public IRichFieldCode CreateFieldWithOptions(object options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options is not IRichFieldOptions fieldOptions)
                throw new InvalidOperationException($"field options must be of type {nameof(IRichFieldOptions)}");

            var fieldCode = CreateField(fieldOptions.FieldType);

            _instructionsBuilder.BuildInstructions(fieldCode, options);

            return fieldCode;
        }

        /// <summary>
        /// Creates a new field code
        /// </summary>
        public IRichFieldCode CreateFieldWithOptions(object options, InstructionsBuilderTargets targets)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options is not IRichFieldOptions fieldOptions)
                throw new InvalidOperationException($"field options must be of type {nameof(IRichFieldOptions)}");

            var fieldCode = CreateField(fieldOptions.FieldType);

            _instructionsBuilder.BuildInstructions(fieldCode, options, targets);

            return fieldCode;
        }

        /// <summary>
        /// Creates a new field code
        /// </summary>
        public IRichFieldCode CreateFieldWithCommonOptions(RichEditFieldType fieldType, string firstArguemnt, RichFieldFormat? format = null)
        {
            var options = RichCommonFieldsOptions.Create(fieldType, firstArguemnt, format);
            return CreateFieldWithOptions(options);
        }

        /// <summary>
        /// Creates a new MERGEFIELD code
        /// </summary>
        public IRichFieldCode CreateMergeField(string fieldPath, RichFieldFormat? format = null)
        {
            var options = RichMergeFieldOptions.Create(fieldPath, format);

            return CreateMergeField(options);
        }

        /// <summary>
        /// Creates a new MERGEFIELD code
        /// </summary>
        public IRichFieldCode CreateMergeField(RichMergeFieldOptions options)
        {
            return CreateFieldWithOptions(options);
        }

        public IRichFieldCode CreateDocumentPropertyField(RichDocPropertyFieldOptions options)
        {
            return CreateFieldWithOptions(options);
        }

        public IRichFieldCode CreateCustomPropertyField(RichDocCustomPropertyFieldOptions options)
        {
            return CreateFieldWithOptions(options);
        }

        public IRichFieldCode CreateDocumentVariableField(RichDocVariableFieldOptions options)
        {
            return CreateFieldWithOptions(options);
        }

        public IRichFieldCode CreateDocumentPropertyField(string propertyName, RichFieldFormat? format = null)
        {
            return CreateDocumentPropertyField(RichDocPropertyFieldOptions.Create(propertyName, format));
        }

        public IRichFieldCode CreateDocumentInfoField(string propertyName, RichFieldFormat? format = null)
        {
            return CreateFieldWithOptions(RichDocInfoFieldOptions.Create(propertyName, format));
        }

        /// <summary>
        /// Creates a new MERGEFIELD code
        /// </summary>
        public IRichFieldCode CreateCustomPropertyField(string propertyName, RichFieldFormat? format = null)
        {
            return CreateCustomPropertyField(RichDocCustomPropertyFieldOptions.Create(propertyName, format));
        }

        public IRichFieldCode CreateDocumentVariableField(string variableName, RichFieldFormat? format = null)
        {
            return CreateDocumentVariableField(RichDocVariableFieldOptions.Create(variableName, format));
        }

        /// <summary>
        /// Creates a TableStart field
        /// </summary>
        public IRichFieldCode CreateTableStart(string tableName)
        {
            return CreateMergeField($"{TableRegionsName.TableStart}{tableName}");
        }

        /// <summary>
        /// Creates a TableEnd field
        /// </summary>
        public IRichFieldCode CreateTableEnd(string tableName)
        {
            return CreateMergeField($"{TableRegionsName.TableEnd}{tableName}");
        }

        /// <summary>
        /// Creates an IF field
        /// </summary>
        public IRichFieldCode CreateIfField(string expression1, string cmOperator, string expression2)
        {
            var options = RichIfFieldOptions.Create(expression1, cmOperator, expression2);
            return CreateIfField(options);
        }

        /// <summary>
        /// Creates an IF field
        /// </summary>
        public IRichFieldCode CreateIfField(RichIfFieldOptions options)
        {
            return CreateFieldWithOptions(options);
        }

        public IRichFieldCode CreatePictureField(string uri)
        {
            var options = RichIncludePictureFieldOptions.Create(uri);
            return CreatePictureField(options);
        }

        public IRichFieldCode CreatePictureField(RichIncludePictureFieldOptions options)
        {
            return CreateFieldWithOptions(options);
        }

        /// <summary>
        /// Creates a DATE field
        /// </summary>
        public IRichFieldCode CreateDateField(RichFieldFormat? format = null)
        {
            return CreateDateField(RichDateBasedFieldOptions.Create(RichEditFieldType.DateField, format));
        }

        /// <summary>
        /// Creates a DATE field
        /// </summary>
        public IRichFieldCode CreateDateField(RichDateBasedFieldOptions options)
        {
            return CreateFieldWithOptions(options);
        }

        /// <summary>
        /// Creates a TIME field
        /// </summary>
        public IRichFieldCode CreateTimeField(RichFieldFormat? format = null)
        {
            var options = RichFormatableFieldOptions.Create(RichEditFieldType.TimeField, format);
            return CreateFieldWithOptions(options);
        }

        /// <summary>
        /// Creates a PAGE field
        /// </summary>
        public IRichFieldCode CreatePageField()
        {
            return new RichFieldCode(RichEditFieldType.PageField);
        }

        /// <summary>
        /// Creates a NUMPAGES field
        /// </summary>
        public IRichFieldCode CreateNumPagesField()
        {
            return new RichFieldCode(RichEditFieldType.NumPagesField);
        }

        /// <summary>
        /// Builds field switches by reflecting over the options object.
        /// Processes properties with RichFieldSwitchAttribute or RichFieldFormatSwitchAttribute.
        /// </summary>
        public string GetFieldSwitchesBuildString(object options)
        {
            return _instructionsBuilder.GetFieldSwitchesBuildString(options);
        }

        public string GetFieldArgumentsBuildString(object options)
        {
            return _instructionsBuilder.GetFieldArgumentsBuildString(options);
        }

        #endregion
    }
}
