using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm99Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public Fm99Provider(IFileService fileService, IJsonSerializationService serializationService) 
            : base(fileService, serializationService)
        {
        }

        public Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
            => ProvideAsync<ALBGlobal>(reportServiceContext.FundingALBOutputKey, reportServiceContext.Container, cancellationToken);
    }
}
