using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class IlrReferenceDataProviderService : AbstractFileServiceProvider, IExternalDataProvider
    {
        public IlrReferenceDataProviderService(IFileService fileService, IJsonSerializationService jsonSerializationService)
            : base(fileService, jsonSerializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
            => await ProvideAsync<ReferenceDataRoot>(reportServiceContext.IlrReferenceDataKey, reportServiceContext.Container, cancellationToken);
    }
}
