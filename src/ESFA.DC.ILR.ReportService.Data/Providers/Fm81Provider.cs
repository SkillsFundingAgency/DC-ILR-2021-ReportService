using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm81Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public Fm81Provider(IFileService fileService, IJsonSerializationService serializationService) 
            : base(fileService, serializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
            => await ProvideAsync<FM81Global>(reportServiceContext.FundingFM81OutputKey, reportServiceContext.Container, cancellationToken);
    }
}
