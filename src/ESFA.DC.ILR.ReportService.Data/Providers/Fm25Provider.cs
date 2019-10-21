using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm25Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        private readonly IFm25Mapper _fm25Mapper;

        public Fm25Provider(IFileService fileService, IJsonSerializationService serializationService, IFm25Mapper fm25Mapper) 
            : base(fileService, serializationService)
        {
            _fm25Mapper = fm25Mapper;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var fm25Global = await ProvideAsync<FM25Global>(reportServiceContext.FundingFM25OutputKey, reportServiceContext.Container, cancellationToken) as FM25Global;

            return _fm25Mapper.MapData(fm25Global);
        }
            
    }
}
