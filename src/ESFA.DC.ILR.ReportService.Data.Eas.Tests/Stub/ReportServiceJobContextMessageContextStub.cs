using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Data.Eas.Tests.Stub
{
    public sealed class ReportServiceJobContextMessageContextStub : IReportServiceContext
    {
        public ReportServiceJobContextMessageContextStub(int ukprn, string originalFilename, string filename, DateTime subimissionDate)
        {
            Ukprn = ukprn;
            OriginalFilename = originalFilename;
            Filename = filename;
            SubmissionDateTimeUtc = subimissionDate;
        }

        public int Ukprn { get; set; }
        public string OriginalFilename { get; set; }
        public string Filename { get; set; }
        public string IlrReportingFilename { get; set; }
        public string LastIlrFileUpdate { get; set; }
        public string EasReportingFilename { get; set; }
        public string LastEasFileUpdate { get; set; }
        public string Container => ILRContextKeys.Container;
        public DateTime SubmissionDateTimeUtc { get; set; }
        public long FileSizeInBytes => 100;
        public int ValidLearnRefNumbersCount => 1;
        public int InvalidLearnRefNumbersCount => 1;
        public int ValidationTotalErrorCount => 1;
        public int ValidationTotalWarningCount => 1;
        public string ValidationErrorsKey => ILRContextKeys.ValidationErrors;
        public string ValidationErrorsLookupsKey => ILRContextKeys.ValidationErrorLookups;
        public string FundingFM81OutputKey => ILRContextKeys.FundingFm81Output;
        public string FundingFM70OutputKey => ILRContextKeys.FundingFm70Output;
        public string FundingFM36OutputKey => ILRContextKeys.FundingFm36Output;
        public string FundingFM35OutputKey => ILRContextKeys.FundingFm35Output;
        public string FundingFM25OutputKey => ILRContextKeys.FundingFm25Output;
        public string FundingALBOutputKey => ILRContextKeys.FundingAlbOutput;
        public string ValidLearnRefNumbersKey => ILRContextKeys.ValidLearnRefNumbers;
        public string FrmReferenceDataOutputKey => "FrmReferenceData";
        public IEnumerable<string> Tasks => new List<string> { "Task1" };
        public string InvalidLearnRefNumbersKey => ILRContextKeys.InvalidLearnRefNumbers;
        public string CollectionName => "CollectionName";
        public int ReturnPeriod => 1;
        public string ReturnPeriodName => $"R{ReturnPeriod:D2}";
        public string IlrReferenceDataKey => ILRContextKeys.IlrReferenceData;
        public string ReportOutputFileNames
        {
            get => "ReportFileNames";
            set => ReportOutputFileNames = value;
        }
        public long JobId => 1;
        public string ServiceReleaseVersion => "1.1";
        public IEnumerable<IReportFilterQuery> ReportFilters => Enumerable.Empty<IReportFilterQuery>();
        public string CollectionYear => ILRContextKeys.CollectionYear;
    }
}
