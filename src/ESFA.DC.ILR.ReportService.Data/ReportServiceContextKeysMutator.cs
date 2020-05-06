using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Data
{
    public class ReportServiceContextKeysMutator : IReportServiceContextKeysMutator
    {
        public async Task<IReportServiceContext> MutateAsync(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData, CancellationToken cancellationToken)
        {
            await AddIlrReportingFilename(reportServiceContext,  cancellationToken);
            await AddEasReportingFilename(reportServiceContext, reportServiceDependentData, cancellationToken);

            return reportServiceContext;
        }

        private async Task AddIlrReportingFilename(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var ilrReportingFilename = reportServiceContext.OriginalFilename ?? reportServiceContext.Filename;

            reportServiceContext.IlrReportingFilename = ilrReportingFilename;
            reportServiceContext.LastIlrFileUpdate = reportServiceContext.SubmissionDateTimeUtc.ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat);
        }

        private async Task AddEasReportingFilename(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData, CancellationToken cancellationToken)
        {
            var referenceDataEASFile = reportServiceDependentData.Get<ReferenceDataRoot>().MetaDatas.ReferenceDataVersions.EasFileDetails;

            reportServiceContext.EasReportingFilename = referenceDataEASFile?.FileName;
            reportServiceContext.LastEasFileUpdate =
                referenceDataEASFile?.UploadDateTime != null ?
                referenceDataEASFile?.UploadDateTime.Value.ToString(ReportServiceConstants.LastFileUpdateDateTimeFormat) : null;
        }
    }
}
