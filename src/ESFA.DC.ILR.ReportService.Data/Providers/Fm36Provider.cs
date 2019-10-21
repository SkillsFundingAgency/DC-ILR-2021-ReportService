using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm36Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        private readonly IFm36Mapper _fm36Mapper;

        public Fm36Provider(IFileService fileService, IJsonSerializationService serializationService, IFm36Mapper fm36Mapper) 
            : base(fileService, serializationService)
        {
            _fm36Mapper = fm36Mapper;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var fm36Global = await ProvideAsync<FM36Global>(reportServiceContext.FundingFM36OutputKey, reportServiceContext.Container, cancellationToken) as FM36Global;

            return _fm36Mapper.MapData(fm36Global);
        }
            
    }
}
