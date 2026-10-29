namespace ISoft.Wpf.RichReports.ViewModels
{
    public class RichCalculatedFieldsEditorViewModel : DocumentViewModelBase, IRichReportCalculatedFieldModelOwner
    {
        #region Proprties
        private readonly IRichEditorControl _editControl;

        public virtual RichCalculatedFieldModelCollection FieldsSource { get; protected set; }

        public virtual RichReportCalculatedFieldModel SelectedItem { get; set; }

        private IRichReportCalculatedFieldStorage Storage
          => _editControl.Owner.Report.CalculatedFieldStorage;
        #endregion

        public static RichCalculatedFieldsEditorViewModel Create(IRichEditorControl editControl)
        {
            return ViewModelSource.Create(() => new RichCalculatedFieldsEditorViewModel(editControl));
        }

        public RichCalculatedFieldsEditorViewModel(IRichEditorControl editControl)
        {
            // Fail fast on a null dependency rather than NRE-ing inside Load().
            ArgumentGuardHelper.ArgumentNotNull(editControl, nameof(editControl));

            _editControl = editControl;
            Load();
        }

        #region Commands
        public void AddNew()
        {
            try
            {
                var reportId = _editControl.Owner.Report.ReportInfo.Id;
                var model = FieldsSource.CreateNew(reportId);

                _editControl.ShowCalculatedFieldModelForm(model, this);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public void Edit(RichReportCalculatedFieldModel model)
        {
            if (model == null)
                return;

            try
            {
                // Clone so that cancelling the dialog doesn't mutate the original.
                var clone = model.CreateClone();
                _editControl.ShowCalculatedFieldModelForm(clone, this);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public bool CanEdit(RichReportCalculatedFieldModel model) => model != null;

        public async Task Delete(RichReportCalculatedFieldModel model)
        {
            if (model == null)
                return;

            try
            {
                // Guard: don't allow deletion if the field is referenced elsewhere.
                if (!_editControl.Owner.CanRemoveCalculatedField(model.FieldName))
                {
                    MessageBoxService.ShowErrorMessage("لا يمكنك حذف الحقل لأنه مستخدم ضمن المستند");
                    return;
                }

                if (!MessageBoxService.ShowEntityDeleteConfirmation())
                    return;

                var isDeleted = await Storage.DeleteAsync(model.Id);
                if (!isDeleted)
                    return;

                FieldsSource.Remove(model);
                _editControl.Owner.OnCalculatedDisplayFieldRemoved(model.FieldName);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public bool CanDelete(RichReportCalculatedFieldModel model) => model != null;

        public void Refresh()
        {
            try
            {
                Load();
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }
        #endregion

        #region Actions
        private void Load()
        {
            try
            {
                // Ensure the collection exists before populating it.
                FieldsSource ??= new RichCalculatedFieldModelCollection();

                var source = Storage.GetCalculatedFields();
                if (source == null)
                    return;

                // Reset and reload. Using Clear + AddRange (or LoadFrom, which
                // should internally do the same) is preferred over concatenating,
                // otherwise repeated Refresh() calls would duplicate rows.
                FieldsSource.LoadFrom(source.Select(x => new RichReportCalculatedFieldModel(x)));
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        bool IRichReportCalculatedFieldModelOwner.IsDuplicateFieldName(Guid? id, string name)
            => Storage.IsDuplicate(id, name);

        bool IRichReportCalculatedFieldModelOwner.IsDuplicateFieldDisplayName(Guid? id, string name)
            => Storage.IsDuplicateDisplayName(id, name);

        async Task IRichReportCalculatedFieldModelOwner.SaveAsync(RichReportCalculatedFieldModel model)
        {
            var saved = await Storage.SaveAsync(model);
            if (saved == null)
                return;

            FieldsSource.AddOrUpdate(new RichReportCalculatedFieldModel(saved));
            _editControl.Owner.OnCalculatedDisplayFieldChanged(saved.FieldName);
        }

        #endregion
    }
}
