using System.ComponentModel;

namespace ISoft.Wpf.RichReports.Views
{
    /// <summary>
    /// Interaction logic for RichCalculatedFieldView.xaml
    /// </summary>
    public partial class RichCalculatedFieldForm : ContentDialogViewControl, IUICommandsProvider
    {
        private readonly IRichReportCalculatedFieldModelOwner _owner;
        private readonly RichReportCalculatedFieldModel _model;

        public RichCalculatedFieldForm(
            ExpressionEditorContext context,
            RichReportCalculatedFieldModel model,
            IRichReportCalculatedFieldModelOwner owner)
        {
            // Guard clauses: fail fast on bad dependencies instead of NREs later.
            ArgumentGuardHelper.ArgumentNotNull(context, nameof(context));
            ArgumentGuardHelper.ArgumentNotNull(owner, nameof(owner));

            InitializeComponent();

            _owner = owner;
            _model = model;

            // Context must be assigned before the editor is used.
            expressionEditor.Context = context;

            Initialize();
        }

        /// <summary>
        /// Loads the model values into the UI controls.
        /// Safe to call when <see cref="_model"/> is null (e.g. "new field" scenario).
        /// </summary>
        private void Initialize()
        {
            if (_model == null)
                return;

            txtFieldName.Text = _model.FieldName;
            txtFieldDisplayName.Text = _model.DisplayName;
            combFieldType.EditValue = _model.FieldType;
            expressionEditor.SetCurrentExpressionString(_model.Expression);
        }

        /// <summary>
        /// Pushes the (validated) UI values back into the model.
        /// Must only be called after <see cref="Validate"/> has succeeded and the
        /// expression editor has been committed via <c>expressionEditor.Save()</c>.
        /// </summary>
        private void ApplyChanges()
        {
            if (_model == null)
                return;

            _model.FieldName = txtFieldName.Text.Trim();
            _model.DisplayName = txtFieldDisplayName.Text.Trim();
            _model.FieldType = combFieldType.EditValue.TryConvert(Core.Fields.Core.RichCalculatedFieldType.None);
            _model.Expression = expressionEditor.GetCurrentExpressionString();
            _model.DataMember = expressionEditor.GetExpressionDataMember();
        }

        /// <summary>
        /// Runs all validation rules in order. Stops at the first failure and shows
        /// the corresponding localized error message.
        /// </summary>
        /// <returns><c>true</c> if all validations pass; otherwise <c>false</c>.</returns>
        private bool Validate()
        {
            // --- Required fields ---
            if (string.IsNullOrWhiteSpace(txtFieldName.Text))
                return Fail(RichFieldDisplayErrors.FieldNameRequired);

            if (string.IsNullOrWhiteSpace(txtFieldDisplayName.Text))
                return Fail(RichFieldDisplayErrors.FieldDisplayNameRequired);

            // --- Uniqueness ---
            if (_owner.IsDuplicateFieldName(_model.Id, txtFieldName.Text))
                return Fail(RichFieldDisplayErrors.FieldNameDuplicate);

            if (_owner.IsDuplicateFieldDisplayName(_model.Id, txtFieldDisplayName.Text))
                return Fail(RichFieldDisplayErrors.FieldDisplayNameDuplicate);

            // --- Expression editor ---
            if (!expressionEditor.CanSave())
                return Fail(RichFieldDisplayErrors.ExpressionInvalid);

            // Commit the expression so GetCurrentExpressionString() reflects the
            // user's latest edits.
            expressionEditor.Save();

            if (string.IsNullOrWhiteSpace(expressionEditor.GetCurrentExpressionString()))
                return Fail(RichFieldDisplayErrors.ExpressionRequired);

            // --- Expression must reference columns from a single detail data source ---
            if (expressionEditor.HasDiffrentDataMembers())
                return Fail(RichFieldDisplayErrors.ExpressionDiffrentDataMembers);

            return true;
        }

        /// <summary>
        /// Helper: shows an error notification and returns <c>false</c> to make the
        /// validation methods read as a flat sequence of guard clauses.
        /// </summary>
        private static bool Fail(string message)
        {
            NotificationManagerHelper.ShowError(message);
            return false;
        }

        /// <summary>
        /// Creates the OK command. On execution it validates the form, cancels the
        /// dialog if validation fails, and otherwise commits the changes to the model.
        /// </summary>
        private IDelegateCommand CreateOkCommand()
        {
            return CommandsFactory.CreateCommand(async delegate (CancelEventArgs arg)
            {
                // Validate first. If anything fails, cancel and return without
                // mutating the model.
                if (!Validate())
                {
                    arg.Cancel = true;
                    return;
                }

                // Validation passed — commit the changes.
                ApplyChanges();

                await Task.CompletedTask; // keeps the async signature without warnings
            });
        }

        IEnumerable<UICommand> IUICommandsProvider.GetUICommands()
            => DialogUICommandsFactory.GetOkCancelCommands(CreateOkCommand());
    }
}
