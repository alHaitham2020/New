using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit.API.Native;
using ISoft.RichEdit.Native;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ISoft.Wpf.RichReports.Editor.Internal
{
    internal sealed class RichEditFieldKeyboardGuard
    {
        private readonly RichEditControl _control;
        private bool _installed;

        private static readonly HashSet<Key> BlockedEditKeys = [Key.Back, Key.Delete, Key.X, Key.V, Key.Enter, Key.Tab, Key.Space];

        public RichEditFieldKeyboardGuard(RichEditControl control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            _control = control;
            Install();
        }

        public void Install()
        {
            if (_installed)
                return;

            _control.PreviewKeyDown += OnPreviewKeyDown;
            _control.PreviewTextInput += OnPreviewTextInput;

            DataObject.AddPastingHandler(_control, OnPasting);

            _installed = true;
        }

        public void UnInstall()
        {
            if (!_installed)
                return;

            _control.PreviewKeyDown -= OnPreviewKeyDown;
            _control.PreviewTextInput -= OnPreviewTextInput;

            DataObject.RemovePastingHandler(_control, OnPasting);

            _installed = false;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key key = e.Key == Key.System ? e.SystemKey : e.Key;

            // Allow Copy, Find, and other non-destructive shortcuts.
            if (Keyboard.Modifiers == ModifierKeys.Control && (key == Key.C || key == Key.F))
            {
                return;
            }

            if (!SelectionTouchesField())
                return;

            DocumentRange selection = _control.Document.Selection;

            // Allow Backspace/Delete only when the selection
            // exactly matches an entire field result.
            if (key == Key.Back || key == Key.Delete)
            {
                if (IsEntireFieldResultSelected(selection))
                    return;

                e.Handled = true;
                return;
            }

            // Prevent Cut, Paste, Enter, Tab, and Space.
            if (BlockedEditKeys.Contains(key))
            {
                e.Handled = true;
                return;
            }

            // Prevent destructive keyboard shortcuts such as
            // Ctrl+X and Ctrl+V while a field is selected.
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0 && (key == Key.X || key == Key.V))
            {
                e.Handled = true;
                return;
            }

            // Prevent ordinary character input.
            // PreviewTextInput also catches characters that cannot
            // be reliably identified by checking Key values.
            if (Keyboard.Modifiers == ModifierKeys.None && IsCharacterKey(key))
            {
                e.Handled = true;
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (SelectionTouchesField())
                e.Handled = true;
        }

        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (SelectionTouchesField())
                e.CancelCommand();
        }

        private bool SelectionTouchesField()
        {
            DocumentRange selection = _control.Document.Selection;

            return selection.WithSubDocument(doc =>
            {
                int start = selection.Start.ToInt();
                int end = selection.End.ToInt();

                foreach (Field field in doc.Fields)
                {
                    DocumentRange result = field.Range;

                    if (result == null)
                        continue;

                    int fieldStart = result.Start.ToInt();
                    int fieldEnd = result.End.ToInt();

                    if (start == end)
                    {
                        // A collapsed caret is protected only
                        // when it is inside the field result.
                        if (start >= fieldStart && start < fieldEnd)
                            return true;
                    }
                    else
                    {
                        // Any overlap with a field result is protected.
                        if (start < fieldEnd && end > fieldStart)
                            return true;
                    }
                }

                return false;
            });
        }

        private bool IsEntireFieldResultSelected(DocumentRange selection)
        {
            if (selection.Length == 0)
                return false;

            return selection.WithSubDocument(doc =>
            {
                int selectionStart = selection.Start.ToInt();
                int selectionEnd = selection.End.ToInt();

                foreach (Field field in doc.Fields)
                {
                    DocumentRange result = field.Range;

                    if (result == null || result.Length == 0)
                        continue;

                    int fieldStart = result.Start.ToInt();
                    int fieldEnd = result.End.ToInt();

                    // Require an exact match. Selecting only part
                    // of the result, or extra surrounding text,
                    // must not enable deletion.
                    if (selectionStart == fieldStart && selectionEnd == fieldEnd)
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        private static bool IsCharacterKey(Key key)
        {
            return (key >= Key.A && key <= Key.Z) ||
                   (key >= Key.D0 && key <= Key.D9) ||
                   (key >= Key.NumPad0 && key <= Key.NumPad9) ||
                   (key >= Key.Oem1 && key <= Key.Oem102) ||
                   key == Key.Decimal ||
                   key == Key.Add ||
                   key == Key.Subtract ||
                   key == Key.Multiply ||
                   key == Key.Divide;
        }
    }

}
