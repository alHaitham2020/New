using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections.Generic;

namespace ISoft.RichEdit.Native
{
    internal static class RichDocumentDiscovery
    {
        /// <summary>The header/footer variants we inspect, in a stable order.</summary>
        private static readonly HeaderFooterType[] HeaderFooterTypes =
        [
            HeaderFooterType.First,
            HeaderFooterType.Odd,
            HeaderFooterType.Even
        ];

        private sealed class RichNativeFieldInfo : IRichDocField
        {
            public RichNativeFieldInfo(Field field, SubDocumentType documentType)
            {
                Field = field;
                DocumentType = documentType;
            }

            public Field Field { get; }

            public SubDocumentType DocumentType { get; }
        }

        /// <summary>
        /// Collects fields from the document according to the specified scope.
        /// </summary>
        /// <param name="document">The document to inspect.</param>
        /// <param name="scope">Which parts to include. Defaults to everything.</param>
        /// <returns>A list of all matched fields.</returns>
        public static IRichFieldCollection GetFields(Document document, RichDocumentSearchScope scope = RichDocumentSearchScope.All)
        {
            var results = new RichFieldCollection();
            if (document == null) return results;

            // 1. Body
            if ((scope & RichDocumentSearchScope.Body) != 0)
            {
                AddFields(document, results);
            }

            // 2. Headers & footers across all sections
            if ((scope & RichDocumentSearchScope.AllHeadersAndFooters) != 0)
            {
                foreach (Section section in document.Sections)
                {
                    foreach (var type in HeaderFooterTypes)
                    {
                        if (IsHeaderInScope(scope, type))
                            ProcessHeaderFooter(section, type, isHeader: true, sd => AddFields(sd, results));

                        if (IsFooterInScope(scope, type))
                            ProcessHeaderFooter(section, type, isHeader: false, sd => AddFields(sd, results));
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Collects tables from the document according to the specified scope.
        /// </summary>
        /// <param name="document">The document to inspect.</param>
        /// <param name="scope">Which parts to include. Defaults to everything.</param>
        /// <returns>A list of all matched tables.</returns>
        public static IList<Table> GetTables(Document document, RichDocumentSearchScope scope = RichDocumentSearchScope.All)
        {
            var results = new List<Table>();
            if (document == null) return results;

            // 1. Body
            if ((scope & RichDocumentSearchScope.Body) != 0)
            {
                results.AddRange(document.Tables);
            }

            // 2. Headers & footers across all sections
            if ((scope & RichDocumentSearchScope.AllHeadersAndFooters) != 0)
            {
                foreach (Section section in document.Sections)
                {
                    foreach (var type in HeaderFooterTypes)
                    {
                        if (IsHeaderInScope(scope, type))
                            ProcessHeaderFooter(section, type, isHeader: true, sd => AddTables(sd, results));

                        if (IsFooterInScope(scope, type))
                            ProcessHeaderFooter(section, type, isHeader: false, sd => AddTables(sd, results));
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Runs <paramref name="action"/> against each existing header and footer
        /// of every section in the document. Non-existent header/footers are skipped.
        /// </summary>
        /// <param name="document">The document to operate on.</param>
        /// <param name="action">The action to run against each <see cref="SubDocument"/>.</param>
        public static void ProcessSubDocuments(Document document, Action<SubDocument> action, RichDocumentSearchScope scope = RichDocumentSearchScope.All)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (action == null) throw new ArgumentNullException(nameof(action));

            // 1. Body
            if ((scope & RichDocumentSearchScope.Body) != 0)
            {
                action(document);
            }

            // 2. Headers & footers across all sections
            if ((scope & RichDocumentSearchScope.AllHeadersAndFooters) != 0)
            {
                foreach (Section section in document.Sections)
                {
                    foreach (var type in HeaderFooterTypes)
                    {
                        if (IsHeaderInScope(scope, type))
                            ProcessHeaderFooter(section, type, isHeader: true, action);

                        if (IsFooterInScope(scope, type))
                            ProcessHeaderFooter(section, type, isHeader: false, action);
                    }
                }
            }
        }

        private static void AddFields(SubDocument subDoc, RichFieldCollection results)
        {
            var documentType = subDoc.GetSubDocumentType();

            for (int i = 0; i < subDoc.Fields.Count; i++)
            {
                results.Add(new RichNativeFieldInfo(subDoc.Fields[i], documentType));
            }
        }

        private static void AddTables(SubDocument subDoc, List<Table> results)
        {
            var tables = subDoc.Tables;
            for (int i = 0, count = tables.Count; i < count; i++)
            {
                results.Add(tables[i]);
            }
        }

        private static bool IsHeaderInScope(RichDocumentSearchScope scope, HeaderFooterType type)
        {
            return type switch
            {
                HeaderFooterType.First => (scope & RichDocumentSearchScope.HeaderFirst) != 0,
                HeaderFooterType.Odd => (scope & RichDocumentSearchScope.HeaderOdd) != 0,
                HeaderFooterType.Even => (scope & RichDocumentSearchScope.HeaderEven) != 0,
                _ => false,
            };
        }

        private static bool IsFooterInScope(RichDocumentSearchScope scope, HeaderFooterType type)
        {
            return type switch
            {
                HeaderFooterType.First => (scope & RichDocumentSearchScope.FooterFirst) != 0,
                HeaderFooterType.Odd => (scope & RichDocumentSearchScope.FooterOdd) != 0,
                HeaderFooterType.Even => (scope & RichDocumentSearchScope.FooterEven) != 0,
                _ => false,
            };
        }

        private static void ProcessHeaderFooter(
            Section section,
            HeaderFooterType type,
            bool isHeader,
            Action<SubDocument> action)
        {
            SubDocument subDoc = null;

            try
            {
                if (isHeader)
                {
                    if (!section.HasHeader(type)) return;
                    subDoc = section.BeginUpdateHeader(type);
                }
                else
                {
                    if (!section.HasFooter(type)) return;
                    subDoc = section.BeginUpdateFooter(type);
                }

                action(subDoc);
            }
            finally
            {
                if (subDoc != null)
                {
                    if (isHeader) section.EndUpdateHeader(subDoc);
                    else section.EndUpdateFooter(subDoc);
                }
            }
        }

    }
}
