namespace ISoft.Wpf.RichReports.Editor.Expressions
{
    public class RichEditExpressionEditorControl : ExpressionEditorControl
    {
        protected override ExpressionEditorController CreateExpressionEditorController(
            IntelliSenseRichEdit richEdit,
            IAutoCompleteWindow autoComplete,
            IExpressionDocumentationControl documentationControl,
            IToolTipProvider toolTipProvider,
            ExpressionEditorContext context)
        {
            return new RichEditExpressionEditorController(
                richEdit, autoComplete, documentationControl, toolTipProvider, context);
        }

        /// <summary>
        /// Commits the current editor buffer so that
        /// <see cref="GetCurrentExpressionString"/> reflects the user's latest edits.
        /// Must be called before reading the edited expression.
        /// </summary>
        public void Save()
        {
            SaveCommand.TryExecuteCommand(null);
        }

        /// <summary>
        /// Returns <c>true</c> if the current expression can be saved (i.e. is syntactically valid).
        /// </summary>
        public bool CanSave() => CanSaveExpression();

        /// <summary>
        /// Returns the column metadata currently available to the expression editor.
        /// Does not commit the editor buffer or evaluate the expression.
        /// </summary>
        public IEnumerable<RichExpressionEditorColumnInfo> GetColumns()
            => Context.Columns.OfType<RichExpressionEditorColumnInfo>();

        /// <summary>
        /// Returns the subset of <see cref="GetColumns"/> that the current
        /// expression actually references.
        /// Commits the editor buffer via <see cref="Save"/> before parsing.
        /// Returns an empty sequence if the expression is invalid, cannot be parsed,
        /// or references no known columns.
        /// </summary>
        public IEnumerable<RichExpressionEditorColumnInfo> GetExpressionColumns()
        {
            try
            {
                // Bail out early if the expression can't be saved (i.e. is invalid).
                // Returning an empty array avoids allocating the result list and the
                // visitor for a case that will yield nothing anyway.
                if (!CanSaveExpression())
                    return Array.Empty<RichExpressionEditorColumnInfo>();

                // Commit the buffer so GetCurrentExpressionString() returns the latest text.
                Save();

                var criteria = CriteriaOperator.TryParse(GetCurrentExpressionString());
                if (criteria.ReferenceEqualsNull())
                    return Array.Empty<RichExpressionEditorColumnInfo>();

                // Collect the property names referenced by the expression.
                var visitor = new RichExpressionFieldNameStringVisitor();
                visitor.Visit(criteria);

                // ToLookup tolerates duplicate column names; each key maps to one or
                // more columns rather than throwing like ToDictionary would.
                var columnsByName = Context.Columns
                    .OfType<RichExpressionEditorColumnInfo>()
                    .ToLookup(x => x.Name, StringComparer.Ordinal);

                var result = new List<RichExpressionEditorColumnInfo>(visitor.ResultNames.Count);
                foreach (var kv in visitor.ResultFullNames)
                {
                    // Take the first column with this name (matches SingleOrDefault's
                    // behavior when there was exactly one).
                    var columnInfo = columnsByName[kv.Value].FirstOrDefault();
                    if (columnInfo != null)
                        result.Add(columnInfo);
                }

                return result;
            }
            catch (Exception ex)
            {
                // A failure here must not break the editor UI. Log it for diagnostics
                // and fall back to "no referenced columns" so callers can continue.
                RunTimeLogger.CurrentLogger.LogException(ex);
                return Array.Empty<RichExpressionEditorColumnInfo>();
            }
        }

        /// <summary>
        /// Returns the current expression string as edited by the user.
        /// Call <see cref="Save"/> first if you need the very latest text.
        /// </summary>
        public string GetCurrentExpressionString()
            => ((ISupportExpressionString)this).GetExpressionString(null);

        /// <summary>
        /// Replaces the editor's current expression with <paramref name="expression"/>.
        /// </summary>
        public void SetCurrentExpressionString(string expression)
            => ((ISupportExpressionString)this).SetExpressionString(null, expression);

        public bool HasDetailsColumns()
        {
            return GetExpressionColumns().Any(x => x.HasValidDataMember);
        }

        public string GetExpressionDataMember()
        {
            return GetExpressionColumns().FirstOrDefault(x => x.HasValidDataMember)?.DataMember ?? string.Empty;
        }

        public bool TryGetExpressionDataMember(out string result)
        {
            result = null;

            var dm = GetExpressionDataMember();
            if (!string.IsNullOrEmpty(dm))
            {
                result = dm;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> when the supplied columns reference <b>two or more
        /// distinct</b> <see cref="RichExpressionEditorColumnInfo.DataMember"/> values.
        /// This indicates the expression spans multiple detail sources and therefore
        /// cannot be resolved by a single data-member context.
        /// </summary>
        /// <remarks>
        /// Only detail columns with a non-blank <c>DataMember</c> are considered.
        /// Master columns and columns without a data member are ignored.
        /// </remarks>
        public bool HasDiffrentDataMembers()
        {
            return HasDiffrentDataMembers(GetExpressionColumns());
        }

        /// <summary>
        /// Returns <c>true</c> when the supplied columns reference <b>two or more
        /// distinct</b> <see cref="RichExpressionEditorColumnInfo.DataMember"/> values.
        /// This indicates the expression spans multiple detail sources and therefore
        /// cannot be resolved by a single data-member context.
        /// </summary>
        /// <remarks>
        /// Only detail columns with a non-blank <c>DataMember</c> are considered.
        /// Master columns and columns without a data member are ignored.
        /// </remarks>
        public bool HasDiffrentDataMembers(IEnumerable<RichExpressionEditorColumnInfo> columns)
        {
            if (columns == null)
                return false;

            // Distinct() short-circuits as soon as it sees a second value,
            // so we don't have to materialise the full set to answer the question.
            var distinct = columns
                .Where(x => x.EffectDataMemberConflict)
                .Select(x => x.DataMember)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)          // we only need to know if there is more than one
                .Count();

            return distinct > 1;
        }
    }
}