using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using ESFA.DC.ILR.ReportService.Models.EAS;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class EasProvider : AbstractFileServiceProvider, IExternalDataProvider
    {
        private readonly IMapper<IEnumerable<ReferenceDataService.Model.EAS.EasFundingLine>, IReadOnlyCollection<EasFundingLine>> _easMapper;

        public EasProvider(
            IFileService fileService, 
            ISerializationService serializationService, 
            IMapper<IEnumerable<ReferenceDataService.Model.EAS.EasFundingLine>, IReadOnlyCollection<EasFundingLine>> easMapper) : base(fileService, serializationService)
        {
            _easMapper = easMapper;
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var referenceData = await ProvideAsync<ReferenceDataRoot>(reportServiceContext.IlrReferenceDataKey, reportServiceContext.Container, cancellationToken) as ReferenceDataRoot;

            var easFundingLines = referenceData?.EasFundingLines;

            return _easMapper.MapData(easFundingLines);
        }
    }
}
