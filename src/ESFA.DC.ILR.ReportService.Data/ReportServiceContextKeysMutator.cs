﻿using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Data
{
    public class ReportServiceContextKeysMutator : IReportServiceContextKeysMutator
    {
        private const string _defaultEASValue = "N/A";
        private readonly IDateTimeProvider _dateTimeProvider;

        public ReportServiceContextKeysMutator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IReportServiceContext> MutateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData, CancellationToken cancellationToken)
        {
            await AddIlrReportingFilename(reportServiceContext,  cancellationToken);
            await AddEasReportingFilename(reportServiceContext, reportServiceDependentData, cancellationToken);

            return reportServiceContext;
        }

        private async Task AddIlrReportingFilename(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var ilrReportingFilename = reportServiceContext.OriginalFilename ?? reportServiceContext.Filename;
            var ilrDate = _dateTimeProvider.ConvertUtcToUk(reportServiceContext.SubmissionDateTimeUtc);

            reportServiceContext.IlrReportingFilename = ilrReportingFilename;
            reportServiceContext.LastIlrFileUpdate = ilrDate.ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat);
        }

        private async Task AddEasReportingFilename(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData, CancellationToken cancellationToken)
        {
            var referenceDataEASFile = new EasFileDetails();

            if (reportServiceDependentData.Contains<ReferenceDataRoot>())
            {
                referenceDataEASFile = reportServiceDependentData.Get<ReferenceDataRoot>()?.MetaDatas.ReferenceDataVersions.EasFileDetails;
            }

            reportServiceContext.EasReportingFilename = referenceDataEASFile?.FileName ?? _defaultEASValue;
            reportServiceContext.LastEasFileUpdate =
                referenceDataEASFile?.UploadDateTime != null ?
                referenceDataEASFile?.UploadDateTime.Value.ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat) : _defaultEASValue;
        }
    }
}
