using System;

namespace ISoft.RichEdit.Native
{
    /// <summary>
    /// Specifies which document parts to include when collecting sub documents.
    /// Combine values using bitwise OR (|).
    /// </summary>
    [Flags]
    public enum RichDocumentSearchScope
    {
        /// <summary>No scope. Returns nothing.</summary>
        None = 0,

        /// <summary>The main document body only.</summary>
        Body = 1 << 0,          // 1

        /// <summary>First-page headers.</summary>
        HeaderFirst = 1 << 1,   // 2

        /// <summary>Odd-page headers.</summary>
        HeaderOdd = 1 << 2,     // 4

        /// <summary>Even-page headers.</summary>
        HeaderEven = 1 << 3,    // 8

        /// <summary>First-page footers.</summary>
        FooterFirst = 1 << 4,   // 16

        /// <summary>Odd-page footers.</summary>
        FooterOdd = 1 << 5,     // 32

        /// <summary>Even-page footers.</summary>
        FooterEven = 1 << 6,    // 64

        /// <summary>All headers of all sections.</summary>
        AllHeaders = HeaderFirst | HeaderOdd | HeaderEven,                       // 14

        /// <summary>All footers of all sections.</summary>
        AllFooters = FooterFirst | FooterOdd | FooterEven,                       // 112

        /// <summary>All headers and footers of all sections.</summary>
        AllHeadersAndFooters = AllHeaders | AllFooters,                          // 126

        /// <summary>Everything: body + all headers + all footers.</summary>
        All = Body | AllHeadersAndFooters                                        // 127
    }
}
