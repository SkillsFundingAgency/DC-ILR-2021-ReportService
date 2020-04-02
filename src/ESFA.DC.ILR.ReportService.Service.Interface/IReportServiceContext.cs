using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Service.Interface
{
    public interface IReportServiceContext
    {
        long JobId { get; }

        int Ukprn { get; }

        string Filename { get; }

        string IlrReportingFilename { get; }

        string Container { get; }

        DateTime SubmissionDateTimeUtc { get; }

        long FileSizeInBytes { get; }

        int ValidLearnRefNumbersCount { get; }

        int InvalidLearnRefNumbersCount { get; }

        int ValidationTotalErrorCount { get; }

        int ValidationTotalWarningCount { get; }

        string ValidationErrorsKey { get; }

        string ValidationErrorsLookupsKey { get; }

        string FundingFM81OutputKey { get; }

        string FundingFM70OutputKey { get; }

        string FundingFM36OutputKey { get; }

        string FundingFM35OutputKey { get; }

        string FundingFM25OutputKey { get; }

        string FundingALBOutputKey { get; }

        string ValidLearnRefNumbersKey { get; }

        string FrmReferenceDataOutputKey { get; }

        IEnumerable<string> Tasks { get; }

        string InvalidLearnRefNumbersKey { get; }

        string CollectionName { get; }

        int ReturnPeriod { get; }

        string ReturnPeriodName { get; }

        string CollectionYear { get;  }

        string IlrReferenceDataKey { get; }

        string ReportOutputFileNames { get; set; }

        string ServiceReleaseVersion { get; }

        IEnumerable<IReportFilterQuery> ReportFilters { get; }
    }
}
