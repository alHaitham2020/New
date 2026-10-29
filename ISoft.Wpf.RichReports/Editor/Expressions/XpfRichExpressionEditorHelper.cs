using System.Windows;
using System.Windows.Media.Imaging;

namespace ISoft.Wpf.RichReports.Editor.Expressions
{
    public static class XpfRichExpressionEditorHelper
    {
        private static void ShowExpressionEditor(ExpressionEditorParams p)
        {
            if (p.DialogWindow == null)
            {
                p.DialogWindow = new ThemedWindow
                {
                    Owner = ((p.RootElement == null) ? null : Window.GetWindow(p.RootElement))
                };
            }
            else if (p.RootElement != null && p.DialogWindow.Owner == null)
            {
                p.DialogWindow.Owner = Window.GetWindow(p.RootElement);
            }
            
            p.DialogWindow.Title = EditorLocalizer.GetString(EditorStringId.ExpressionEditor_Title);
            p.DialogWindow.ShowInTaskbar = false;
            p.DialogWindow.Width = 700.0;
            p.DialogWindow.Height = 500.0;
            p.DialogWindow.MinHeight = 230.0;
            p.DialogWindow.MinWidth = 400.0;
            p.DialogWindow.WindowStyle = WindowStyle.ToolWindow;
            p.DialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (p.RootElement != null && CommonThemeHelper.IsTouchTheme(p.RootElement))
            {
                p.DialogWindow.Width = 950.0;
                p.DialogWindow.Height = 750.0;
            }

            if (p.Mode == ExpressionEditorMode.Standard)
            {
                throw new InvalidOperationException();
            }

            ISupportExpressionString editorToShow = p.AutoCompleteEditor;

            ((FrameworkElement)editorToShow).Loaded += delegate
            {
                if (p.CustomExpression != null)
                {
                    editorToShow.SetExpressionString(p.Column, p.CustomExpression);
                }
            };

            p.DialogWindow.Content = editorToShow;
            if (p.Theme != null)
            {
                CommonThemeHelper.SetTheme(p.DialogWindow, p.Theme);
            }
            else if (p.DialogWindow.Owner != null)
            {
                string themeName = ApplicationThemeManager.GetElementThemeName(p.DialogWindow.Owner);
                if (!string.IsNullOrWhiteSpace(themeName) && themeName != "None")
                {
                    CommonThemeHelper.SetThemeName(p.DialogWindow, themeName);
                }
                else
                {
                    CommonThemeHelper.SetThemeName(p.DialogWindow, ApplicationThemeManager.GetElementThemeName(p.RootElement));
                }
            }

            if (p.DialogWindow is ThemedWindow)
            {
                (p.DialogWindow as ThemedWindow).ShowDialog(MessageBoxButton.OKCancel);
            }
            else if (p.DialogWindow is DXDialogWindow)
            {
                (p.DialogWindow as DXDialogWindow).SmallIcon = new BitmapImage(UriHelper.GetUri("DevExpress.Xpf.Core", "/Editors/Images/ExpressionEditor/expression.png"));
                (p.DialogWindow as DXDialogWindow).ShowDialogWindow(MessageBoxButton.OKCancel);
            }

            p.ClosedHandler((p.DialogWindow.DialogResult.HasValue && p.DialogWindow.DialogResult.Value) ? editorToShow.GetExpressionString(p.Column) : null);
        }

        public static void ShowExpressionEditor(
            ExpressionEditorContext context,
            ExpressionEditorDialogClosedDelegate closedHandler,
            string expression = null,
            FrameworkElement rootElement = null)
        {
            ArgumentGuardHelper.ArgumentNotNull(context, nameof(context));

            DevExpress.Xpf.ExpressionEditor.ExpressionEditorControl control = new()
            {
                Context = context
            };

            ExpressionEditorParams p = new(null as IDataColumnInfo, closedHandler)
            {
                StandardEditor = null,
                AutoCompleteEditor = control,
                Mode = ExpressionEditorMode.AutoComplete,
                RootElement = rootElement,
                CustomExpression = expression
            };

            ShowExpressionEditor(p);
        }
    }
}
