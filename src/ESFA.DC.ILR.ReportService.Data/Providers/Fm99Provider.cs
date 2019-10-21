using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm99Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        private readonly IFm99Mapper _fm99Mapper;

        public Fm99Provider(IFileService fileService, IJsonSerializationService serializationService, IFm99Mapper fm99Mapper) 
            : base(fileService, serializationService)
        {
            _fm99Mapper = fm99Mapper;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext,CancellationToken cancellationToken)
        {
            var albGlobal = await ProvideAsync<ALBGlobal>(reportServiceContext.FundingALBOutputKey, reportServiceContext.Container, cancellationToken) as ALBGlobal;

            return _fm99Mapper.MapData(albGlobal);
        }      
    }
}
