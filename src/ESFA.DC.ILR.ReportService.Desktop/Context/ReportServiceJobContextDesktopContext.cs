using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Desktop.Context
{
    public class ReportServiceJobContextDesktopContext : IReportServiceContext
    {
        private readonly IDesktopContext _desktopContext;

        public ReportServiceJobContextDesktopContext(IDesktopContext desktopContext, IEnumerable<IReportFilterQuery> reportFilters)
        {
            _desktopContext = desktopContext;
            ReportFilters = reportFilters;
        }

        public long JobId { get; }
        public int Ukprn => int.Parse(_desktopContext.KeyValuePairs[ILRContextKeys.Ukprn].ToString());
        public string Filename => _desktopContext.KeyValuePairs[ILRContextKeys.Filename].ToString();
        
        public string OriginalFilename => _desktopContext.KeyValuePairs[ILRContextKeys.OriginalFilename].ToString();

        public string Container => _desktopContext.KeyValuePairs[ILRContextKeys.Container].ToString();

        public string ValidationErrorsKey => _desktopContext.KeyValuePairs[ILRContextKeys.ValidationErrors].ToString();

        public DateTime SubmissionDateTimeUtc => _desktopContext.DateTimeUtc;
        public long FileSizeInBytes => long.Parse(_desktopContext.KeyValuePairs[ILRContextKeys.FileSizeInBytes].ToString());

        public int ValidLearnRefNumbersCount => int.Parse(_desktopContext.KeyValuePairs[ILRContextKeys.ValidLearnRefNumbersCount].ToString());
        public int InvalidLearnRefNumbersCount => int.Parse(_desktopContext.KeyValuePairs[ILRContextKeys.InvalidLearnRefNumbersCount].ToString());
        public int ValidationTotalErrorCount => int.Parse(_desktopContext.KeyValuePairs[ILRContextKeys.ValidationTotalErrorCount].ToString());
        public int ValidationTotalWarningCount => int.Parse(_desktopContext.KeyValuePairs[ILRContextKeys.ValidationTotalWarningCount].ToString());
        public string ValidationErrorsLookupsKey => _desktopContext.KeyValuePairs[ILRContextKeys.ValidationErrorLookups].ToString();
        public string FundingFM81OutputKey => _desktopContext.KeyValuePairs[ILRContextKeys.FundingFm81Output].ToString();
        public string FundingFM70OutputKey => _desktopContext.KeyValuePairs[ILRContextKeys.FundingFm70Output].ToString();
        public string FundingFM36OutputKey => _desktopContext.KeyValuePairs[ILRContextKeys.FundingFm36Output].ToString();
        public string FundingFM35OutputKey => _desktopContext.KeyValuePairs[ILRContextKeys.FundingFm35Output].ToString();
        public string FundingFM25OutputKey => _desktopContext.KeyValuePairs[ILRContextKeys.FundingFm25Output].ToString();
        public string FundingALBOutputKey => _desktopContext.KeyValuePairs[ILRContextKeys.FundingAlbOutput].ToString();
        public string ValidLearnRefNumbersKey => _desktopContext.KeyValuePairs[ILRContextKeys.ValidLearnRefNumbers].ToString();

        public IEnumerable<string> Tasks
        {
            get
            {
                var tasks = _desktopContext.KeyValuePairs[ILRContextKeys.ReportTasks].ToString().Split(new [] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return tasks;
            }
        }

        public string InvalidLearnRefNumbersKey => _desktopContext.KeyValuePairs[ILRContextKeys.InvalidLearnRefNumbers].ToString();
        public string CollectionName { get; }
        public int ReturnPeriod => int.Parse(_desktopContext.KeyValuePairs[ILRContextKeys.ReturnPeriod].ToString());
        public string IlrReferenceDataKey => _desktopContext.KeyValuePairs[ILRContextKeys.IlrReferenceData].ToString();

        public string ReportOutputFileNames
        {
            get => _desktopContext.KeyValuePairs[ILRContextKeys.ReportOutputFileNames].ToString();
            set => _desktopContext.KeyValuePairs[ILRContextKeys.ReportOutputFileNames] = value;
        }

        public string ServiceReleaseVersion { get; set; }

        public IEnumerable<IReportFilterQuery> ReportFilters { get; }
    }
}
