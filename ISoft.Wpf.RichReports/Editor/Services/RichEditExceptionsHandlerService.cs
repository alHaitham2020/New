namespace ISoft.Wpf.RichReports.Editor.Services
{
    public class RichEditExceptionsHandlerService
    {
        private readonly RichEditControl _control;

        public RichEditExceptionsHandlerService(RichEditControl control)
        {
            _control = control;
        }

        public void Install()
        {
            if (_control != null)
                _control.UnhandledException += OnRichEditControlUnhandledException;
        }

        private void OnRichEditControlUnhandledException(object sender, RichEditUnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.Exception != null)
                    throw e.Exception;
            }
            catch (RichEditUnsupportedFormatException ex)
            {
                ex.ShowExceptionMessage();
                e.Handled = true;
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                ex.ShowExceptionMessage();
                e.Handled = true;
            }
            catch (System.IO.IOException ex)
            {
                ex.ShowExceptionMessage();
                e.Handled = true;
            }
            catch (System.InvalidOperationException ex)
            {
                if (ex.Message == DevExpress.XtraRichEdit.Localization.XtraRichEditLocalizer.GetString(DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.Msg_MagicNumberNotFound) ||
                    ex.Message == DevExpress.XtraRichEdit.Localization.XtraRichEditLocalizer.GetString(DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.Msg_UnsupportedDocVersion))
                {
                    ex.ShowExceptionMessage();
                    e.Handled = true;
                }
                else
                    throw;
            }
            catch (SystemException ex)
            {
                ex.ShowExceptionMessage();
                e.Handled = true;
            }
        }
    }
}
