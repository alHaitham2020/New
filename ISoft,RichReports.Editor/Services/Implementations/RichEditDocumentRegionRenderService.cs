using ISoft.RichReports.Editor.Fields.Regions;
using ISoft.RichReports.Editor.Services.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Services.Implementations
{
    /// <summary>
    /// Service responsible for analyzing tables and injecting 
    /// TableStart/TableEnd tags for collection detection
    /// </summary>
    public class RichEditDocumentRegionRenderService : IRichEditDocumentRegionRenderService
    {
        #region Private Fields

        private readonly IRichEditDocumentFieldManagerService _fieldService;
        private readonly int _startRowIndex = 0;
        #endregion

        #region Constructors

        public RichEditDocumentRegionRenderService(IRichEditDocumentFieldManagerService fieldService)
        {
            _fieldService = fieldService ?? throw new ArgumentNullException(nameof(fieldService));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Main entry point: Process all tables in the document
        /// </summary>
        public void Render(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            using var scanner = document.CreateRegionScanner(autoScan: true);

            document.BeginUpdate();
            try
            {
                document.ProcessAllSubDocuments(doc =>
                {
                    var tableCount = doc.Tables.Count;
                    for (int i = 0; i < tableCount; i++)
                    {
                        var table = doc.Tables[i];
                        ProcessTable(doc, table, scanner);
                    }
                });
            }
            finally
            {
                document.EndUpdate();
            }
        }

        /// <summary>
        /// Renders tables asynchronously
        /// </summary>
        public async Task RenderAsync(Document document, CancellationToken cancellationToken = default)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            using var scanner = await document.CreateRegionScannerAsync(cancellationToken: cancellationToken);

            document.BeginUpdate();
            try
            {
                document.ProcessAllSubDocuments(doc =>
                {
                    var tableCount = doc.Tables.Count;
                    for (int i = 0; i < tableCount; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var table = doc.Tables[i];
                        ProcessTable(doc, table, scanner);
                    }
                });
            }
            finally
            {
                document.EndUpdate();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Process a single table with hierarchy awareness
        /// </summary>
        private void ProcessTable(SubDocument document, Table table, IMailMergeDocumentRegionScanner scanner)
        {
            var rowCount = table.Rows.Count;
            for (int r = _startRowIndex; r < rowCount; r++)
            {
                TableRow row = table.Rows[r];

                // Skip header row
                if (row.RepeatAsHeaderRow) continue;

                ProcessTableRow(document, row, scanner);
            }
        }

        /// <summary>
        /// Process a single row for dataTable fields
        /// </summary>
        private void ProcessTableRow(SubDocument document, TableRow row, IMailMergeDocumentRegionScanner scanner)
        {
            if (!CanInjectRowRegion(row)) return;

            string dataTableName = _fieldService.ExtractDataTableName(row);

            // Inject TableStart/TableEnd tags
            if (!string.IsNullOrEmpty(dataTableName))
            {
                // remove all regions in the row
                var regions = scanner.GetRegionsInRow(row);
                RemoveRegions(document, regions);

                // add the new region
                TryInsertRegion(row, dataTableName);
            }
        }

        /// <summary>
        /// Removes a specific field from a region
        /// </summary>
        private void RemoveRegions(SubDocument document, IRichReadOnlyRegionCollection regions)
        {
            try
            {
                foreach (var reg in regions)
                {
                    if (reg.StartField != null)
                    {
                        document.Delete(reg.StartField.Range);
                    }

                    if (reg.EndField != null)
                    {
                        document.Delete(reg.EndField.Range);
                    }
                }
            }
            catch
            { /* Ignore removal errors */ }
        }

        /// <summary>
        /// Direct injection attempt as fallback
        /// </summary>
        private void TryInsertRegion(TableRow row, string regionName)
        {
            try
            {
                // Create fields again
                var startField = _fieldService.CreateTableStartField(row.FirstCell.ContentRange.Start, regionName);
                var endField = _fieldService.CreateTableEndField(row.LastCell.ContentRange.End, regionName);

                if (startField != null) startField.Update();
                if (endField != null) endField.Update();
            }
            catch (Exception ex)
            {
                RunTimeLogger
                    .CurrentLogger
                    .LogException(ex,$"Failed direct injection for region '{regionName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a row can have regions injected
        /// </summary>
        private bool CanInjectRowRegion(TableRow row)
        {
            return row != null && row.Cells.Count > 1;
        }

        #endregion
    }
}
