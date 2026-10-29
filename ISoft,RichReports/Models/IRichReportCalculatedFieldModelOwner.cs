using System;
using System.Threading.Tasks;

namespace ISoft_RichReports.Models
{
    public interface IRichReportCalculatedFieldModelOwner
    {
        bool IsDuplicateFieldName(Guid? id, string name);

        bool IsDuplicateFieldDisplayName(Guid? id, string name);

        Task SaveAsync(RichReportCalculatedFieldModel model);
    }
}
