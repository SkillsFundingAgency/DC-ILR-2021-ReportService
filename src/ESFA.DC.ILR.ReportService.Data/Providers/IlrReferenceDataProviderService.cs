using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class IlrReferenceDataProviderService : AbstractFileServiceProvider, IExternalDataProvider
    {
        private readonly IMapper<ReferenceDataService.Model.ReferenceDataRoot, Models.ReferenceData.ReferenceDataRoot> _referenceDataMapper;

        public IlrReferenceDataProviderService(
            IFileService fileService, 
            IJsonSerializationService jsonSerializationService, 
            IMapper<ReferenceDataService.Model.ReferenceDataRoot, Models.ReferenceData.ReferenceDataRoot> referenceDataMapper)
            : base(fileService, jsonSerializationService)
        {
            _referenceDataMapper = referenceDataMapper;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var referenceData = await ProvideAsync<ReferenceDataRoot>(reportServiceContext.IlrReferenceDataKey, reportServiceContext.Container, cancellationToken) as ReferenceDataRoot;

            return _referenceDataMapper.MapData(referenceData);
        }            
    }
}
