using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm35Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        private readonly IFm35Mapper _fm35Mapper;

        public Fm35Provider(IFileService fileService, IJsonSerializationService serializationService, IFm35Mapper fm35Mapper) 
            : base(fileService, serializationService)
        {
            _fm35Mapper = fm35Mapper;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var fm35Global = await ProvideAsync<FM35Global>(reportServiceContext.FundingFM35OutputKey, reportServiceContext.Container, cancellationToken) as FM35Global;

            return _fm35Mapper.MapData(fm35Global);
        }
            
    }
}
