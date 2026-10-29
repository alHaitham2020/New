using DevExpress.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISoft.RichReports.Editor.Data
{
    /// <summary>
    /// Static helper for recursively traversing and flattening IDataColumnInfo trees.
    /// </summary>
    public static class RichDataColumnHelper
    {
        // =====================================================================
        // FLATTEN (yield-based)
        // =====================================================================

        /// <summary>
        /// Flattens a single column and all its descendants (depth-first, pre-order).
        /// </summary>
        public static IEnumerable<IDataColumnInfo> Flatten(IDataColumnInfo column)
        {
            if (column == null) yield break;

            yield return column;

            if (column.Columns == null) yield break;

            foreach (var descendant in Flatten(column.Columns))
                yield return descendant;
        }

        /// <summary>
        /// Flattens a list of columns and all their descendants (depth-first, pre-order).
        /// </summary>
        public static IEnumerable<IDataColumnInfo> Flatten(IEnumerable<IDataColumnInfo> columns)
        {
            if (columns == null) yield break;

            foreach (var column in columns)
            {
                if (column == null) continue;

                yield return column;

                if (column.Columns == null) continue;

                foreach (var descendant in Flatten(column.Columns))
                    yield return descendant;
            }
        }

        // =====================================================================
        // FLATTEN (iterative — safe for very deep trees)
        // =====================================================================

        /// <summary>
        /// Iterative flatten (avoids stack overflow on deep trees).
        /// </summary>
        public static IEnumerable<IDataColumnInfo> FlattenIterative(IEnumerable<IDataColumnInfo> columns)
        {
            if (columns == null) yield break;

            var stack = new Stack<IDataColumnInfo>();

            // Push in reverse to preserve left-to-right order.
            var list = columns as IList<IDataColumnInfo> ?? columns.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] != null)
                    stack.Push(list[i]);
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;

                if (current.Columns == null) continue;

                for (int i = current.Columns.Count - 1; i >= 0; i--)
                {
                    if (current.Columns[i] != null)
                        stack.Push(current.Columns[i]);
                }
            }
        }

        /// <summary>
        /// Iterative flatten for a single column.
        /// </summary>
        public static IEnumerable<IDataColumnInfo> FlattenIterative(IDataColumnInfo column)
            => column == null
                ? Enumerable.Empty<IDataColumnInfo>()
                : FlattenIterative(new[] { column });

        // =====================================================================
        // FLATTEN WITH PATH
        // =====================================================================

        /// <summary>
        /// Flattens with a dotted path for each column (e.g. "Parent.Child.Leaf").
        /// </summary>
        public static IEnumerable<RichDataColumnWithPath> FlattenWithPath(IEnumerable<IDataColumnInfo> columns)
            => FlattenWithPathInternal(columns, string.Empty);

        /// <summary>
        /// Flattens a single column with a dotted path.
        /// </summary>
        public static IEnumerable<RichDataColumnWithPath> FlattenWithPath(IDataColumnInfo column)
            => column == null
                ? Enumerable.Empty<RichDataColumnWithPath>()
                : FlattenWithPathInternal(new[] { column }, string.Empty);

        private static IEnumerable<RichDataColumnWithPath> FlattenWithPathInternal(IEnumerable<IDataColumnInfo> columns, string parentPath)
        {
            if (columns == null) yield break;

            foreach (var column in columns)
            {
                if (column == null) continue;

                string path = string.IsNullOrEmpty(parentPath)
                    ? column.Name
                    : $"{parentPath}.{column.Name}";

                yield return new RichDataColumnWithPath(column, path);

                if (column.Columns == null) continue;

                foreach (var item in FlattenWithPathInternal(column.Columns, path))
                    yield return item;
            }
        }

        // =====================================================================
        // TRAVERSE (Action-based)
        // =====================================================================

        /// <summary>
        /// Traverses a single column and all descendants, invoking <paramref name="action"/> on each.
        /// </summary>
        public static void Traverse(IDataColumnInfo column, Action<IDataColumnInfo> action)
        {
            if (column == null || action == null) return;

            action(column);

            if (column.Columns == null) return;

            foreach (var child in column.Columns)
                Traverse(child, action);
        }

        /// <summary>
        /// Traverses a list of columns and all descendants, invoking <paramref name="action"/> on each.
        /// </summary>
        public static void Traverse(IEnumerable<IDataColumnInfo> columns, Action<IDataColumnInfo> action)
        {
            if (columns == null || action == null) return;

            foreach (var column in columns)
            {
                if (column == null) continue;

                action(column);

                if (column.Columns == null) continue;

                Traverse(column.Columns, action);
            }
        }

        // =====================================================================
        // WRITE (StringBuilder, indented tree)
        // =====================================================================

        /// <summary>
        /// Writes a single column tree to a StringBuilder with indentation.
        /// </summary>
        public static void Write(IDataColumnInfo column, StringBuilder sb, int indent = 0)
        {
            if (column == null || sb == null) return;

            AppendColumn(sb, column, indent);

            if (column.Columns == null) return;

            foreach (var child in column.Columns)
                Write(child, sb, indent + 1);
        }

        /// <summary>
        /// Writes a list of column trees to a StringBuilder with indentation.
        /// </summary>
        public static void Write(IEnumerable<IDataColumnInfo> columns, StringBuilder sb, int indent = 0)
        {
            if (columns == null || sb == null) return;

            foreach (var column in columns)
            {
                if (column == null) continue;

                AppendColumn(sb, column, indent);

                if (column.Columns == null) continue;

                Write(column.Columns, sb, indent + 1);
            }
        }

        /// <summary>
        /// Returns the indented tree as a string.
        /// </summary>
        public static string DumpTree(IEnumerable<IDataColumnInfo> columns)
        {
            var sb = new StringBuilder();
            Write(columns, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Returns the indented tree as a string.
        /// </summary>
        public static string DumpTree(IDataColumnInfo column)
        {
            var sb = new StringBuilder();
            Write(column, sb);
            return sb.ToString();
        }

        private static void AppendColumn(StringBuilder sb, IDataColumnInfo column, int indent)
        {
            string pad = new string(' ', indent * 2);
            sb.AppendLine(
                $"{pad}- {column.Caption} [{column.Name}] " +
                $"Field={column.FieldName}, Type={column.FieldType?.Name}, " +
                $"Unbound={column.UnboundExpression}");
        }

        // =====================================================================
        // LOOKUP (Dictionary)
        // =====================================================================

        /// <summary>
        /// Builds a flat lookup keyed by dotted path (case-insensitive).
        /// </summary>
        public static Dictionary<string, IDataColumnInfo> BuildLookup(
            IEnumerable<IDataColumnInfo> columns,
            StringComparer comparer = null)
        {
            var dict = new Dictionary<string, IDataColumnInfo>(
                comparer ?? StringComparer.OrdinalIgnoreCase);

            if (columns == null) return dict;

            foreach (var column in FlattenWithPath(columns))
                dict[column.Path] = column.Column;

            return dict;
        }

        /// <summary>
        /// Builds a flat lookup keyed by dotted path (case-insensitive) from a single column.
        /// </summary>
        public static Dictionary<string, IDataColumnInfo> BuildLookup(IDataColumnInfo column)
            => column == null
                ? new Dictionary<string, IDataColumnInfo>(StringComparer.OrdinalIgnoreCase)
                : BuildLookup(new[] { column });

        // =====================================================================
        // FIND
        // =====================================================================

        /// <summary>
        /// Finds the first column matching the predicate (pre-order DFS).
        /// </summary>
        public static IDataColumnInfo Find(
            IEnumerable<IDataColumnInfo> columns,
            Func<IDataColumnInfo, bool> predicate)
        {
            if (columns == null || predicate == null) return null;

            foreach (var column in columns)
            {
                if (column == null) continue;

                if (predicate(column)) return column;

                if (column.Columns == null) continue;

                var found = Find(column.Columns, predicate);
                if (found != null) return found;
            }

            return null;
        }

        /// <summary>
        /// Finds the first column matching the predicate.
        /// </summary>
        public static IDataColumnInfo Find(
            IDataColumnInfo column,
            Func<IDataColumnInfo, bool> predicate)
            => column == null || predicate == null
                ? null
                : Find(new[] { column }, predicate);

        /// <summary>
        /// Finds all columns matching the predicate.
        /// </summary>
        public static IEnumerable<IDataColumnInfo> FindAll(
            IEnumerable<IDataColumnInfo> columns,
            Func<IDataColumnInfo, bool> predicate)
            => predicate == null
                ? Enumerable.Empty<IDataColumnInfo>()
                : Flatten(columns).Where(predicate);

        // =====================================================================
        // SELECT / MAP
        // =====================================================================

        /// <summary>
        /// Projects each column in the tree to a value (pre-order).
        /// </summary>
        public static IEnumerable<TResult> Select<TResult>(
            IEnumerable<IDataColumnInfo> columns,
            Func<IDataColumnInfo, TResult> selector)
            => selector == null
                ? Enumerable.Empty<TResult>()
                : Flatten(columns).Select(selector);

        // =====================================================================
        // GROUPING
        // =====================================================================

        /// <summary>
        /// Groups all columns by field type.
        /// </summary>
        public static ILookup<Type, IDataColumnInfo> GroupByFieldType(
            IEnumerable<IDataColumnInfo> columns)
            => Flatten(columns).ToLookup(c => c.FieldType);
    }
}
