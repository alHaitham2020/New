using ISoft_RichReports.Context;
using ISoft_RichReports.Models.Reports;
using System;

namespace ISoft_RichReports.Factories
{
    public interface IRichReportFactory<TReport> where TReport : IRichReport
    {
        TReport CreateReport(IRichReportContext context);
    }

    public abstract class RichReportFactoryBase<TReport> : IRichReportFactory<TReport> where TReport : IRichReport
    {
        private readonly Func<IRichReportContext,TReport> _factory;

        protected RichReportFactoryBase(Func<IRichReportContext,TReport> factory)
        {
            _factory = factory;
        }

        TReport IRichReportFactory<TReport>.CreateReport(IRichReportContext context)
        {
            return _factory(context);
        }
    }
}
