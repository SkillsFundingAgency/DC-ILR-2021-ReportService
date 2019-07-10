using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.JobContextManager.Model;

namespace ESFA.DC.ILR.ReportService.Stateless.Context
{
    public sealed class ReportServiceJobContextMessageContext : IReportServiceContext
    {
        private readonly JobContextMessage _jobContextMessage;

        public ReportServiceJobContextMessageContext(JobContextMessage jobContextMessage)
        {
            _jobContextMessage = jobContextMessage;
        }

        public int Ukprn => int.Parse(_jobContextMessage.KeyValuePairs[ILRContextKeys.Ukprn].ToString());

        public string Filename => _jobContextMessage.KeyValuePairs[ILRContextKeys.Filename].ToString();

        public string OriginalFilename => _jobContextMessage.KeyValuePairs.ContainsKey("OriginalFilename")
            ? _jobContextMessage.KeyValuePairs["OriginalFilename"].ToString()
            : _jobContextMessage.KeyValuePairs[ILRContextKeys.Filename].ToString();

        public string Container => _jobContextMessage.KeyValuePairs[ILRContextKeys.Container].ToString();

        public DateTime SubmissionDateTimeUtc => _jobContextMessage.SubmissionDateTimeUtc;

        public long FileSizeInBytes => long.Parse(_jobContextMessage.KeyValuePairs[ILRContextKeys.FileSizeInBytes].ToString());

        public int ValidLearnRefNumbersCount => int.Parse(_jobContextMessage.KeyValuePairs[ILRContextKeys.ValidLearnRefNumbersCount].ToString());

        public int InvalidLearnRefNumbersCount => int.Parse(_jobContextMessage.KeyValuePairs[ILRContextKeys.InvalidLearnRefNumbersCount].ToString());

        public int ValidationTotalErrorCount => int.Parse(_jobContextMessage.KeyValuePairs[ILRContextKeys.ValidationTotalErrorCount].ToString());

        public int ValidationTotalWarningCount => int.Parse(_jobContextMessage.KeyValuePairs[ILRContextKeys.ValidationTotalErrorCount].ToString());

        public string ValidationErrorsKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.ValidationErrors].ToString();

        public string ValidationErrorsLookupsKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.ValidationErrorLookups].ToString();

        public string FundingFM81OutputKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.FundingFm81Output].ToString();

        public string FundingFM70OutputKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.FundingFm70Output].ToString();

        public string FundingFM36OutputKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.FundingFm36Output].ToString();

        public string FundingFM35OutputKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.FundingFm35Output].ToString();

        public string FundingFM25OutputKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.FundingFm25Output].ToString();

        public string FundingALBOutputKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.FundingAlbOutput].ToString();

        public string ValidLearnRefNumbersKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.ValidLearnRefNumbers].ToString();

        public IEnumerable<string> Tasks => _jobContextMessage.Topics[_jobContextMessage.TopicPointer].Tasks.SelectMany(x => x.Tasks);

        public string InvalidLearnRefNumbersKey => _jobContextMessage.KeyValuePairs[ILRContextKeys.ValidLearnRefNumbers].ToString();

        public string CollectionName => _jobContextMessage.KeyValuePairs["CollectionName"].ToString();

        public int ReturnPeriod => int.Parse(_jobContextMessage.KeyValuePairs[ILRContextKeys.ReturnPeriod].ToString());

        public string IlrReferenceDataKey
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string ReportOutputFileNames
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public long JobId => _jobContextMessage.JobId;
    }
}
