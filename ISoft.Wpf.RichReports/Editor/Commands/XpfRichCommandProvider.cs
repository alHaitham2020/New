using DevExpress.Mvvm;
using ISoft.RichReportDesigner.UI.Controls;
using ISoft.Utils.Helpers;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public class XpfRichCommandProvider : ViewModelBase, IDisposable
    {
        #region Fields
        public const int PendingRaiseCommandsCanExecuteChangedMilliseconds = 200;

        private readonly XpfRichEditorControl control;

        private DispatcherTimer pendingTimer;

        private bool shouldUpdateCommandsState;
        #endregion

        #region Commands
        private Lazy<XpfRichEditCategoryUICommand> toolsFieldsCommandGroup;
        private Lazy<XpfRichEditUICommand> removeSelectedField;
        private Lazy<XpfRichEditUICommand> removeSelectedFields;
        private Lazy<XpfRichEditUICommand> changeFieldBarcodeType;
        private Lazy<XpfRichEditUICommand> changeFieldImageWidth;
        private Lazy<XpfRichEditUICommand> changeFieldImageHeight;
        private Lazy<XpfRichEditUICommand> changeFieldDateTimeFormat;
        private Lazy<XpfRichEditUICommand> changeFieldNumericFormat;
        private Lazy<XpfRichEditUICommand> showCalculatedFieldsEditorForm;


        public ICommand ToolsFieldsCommandGroup => toolsFieldsCommandGroup.Value;

        public ICommand RemoveSelectedField => removeSelectedField.Value;

        public ICommand RemoveSelectedFields => removeSelectedFields.Value;

        public ICommand ChangeFieldBarcodeType => changeFieldBarcodeType.Value;

        public ICommand ChangeFieldImageWidth => changeFieldImageWidth.Value;

        public ICommand ChangeFieldImageHeight => changeFieldImageHeight.Value;

        public ICommand ChangeFieldDateTimeFormat => changeFieldDateTimeFormat.Value;

        public ICommand ChangeFieldNumericFormat => changeFieldNumericFormat.Value;

        public ICommand ShowCalculatedFieldsEditorForm => showCalculatedFieldsEditorForm.Value;

        #endregion

        #region Actions
        public XpfRichCommandProvider(XpfRichEditorControl control)
        {
            ArgumentGuardHelper.ArgumentNotNull(control, nameof(control));
            this.control = control;
            InitializeTimer();
            CreateCommands();
            SubscribeControlEvents();
        }

        private void CreateCommands()
        {
            toolsFieldsCommandGroup = new Lazy<XpfRichEditCategoryUICommand>(() => new XpfRichEditCategoryUICommand(control, XpfRichEditCommandId.ToolsFieldsCommandGroup));
            removeSelectedField = new Lazy<XpfRichEditUICommand>(() => new XpfRichEditUICommand(control, XpfRichEditCommandId.RemoveSelectedField));
            removeSelectedFields = new Lazy<XpfRichEditUICommand>(() => new XpfRichEditUICommand(control, XpfRichEditCommandId.RemoveSelectedFields));
            changeFieldBarcodeType = new Lazy<XpfRichEditUICommand>(() => new XpfRichEditUICommand(control, XpfRichEditCommandId.ChangeFieldBarcodeType));
            changeFieldImageWidth = new Lazy<XpfRichEditUICommand>(() => new XpfRichEditUICommand(control, XpfRichEditCommandId.ChangeFieldImageWidth));
            changeFieldImageHeight = new Lazy<XpfRichEditUICommand>(() => new XpfRichEditUICommand(control, XpfRichEditCommandId.ChangeFieldImageHeight));
            changeFieldDateTimeFormat = new Lazy<XpfRichEditUICommand>(() => new XpfRichEditUICommand(control, XpfRichEditCommandId.ChangeFieldDateTimeFormat));
            changeFieldNumericFormat = new Lazy<XpfRichEditUICommand>(() => new XpfRichEditUICommand(control, XpfRichEditCommandId.ChangeFieldNumericFormat));
            showCalculatedFieldsEditorForm = new Lazy<XpfRichEditUICommand>(() => new XpfRichEditUICommand(control, XpfRichEditCommandId.RichShowCalculatedFieldsEditorForm));
        }

        private void RaiseCommandsCanExecuteChanged()
        {
            if (toolsFieldsCommandGroup.IsValueCreated)
            {
                toolsFieldsCommandGroup.Value.RaiseCanExecuteChanged();
            }

            if (changeFieldBarcodeType.IsValueCreated)
            {
                changeFieldBarcodeType.Value.RaiseCanExecuteChanged();
            }

            if (removeSelectedField.IsValueCreated)
            {
                removeSelectedField.Value.RaiseCanExecuteChanged();
            }

            if (removeSelectedFields.IsValueCreated)
            {
                removeSelectedFields.Value.RaiseCanExecuteChanged();
            }

            if (changeFieldImageWidth.IsValueCreated)
            {
                changeFieldImageWidth.Value.RaiseCanExecuteChanged();
            }

            if (changeFieldImageHeight.IsValueCreated)
            {
                changeFieldImageHeight.Value.RaiseCanExecuteChanged();
            }

            if (changeFieldDateTimeFormat.IsValueCreated)
            {
                changeFieldDateTimeFormat.Value.RaiseCanExecuteChanged();
            }

            if (changeFieldNumericFormat.IsValueCreated)
            {
                changeFieldNumericFormat.Value.RaiseCanExecuteChanged();
            }

            if (showCalculatedFieldsEditorForm.IsValueCreated)
            {
                showCalculatedFieldsEditorForm.Value.RaiseCanExecuteChanged();
            }
        }

        private void UpdateRibbonPageCategoriesState()
        {
            if (toolsFieldsCommandGroup.IsValueCreated)
            {
                toolsFieldsCommandGroup.Value.UpdateRibbonPageCategoryState();
            }
        }

        private void InitializeTimer()
        {
            pendingTimer = new DispatcherTimer();
            pendingTimer.Tick += OnPendingTimerTick;
            pendingTimer.Interval = TimeSpan.FromMilliseconds(200.0);
            pendingTimer.Start();
        }

        private void OnPendingTimerTick(object sender, EventArgs e)
        {
            if (shouldUpdateCommandsState)
            {
                RaiseCommandsCanExecuteChanged();
                UpdateRibbonPageCategoriesState();
            }

            shouldUpdateCommandsState = false;
        }

        private void DestroyTimer()
        {
            if (pendingTimer != null)
            {
                pendingTimer.Stop();
                pendingTimer.Tick -= OnPendingTimerTick;
                pendingTimer = null;
            }
        }

        private void SubscribeControlEvents()
        {
            control.UpdateUI += OnUpdateUI;
        }

        private void UnsubscribeControlEvents()
        {
            control.UpdateUI -= OnUpdateUI;
        }

        private void OnUpdateUI(object sender, EventArgs e)
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            shouldUpdateCommandsState = true;
        }

        public void Initialize()
        {
            InitializeTimer();
        }

        public void Invalidate()
        {
            DestroyTimer();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~XpfRichCommandProvider()
        {
            Dispose(disposing: false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                DestroyTimer();
                UnsubscribeControlEvents();
            }
        }
        #endregion
    }
}
