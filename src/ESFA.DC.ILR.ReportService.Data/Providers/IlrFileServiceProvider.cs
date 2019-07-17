using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class IlrFileServiceProvider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public IlrFileServiceProvider(IFileService fileService, IXmlSerializationService xmlSerializationService)
        : base(fileService, xmlSerializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
            => await ProvideAsync<Message>(reportServiceContext.Filename, reportServiceContext.Container, cancellationToken);
    }
}
