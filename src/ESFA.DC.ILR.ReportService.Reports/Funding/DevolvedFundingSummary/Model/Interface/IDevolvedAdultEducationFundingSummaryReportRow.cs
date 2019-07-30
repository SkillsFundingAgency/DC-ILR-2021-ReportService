using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface
{
    public interface IDevolvedAdultEducationFundingSummaryReportRow
    {
        string Title { get; }

        decimal Period1 { get; }

        decimal Period2 { get; }

        decimal Period3 { get; }

        decimal Period4 { get; }

        decimal Period5 { get; }

        decimal Period6 { get; }

        decimal Period7 { get; }

        decimal Period8 { get; }

        decimal Period9 { get; }

        decimal Period10 { get; }

        decimal Period11 { get; }

        decimal Period12 { get; }

        decimal Period1To8 { get; }

        decimal Period9To12 { get; }

        decimal YearToDate { get; }

        decimal Total { get; }

        int CurrentPeriod { get; }
    }
}
