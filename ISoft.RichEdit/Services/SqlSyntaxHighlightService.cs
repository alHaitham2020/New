using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace ISoft.RichEdit.Services
{
    public class SqlSyntaxHighlightService : ISyntaxHighlightService
    {
        #region Fields and Proprties
        private readonly Document _document;
        private readonly Regex _keywordsRegex;
        private readonly Regex _quotedStringRegex = new(@"'([^']|'')*'", RegexOptions.Compiled);
        private readonly Regex _commentedStringRegex = new(@"(/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/)", RegexOptions.Compiled);

        // SQL keywords for syntax highlighting
        private static readonly string[] SqlKeywords =
        {
            "INSERT", "SELECT", "CREATE","ALTER","UPDATE","DROP", "PROC",
            "TRANSACTION","ROLLBACK","COMMIT","RETURN",
            "DELETE", "TABLE", "USE", "IDENTITY","AS","OUTPUT","LIKE","ORDER","BY",
            "ON", "OFF", "NOT", "NULL", "WITH", "SET", "GO", "DECLARE", "EXECUTE","EXEC",
            "NVARCHAR", "FROM", "INTO", "VALUES", "WHERE", "AND","BEGIN","END"
        };

        #endregion

        public SqlSyntaxHighlightService(Document document)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));

            // Build keywords regex pattern with word boundaries for exact matching
            string keywordsPattern = $@"\b({string.Join("|", SqlKeywords.Select(Regex.Escape))})\b";
            _keywordsRegex = new Regex(keywordsPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Executes syntax highlighting on the document
        /// </summary>
        public void Execute()
        {
            var tokens = ParseTokens();
            _document.ApplySyntaxHighlight(tokens);
        }

        /// <summary>
        /// Forces execution of syntax highlighting (alias for Execute)
        /// </summary>
        public void ForceExecute()
        {
            Execute();
        }

        /// <summary>
        /// Parses the document content and generates syntax highlighting tokens
        /// </summary>
        private List<SyntaxHighlightToken> ParseTokens()
        {
            var tokens = new List<SyntaxHighlightToken>();

            // Process different token types in order of precedence
            FindAndAddTokens(_quotedStringRegex, tokens, Color.Red);    // Strings in red
            FindAndAddTokens(_keywordsRegex, tokens, Color.Blue);       // Keywords in blue
            FindAndAddTokens(_commentedStringRegex, tokens, Color.Green); // Comments in green

            // Sort tokens by position and fill in plain text gaps
            return ProcessAndCompleteTokens(tokens);
        }

        /// <summary>
        /// Finds matches for a regex pattern and adds them as tokens if they don't overlap with existing tokens
        /// </summary>
        private void FindAndAddTokens(Regex regex, List<SyntaxHighlightToken> existingTokens, Color color)
        {
            var ranges = _document.FindAll(regex)?.GetAsFrozen() as DocumentRange[];
            if (ranges == null) return;

            foreach (var range in ranges)
            {
                if (!IsRangeOverlapping(range, existingTokens))
                {
                    existingTokens.Add(CreateToken(range.Start.ToInt(), range.End.ToInt(), color));
                }
            }
        }

        /// <summary>
        /// Sorts tokens and fills gaps with default text tokens
        /// </summary>
        private List<SyntaxHighlightToken> ProcessAndCompleteTokens(List<SyntaxHighlightToken> tokens)
        {
            // Sort tokens by start position for proper processing
            tokens.Sort((x, y) => x.Start.CompareTo(y.Start));

            return FillTextGaps(tokens);
        }

        /// <summary>
        /// Fills the gaps between syntax tokens with default text tokens
        /// </summary>
        private List<SyntaxHighlightToken> FillTextGaps(List<SyntaxHighlightToken> tokens)
        {
            var result = new List<SyntaxHighlightToken>();
            int docStart = _document.Range.Start.ToInt();
            int docEnd = _document.Range.End.ToInt();

            // Handle empty document case
            if (tokens.Count == 0)
            {
                result.Add(CreateToken(docStart, docEnd, Color.Black));
                return result;
            }

            // Add text before first token
            var firstToken = tokens[0];
            if (docStart < firstToken.Start)
            {
                result.Add(CreateToken(docStart, firstToken.Start, Color.Black));
            }
            result.Add(firstToken);

            // Fill gaps between tokens
            for (int i = 1; i < tokens.Count; i++)
            {
                var currentToken = tokens[i];
                var previousToken = tokens[i - 1];

                if (previousToken.End < currentToken.Start)
                {
                    result.Add(CreateToken(previousToken.End, currentToken.Start, Color.Black));
                }
                result.Add(currentToken);
            }

            // Add text after last token (using .NET 4.8 compatible index access)
            var lastToken = tokens[tokens.Count - 1];
            if (docEnd > lastToken.End)
            {
                result.Add(CreateToken(lastToken.End, docEnd, Color.Black));
            }

            return result;
        }

        /// <summary>
        /// Checks if a document range overlaps with any existing tokens
        /// </summary>
        private bool IsRangeOverlapping(DocumentRange range, List<SyntaxHighlightToken> tokens)
        {
            return tokens.Any(token => RangesOverlap(range, token));
        }

        /// <summary>
        /// Determines if a document range overlaps with a syntax token
        /// </summary>
        private bool RangesOverlap(DocumentRange range, SyntaxHighlightToken token)
        {
            int rangeStart = range.Start.ToInt();
            int rangeEnd = range.End.ToInt() - 1; // Convert exclusive end to inclusive

            // Check for any overlap between the ranges
            return (rangeStart >= token.Start && rangeStart < token.End) ||
                   (rangeEnd >= token.Start && rangeEnd < token.End) ||
                   (rangeStart <= token.Start && rangeEnd >= token.End);
        }

        /// <summary>
        /// Creates a new syntax highlight token with the specified properties
        /// </summary>
        private SyntaxHighlightToken CreateToken(int start, int end, Color foreColor)
        {
            var properties = new SyntaxHighlightProperties { ForeColor = foreColor };
            return new SyntaxHighlightToken(start, end - start, properties);
        }
    }
}
