using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Validation result for a table's regions
    /// </summary>
    public class TableValidationResult
    {
        public int TableIndex { get; set; }

        public int TotalRows { get; set; }

        public bool IsValid { get; set; }

        public bool HasInvalidRows { get; set; }

        public List<RowValidationResult> RowResults { get; set; } = new List<RowValidationResult>();
    }
}
