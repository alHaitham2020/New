using DevExpress.Mvvm.UI.Native;
using ISoft.Mvvm.Helpers;
using ISoft.Mvvm.Native;
using ISoft.RichReportDesigner.Core.Designer;
using ISoft.Wpf.RichReports.Controllers;
using ISoft.Wpf.RichReports.DataModel;
using ISoft.Wpf.RichReports.Internal;
using ISoft.RichReportDesigner.Core.Fields.Display;
using ISoft.RichReportDesigner.Core.Reports;
using ISoft.RichReportDesigner.Core.Reports.DataModel;
using ISoft.RichReportDesigner.Core.RichEdit;
using ISoft.RichReportDesigner.Core.RichEdit.Services;
using ISoft.RichReportDesigner.Core.Templates;
using ISoft.Utils.Extentions;
using ISoft.Utils.Helpers;
using ISoft.Utils.Mvvm;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ISoft.Wpf.RichReports.Controls
{
    [TemplatePart(Name = PART_FieldsPanel, Type = typeof(RichReportDesignerFieldsPanel))]
    [TemplatePart(Name = PART_RichEdit, Type = typeof(XpfRichEditorControl))]
    public class RichReportDesignerControl : Control, IRichReportDesignerControl, IBackstageOwner<RichReportDesignerBackstageTab>
    {
        private const string PART_RichEdit = "PART_RichEdit";
        private const string PART_FieldsPanel = "PART_FieldsPanel";

        #region DependencyProperties
        public static readonly DependencyProperty DesignerProperty;

        public static readonly DependencyProperty DisplayFieldsProperty;
        private static readonly DependencyPropertyKey DisplayFieldsPropertyKey;

        public static readonly DependencyProperty ReportProperty;
        public static readonly DependencyProperty AutoUpdateWindowTitleProperty;
        public static readonly DependencyProperty ShowFieldsPanelProperty;

        public static readonly DependencyProperty DisplayFieldsBuilderProperty;

        public static readonly DependencyProperty IsBackstageOpenedProperty;
        public static readonly DependencyProperty BackstageTabIndexProperty;
        public static readonly DependencyProperty DefaultNewTemplateNameProperty;

        public static readonly DependencyProperty ActiveReportTemplateProperty;
        private static readonly DependencyPropertyKey ActiveReportTemplatePropertyKey;

        public static readonly DependencyProperty CommandsProperty;
        public static readonly DependencyProperty ActualCommandsProperty;
        private static readonly DependencyPropertyKey ActualCommandsPropertyKey;

        #endregion

        static RichReportDesignerControl()
        {
            DependencyPropertyRegistrator<RichReportDesignerControl>
                .New()
                .OverrideDefaultStyleKey()
                .RegisterAttached("GetDesigner", out DesignerProperty, (RichReportDesignerControl)null, (Action<RichReportDesignerControl>)null, (FrameworkPropertyMetadataOptions?)FrameworkPropertyMetadataOptions.Inherits)
                .RegisterReadOnly<RichDisplayFieldGroupCollection>(nameof(DisplayFields), out DisplayFieldsPropertyKey, out DisplayFieldsProperty, defaultValue: null)
                .Register<IRichReportDesignerDisplayFieldsBuilder>(nameof(DisplayFieldsBuilder), out DisplayFieldsBuilderProperty, new RichReportDesignerDisplayFieldsBuilder())
                .Register<IXpfRichReport>(nameof(Report), out ReportProperty, null, delegate (RichReportDesignerControl d)
                {
                    d.OnReportSourceChanged(d.Report);
                })
                .RegisterReadOnly<IRichReportTemplate>(nameof(ActiveReportTemplateProperty), out ActiveReportTemplatePropertyKey, out ActiveReportTemplateProperty, defaultValue: null)
                .Register<RichReportDesignerCommands>(nameof(Commands), out CommandsProperty, null, delegate (RichReportDesignerControl d, DependencyPropertyChangedEventArgs e)
                {
                    d.OnCommandsChanged(e);
                })
                .RegisterReadOnly<RichReportDesignerCommands>(nameof(ActualCommands), out ActualCommandsPropertyKey, out ActualCommandsProperty, null)
                .Register(nameof(AutoUpdateWindowTitle), out AutoUpdateWindowTitleProperty, true)
                .Register(nameof(BackstageTabIndex), out BackstageTabIndexProperty, -1)
                .Register(nameof(IsBackstageOpened), out IsBackstageOpenedProperty, false)
                .Register<string>(nameof(DefaultNewTemplateName), out DefaultNewTemplateNameProperty, null)
                .Register(nameof(ShowFieldsPanel), out ShowFieldsPanelProperty, true);
        }

        public RichReportDesignerControl()
        {
            InitializeDependencyProperties();

            _backstageManager = BackstageManager<RichReportDesignerBackstageTab>.Create(this);
            Controller = new RichDesignerController(this);
            dragDropManager = new RichDesignerDragDropManager(this);
            Commands = new RichReportDesignerCommands(this);/*FormatStringUITypeEditor*/
        }

        #region Dependency Property Accessors

        /// <summary>
        /// Gets the designer associated with a dependency object.
        /// </summary>
        public static RichReportDesignerControl GetDesigner(DependencyObject element) =>
            (RichReportDesignerControl)element.GetValue(DesignerProperty);

        /// <summary>
        /// Sets the designer for a dependency object.
        /// </summary>
        public static void SetDesigner(DependencyObject element, RichReportDesignerControl value) =>
            element.SetValue(DesignerProperty, value);

        #endregion

        #region Events Handelrs Proprties
        /// <summary>
        /// Occurs when a template is saved.
        /// </summary>
        public event EventHandler<RichReportDesignerTemplateSavedEventArgs> TemplateSaved;

        /// <summary>
        /// Occurs when the current template changes.
        /// </summary>
        public event EventHandler<RichReportDesignerTemplateEventArgs> CurrentTemplateChanged;
        #endregion

        #region Proprties Fields
        private readonly RichDesignerController Controller;
        private readonly RichDesignerDragDropManager dragDropManager;

        //private bool isDisposed;
        private bool isDesignerLoaded;
        private readonly IBackstageManager<RichReportDesignerBackstageTab> _backstageManager;
        private RichReportDesignerFieldsPanel fieldsPanel;
        private XpfRichEditorControl editor;

        #endregion

        #region Proprties

        public bool IsLoading { get; private set; }

        public bool IsReady => !IsLoading && Report != null && ActiveReportTemplate != null;

        // used in designer xaml
        public XpfRichEditorControl EditControl => editor;

        //protected internal RichEditSearchPanel RichEditSearchPanel
        //{
        //    get
        //    {
        //        ApplyTemplate();
        //        return GetTemplateChild("SearchPanel") as RichEditSearchPanel;
        //    }
        //}

        public RichDisplayFieldGroupCollection DisplayFields => (RichDisplayFieldGroupCollection)GetValue(DisplayFieldsProperty);

        public IRichReportTemplate ActiveReportTemplate => (IRichReportTemplate)GetValue(ActiveReportTemplateProperty);

        public IXpfRichReport Report
        {
            get => (IXpfRichReport)GetValue(ReportProperty);
            set => SetValue(ReportProperty, value);
        }

        public IRichReportDesignerDisplayFieldsBuilder DisplayFieldsBuilder
        {
            get
            {
                return (IRichReportDesignerDisplayFieldsBuilder)GetValue(DisplayFieldsBuilderProperty);
            }
            set
            {
                SetValue(DisplayFieldsBuilderProperty, value);
            }
        }

        public bool AutoUpdateWindowTitle
        {
            get
            {
                return (bool)GetValue(AutoUpdateWindowTitleProperty);
            }
            set
            {
                SetValue(AutoUpdateWindowTitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the designer commands.
        /// </summary>
        public RichReportDesignerCommands Commands
        {
            get => (RichReportDesignerCommands)GetValue(CommandsProperty);
            set => SetValue(CommandsProperty, value);
        }

        /// <summary>
        /// Gets the actual commands instance.
        /// </summary>
        public RichReportDesignerCommands ActualCommands =>
            (RichReportDesignerCommands)GetValue(ActualCommandsProperty);

        public bool IsBackstageOpened
        {
            get
            {
                return (bool)GetValue(IsBackstageOpenedProperty);
            }
            set
            {
                SetValue(IsBackstageOpenedProperty, value);
            }
        }

        public int BackstageTabIndex
        {
            get
            {
                return (int)GetValue(BackstageTabIndexProperty);
            }
            set
            {
                SetValue(BackstageTabIndexProperty, value);
            }
        }

        public string DefaultNewTemplateName
        {
            get
            {
                return (string)GetValue(DefaultNewTemplateNameProperty);
            }
            set
            {
                SetValue(DefaultNewTemplateNameProperty, value);
            }
        }

        public bool ShowFieldsPanel
        {
            get => (bool)GetValue(ShowFieldsPanelProperty);
            set => SetValue(ShowFieldsPanelProperty, value);
        }

        #endregion

        #region Ovverieds
        public override void OnApplyTemplate()
        {
            isDesignerLoaded = false;
            IsLoading = true;
            base.OnApplyTemplate();
            InitializeVisualComponents();
            isDesignerLoaded = true;
            IsLoading = false;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                dragDropManager.Install(fieldsPanel, editor);
                Controller.Editor.Install(editor);
            }
        }

        #endregion

        #region Actions
        private void InitializeVisualComponents()
        {
            fieldsPanel = GetTemplateChild(PART_FieldsPanel) as RichReportDesignerFieldsPanel;
            editor = GetTemplateChild(PART_RichEdit) as XpfRichEditorControl;
        }

        private void RegisterEditorServices()
        {
            Report?.CopyRequiredServices(editor);
        }

        internal T GetService<T>() where T : class
        {
            return Report?.GetService<T>() ?? default;
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (_editor != null)
        //        {
        //            DettachFromEditor();
        //            _editor.Dispose();
        //        }
        //    }

        //    isDisposed = true;
        //}

        //public void Dispose()
        //{
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}

        //~RichReportDesignerControl()
        //{
        //    Dispose(disposing: false);
        //}

        #endregion

        #region privet Actions
        private void InitializeDependencyProperties()
        {
            SetDesigner(this, this);
            SetValue(DisplayFieldsPropertyKey, new RichDisplayFieldGroupCollection());
            SetValue(ActualCommandsPropertyKey, null);
        }

        private void OnException(Exception ex)
        {
            ex?.ShowExceptionMessage();
        }

        #endregion

        //#region Editor Events Handelrs

        //private void Editor_Loaded(object sender, RoutedEventArgs e)
        //{
        //    SetEditorFocus();
        //}

        //private void Editor_DocumentLoaded(object sender, EventArgs e)
        //{
        //    Controller.Fields.HighlightsFields();
        //}

        //private void Editor_InitializeDocument(object sender, EventArgs e)
        //{
        //    editor.ConfigureDocumentDefaultProperties(FlowDirection.IsRightToLeft());
        //}

        //private void Editor_CalculateDocumentVariable(object sender, CalculateDocumentVariableEventArgs e)
        //{
        //    GetService<IRichDocumentVariableManagerService>().DoIfNotNull(s =>
        //    {
        //        s.Calculate(e);
        //    });
        //}

        //// can called by _editor.Refresh()
        //private void Editor_BeforePagePaint(object sender, BeforePagePaintEventArgs e)
        //{
        //    // Only apply custom drawing to the screen, not to the printer
        //    if (e.CanvasOwnerType != CanvasOwnerType.Control || !IsReady)
        //        return;

        //    // Assign the custom painter
        //    e.Painter = new PagePainter();
        //}

        //#endregion

        #region Static Property Change Handlers

        #endregion

        #region Property Change Handelrs
        private void OnCommandsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is RichReportDesignerCommands newCommands)
            {
                newCommands.Attach(this);
                SetValue(ActualCommandsPropertyKey, newCommands);
            }

            if (e.OldValue is RichReportDesignerCommands oldCommands)
            {
                oldCommands.Detach();
            }
        }

        internal void OnReportSourceChanged(IXpfRichReport report)
        {
            if (report == null)
            {
                DisplayFields.Clear();
                return;
            }

            IsLoading = true;
            try
            {
                RegisterEditorServices();

                DisplayFields.LoadFromSource(DisplayFieldsBuilder.Build(report));

                report.DefaultTemplatesSource.LoadIfNotLoaded();
                report.StoreTemplatesSource.LoadIfNotLoaded();

                LoadTemplateSource(report.DesignerTemplate);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void LoadTemplateSource(IRichReportTemplate template)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            ExecuteWithDispatcherAsync(delegate
            {
                GetService<IRichWaitIndicationService>()
                         ?.ShowWaitIndicatorAsync(async () =>
                         {
                             IsLoading = true;
                             await Task.Delay(500);
                             Controller.Template.LoadTemplateSource(template);
                             IsLoading = false;

                         }, "جاري تحميل بيانات التصميم ...");

            }, DispatcherPriority.Loaded);
        }

        #endregion

        #region Display Fields

        internal void HighlightsFields()
        {
            try
            {
                Controller.Fields.HighlightsFields();
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal void ClearFieldsHighlights()
        {
            try
            {
                Controller.Fields.ClearHighlightsFields();
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal void RefreshFieldsHighlight()
        {
            try
            {
                Controller.Fields.RefreshHighlightsFields();
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal void InsertField(IRichDisplayField field)
        {
            try
            {
                Controller.Fields.InsertField(field);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        #endregion

        #region Template Commands
        internal Task NewAsync() => Controller.CommandsHandler.NewAsync();

        internal bool CanNew() => Controller.CommandsHandler.CanNew();

        internal Task OpenDefaultAsync(IRichReportTemplatePreviewItem item) => Controller.CommandsHandler.OpenDefaultAsync(item);

        internal bool CanOpenDefault(IRichReportTemplatePreviewItem item) => Controller.CommandsHandler.CanOpenDefault(item);

        internal Task OpenAsync(IRichReportTemplatePreviewItem item) => Controller.CommandsHandler.OpenAsync(item);

        internal bool CanOpen(IRichReportTemplatePreviewItem item) => Controller.CommandsHandler.CanOpen(item);

        internal Task SaveAsync() => Controller.CommandsHandler.SaveAsync();

        internal bool CanSave() => Controller.CommandsHandler.CanSave();

        internal Task SaveAsAsync(string name) => Controller.CommandsHandler.SaveAsAsync(name);

        internal bool CanSaveAs(string name) => Controller.CommandsHandler.CanSaveAs(name);

        internal void Export() => Controller.CommandsHandler.Export();

        internal bool CanExport() => Controller.CommandsHandler.CanExport();

        internal Task ImportAsync() => Controller.CommandsHandler.ImportAsync();

        internal bool CanImport() => Controller.CommandsHandler.CanImport();

        internal Task ShowPrintPreviewAsync() => Controller.CommandsHandler.ShowPrintPreviewAsync();

        internal bool CanShowPreview() => Controller.CommandsHandler.CanShowPreview();

        #endregion

        #region IRichDocumentDesignerControl

        bool IRichReportDesignerControl.IsRightToLeft => FlowDirection.IsRightToLeft();

        bool IRichReportDesigner.IsInDesignTime => DesignerProperties.GetIsInDesignMode(this);

        IRichEditorControl IRichEditorControlOwner.Editor => editor;

        IRichReport IRichReportDesigner.Report => Report;

        IRichReportTemplate IRichReportDesignerControl.ActiveReportTemplate => ActiveReportTemplate;

        IBackstageManager<RichReportDesignerBackstageTab> IRichReportDesignerControl.BackstageManager => _backstageManager;

        IRichDisplayFieldGroupCollection IRichReportDesigner.DisplayFields => DisplayFields;

        T IRichReportDesigner.GetService<T>() => GetService<T>();

        Window IRichReportDesignerControl.GetWindow() => Window.GetWindow(this);

        void IRichReportDesignerControl.RaiseActiveTemplateChanged(IRichReportTemplate template)
        {
            SetValue(ActiveReportTemplatePropertyKey, template);

            CurrentTemplateChanged?.Invoke(this, new RichReportDesignerTemplateEventArgs(template));

            if (AutoUpdateWindowTitle)
            {
                template.DoIfNotNull(t =>
                {
                    ((IRichReportDesignerControl)this).GetWindow().DoIfNotNull(x => x.Title = t.GetDesignerTitle());
                });
            }
        }

        void IRichReportDesignerControl.RaiseTemplateSaved(IRichReportTemplate template, bool isNew)
        {
            TemplateSaved?.Invoke(this, new RichReportDesignerTemplateSavedEventArgs(template, isNew));

            Report.DoIfNotNull(async r =>
            {
                try
                {
                    DataChangeMessageType changedType = isNew ? DataChangeMessageType.Insert : DataChangeMessageType.Update;
                    await r.StoreTemplatesSource.OnItemChangedAsync(changedType, template.Id);
                }
                catch (Exception ex)
                {
                    ((IRichReportDesigner)this).HandleException(ex);
                }
            });
        }

        void IRichReportDesignerControl.ExecuteWithDispatcher(Action action, DispatcherPriority priority)
        {
            Dispatcher.BeginInvoke(action, priority);
        }

        public Task ExecuteWithDispatcherAsync(Action action, DispatcherPriority priority)
        {
            return Dispatcher.InvokeAsync(action, priority).Task;
        }

        void IRichReportDesigner.HandleException(Exception ex) => OnException(ex);

        void IRichReportDesigner.OnCalculatedDisplayFieldChanged(string fieldName) => Controller.Fields.OnCalculatedFieldChanged(fieldName);

        void IRichReportDesigner.OnCalculatedDisplayFieldRemoved(string fieldName) => Controller.Fields.OnCalculatedFieldRemoved(fieldName);

        bool IRichReportDesigner.CanRemoveCalculatedField(string fieldName) => Controller.Fields.CanRemoveCalculatedField(fieldName);

        #endregion
    }
}
