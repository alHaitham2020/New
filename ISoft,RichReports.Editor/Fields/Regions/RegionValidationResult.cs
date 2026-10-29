using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Validation result for mail merge regions
    /// </summary>
    public class RegionValidationResult
    {
        public bool IsValid { get; set; } = true;

        public List<RegionValidationError> Errors { get; set; } = new List<RegionValidationError>();

        public List<RegionValidationWarning> Warnings { get; set; } = new List<RegionValidationWarning>();

        public int TotalRegionsChecked { get; set; }

        public DateTime ValidationTime { get; set; } = DateTime.Now;
    }
}
