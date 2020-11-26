using System;
using System.IO;
using System.Threading;
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

            reportServiceContext.IlrReportingFilename = Path.GetFileName(ilrReportingFilename);
            reportServiceContext.LastIlrFileUpdate = ilrDate.ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat);
        }

        private async Task AddEasReportingFilename(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData, CancellationToken cancellationToken)
        {
            var referenceDataEASFile = new EasFileDetails();

            if (reportServiceDependentData.Contains<ReferenceDataRoot>())
            {
                referenceDataEASFile = reportServiceDependentData.Get<ReferenceDataRoot>()?.MetaDatas.ReferenceDataVersions.EasFileDetails;
            }

            var easFileName = !string.IsNullOrWhiteSpace(referenceDataEASFile?.FileName) && referenceDataEASFile.FileName != _defaultEASValue ? Path.GetFileName(referenceDataEASFile?.FileName) : _defaultEASValue;
            reportServiceContext.EasReportingFilename = easFileName;

            if (referenceDataEASFile?.UploadDateTime != null)
            {
                var easLastUpdated = (DateTime)referenceDataEASFile?.UploadDateTime.Value;

                reportServiceContext.LastEasFileUpdate = _dateTimeProvider.ConvertUtcToUk(easLastUpdated).ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat);
            }

            if (referenceDataEASFile?.UploadDateTime == null)
            {

                reportServiceContext.LastEasFileUpdate = _defaultEASValue;
            }
        }
    }
}
