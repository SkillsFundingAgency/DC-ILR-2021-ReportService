using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm81Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        private readonly IMapper<FundingService.FM81.FundingOutput.Model.Output.FM81Global, Models.Fm81.FM81Global> _fm81Mapper;

        public Fm81Provider(
            IFileService fileService, 
            IJsonSerializationService serializationService, 
            IMapper<FundingService.FM81.FundingOutput.Model.Output.FM81Global, Models.Fm81.FM81Global> fm81Mapper) 
            : base(fileService, serializationService)
        {
            _fm81Mapper = fm81Mapper;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var fm81Global = await ProvideAsync<FM81Global>(reportServiceContext.FundingFM81OutputKey, reportServiceContext.Container, cancellationToken) as FM81Global;

            return _fm81Mapper.MapData(fm81Global);
        }
    }
}
