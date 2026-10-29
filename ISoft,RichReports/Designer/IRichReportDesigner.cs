using ISoft.RichReports.Designer.Fields;
using ISoft.RichReports.Models.Reports;
using System;

namespace ISoft.RichReports.Designer
{
    public interface IRichReportDesigner
    {
        bool IsInDesignTime { get; }

        bool IsLoading { get; }

        bool IsReady { get; }

        IRichDisplayFieldGroupCollection DisplayFields { get; }

        IRichReport Report { get; }

        IRichReportTemplate ActiveReportTemplate { get; }

        void HandleException(Exception ex);

        void OnCalculatedDisplayFieldChanged(string fieldName);

        void OnCalculatedDisplayFieldRemoved(string fieldName);

        bool CanRemoveCalculatedField(string fieldName);

        T GetService<T>() where T : class;
    }

}
