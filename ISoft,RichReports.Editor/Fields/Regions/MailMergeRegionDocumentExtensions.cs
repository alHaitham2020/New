using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Extension methods for MailMergeRegionDiscover
    /// </summary>
    public static class MailMergeRegionDocumentExtensions
    {
        /// <summary>
        /// Creates a discover from a document
        /// </summary>
        public static IRichMailMergeRegionScanner ScanRegions(this Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            var rootRegion = document.GetRegionHierarchy();
            return new RichMailMergeRegionScanner(rootRegion);
        }

        /// <summary>
        /// Creates a discover from a document with custom region tags
        /// </summary>
        public static IRichMailMergeRegionScanner ScanRegions(this Document document, string regionStartTag, string regionEndTag)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            var rootRegion = document.GetRegionHierarchy(regionStartTag, regionEndTag);
            return new RichMailMergeRegionScanner(rootRegion);
        }

        /// <summary>
        /// Creates a mail merge region scanner for the document
        /// </summary>
        public static IMailMergeDocumentRegionScanner CreateRegionScanner(
            this Document document,
            bool autoScan = true,
            string regionStartTag = null,
            string regionEndTag = null)
        {
            return new MailMergeDocumentRegionScanner(document, autoScan, regionStartTag, regionEndTag);
        }

        /// <summary>
        /// Creates a mail merge region scanner asynchronously
        /// </summary>
        public static async Task<IMailMergeDocumentRegionScanner> CreateRegionScannerAsync(
            this Document document,
            string regionStartTag = null,
            string regionEndTag = null,
            CancellationToken cancellationToken = default)
        {
            var scanner = new MailMergeDocumentRegionScanner(document, false, regionStartTag, regionEndTag);
            await scanner.ScanAsync(cancellationToken);
            return scanner;
        }
    }
}
