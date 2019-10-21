using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;

namespace ESFA.DC.ILR.ReportService.Data.Mappers
{
    public class ReferenceDataMapper : IReferenceDataMapper
    {
        private readonly IMetaDataMapper _metaDataMapper;
        private readonly IApprenticeshipEarningsHistoryMapper _apprenticeshipEarningsHistoryMapper;
        private readonly IEasFundingLineMapper _easFundingLineMapper;
        private readonly IEmployerMapper _employerMapper;
        private readonly IEpaOrganisationMapper _epaOrganisationMapper;
        private readonly IFcsContractAllocationMapper _fcsContractAllocationMapper;
        private readonly IOrganisationMapper _organisationMapper;
        private readonly IPostcodeMapper _postcodeMapper;
        private readonly IDevolvedPostcodeMapper _devolvedPostcodeMapper;
        private readonly ILarsLearningDeliveryMapper _larsLearningDeliveryMapper;
        private readonly ILarsStandardMapper _larsStandardMapper;

        public ReferenceDataMapper(IMetaDataMapper metaDataMapper, IApprenticeshipEarningsHistoryMapper apprenticeshipEarningsHistoryMapper, IEasFundingLineMapper easFundingLineMapper, IEmployerMapper employerMapper, IEpaOrganisationMapper epaOrganisationMapper, IFcsContractAllocationMapper fcsContractAllocationMapper, IOrganisationMapper organisationMapper, IPostcodeMapper postcodeMapper, IDevolvedPostcodeMapper devolvedPostcodeMapper, ILarsLearningDeliveryMapper larsLearningDeliveryMapper, ILarsStandardMapper larsStandardMapper)
        {
            _metaDataMapper = metaDataMapper;
            _apprenticeshipEarningsHistoryMapper = apprenticeshipEarningsHistoryMapper;
            _easFundingLineMapper = easFundingLineMapper;
            _employerMapper = employerMapper;
            _epaOrganisationMapper = epaOrganisationMapper;
            _fcsContractAllocationMapper = fcsContractAllocationMapper;
            _organisationMapper = organisationMapper;
            _postcodeMapper = postcodeMapper;
            _devolvedPostcodeMapper = devolvedPostcodeMapper;
            _larsLearningDeliveryMapper = larsLearningDeliveryMapper;
            _larsStandardMapper = larsStandardMapper;
        }

        public ReferenceDataRoot MapData(ReferenceDataService.Model.ReferenceDataRoot root)
        {
            return new ReferenceDataRoot()
            {
                MetaDatas = _metaDataMapper.MapData(root.MetaDatas),
                AppsEarningsHistories = _apprenticeshipEarningsHistoryMapper.MapData(root.AppsEarningsHistories),
                EasFundingLines = _easFundingLineMapper.MapData(root.EasFundingLines),
                Employers = _employerMapper.MapData(root.Employers),
                EPAOrganisations = _epaOrganisationMapper.MapData(root.EPAOrganisations),
                FCSContractAllocations = _fcsContractAllocationMapper.MapData(root.FCSContractAllocations),
                LARSLearningDeliveries = _larsLearningDeliveryMapper.MapData(root.LARSLearningDeliveries),
                LARSStandards = _larsStandardMapper.MapData(root.LARSStandards),
                Organisations = _organisationMapper.MapData(root.Organisations),
                Postcodes = _postcodeMapper.MapData(root.Postcodes),
                DevolvedPostocdes = _devolvedPostcodeMapper.MapData(root.DevolvedPostocdes),
                ULNs = root.ULNs
            };
        }
    }
}
