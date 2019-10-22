﻿using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.EAS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.FCS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;

namespace ESFA.DC.ILR.ReportService.Data.Mappers
{
    public class ReferenceDataMapper : IMapper<ReferenceDataService.Model.ReferenceDataRoot, ReferenceDataRoot>
    {
        private readonly IMapper<ReferenceDataService.Model.MetaData.MetaData, MetaData> _metaDataMapper;
        private readonly IMapper<IEnumerable<ReferenceDataService.Model.AppEarningsHistory.ApprenticeshipEarningsHistory>, IReadOnlyCollection<ApprenticeshipEarningsHistory>> _apprenticeshipEarningsHistoryMapper;
        private readonly IMapper<IEnumerable<ReferenceDataService.Model.EAS.EasFundingLine>, IReadOnlyCollection<EasFundingLine>> _easFundingLineMapper;
        private readonly IMapper<IEnumerable<ReferenceDataService.Model.FCS.FcsContractAllocation>, IReadOnlyCollection<FcsContractAllocation>> _fcsContractAllocationMapper;
        private readonly IMapper<IEnumerable<ReferenceDataService.Model.Organisations.Organisation>, IReadOnlyCollection<Organisation>> _organisationMapper;
        private readonly IMapper<ReferenceDataService.Model.PostcodesDevolution.DevolvedPostcodes, DevolvedPostcodes> _devolvedPostcodeMapper;
        private readonly IMapper<IEnumerable<ReferenceDataService.Model.LARS.LARSLearningDelivery>, IReadOnlyCollection<LARSLearningDelivery>> _larsLearningDeliveryMapper;
        private readonly IMapper<IEnumerable<ReferenceDataService.Model.LARS.LARSStandard>, IReadOnlyCollection<LARSStandard>> _larsStandardMapper;

        public ReferenceDataMapper(
            IMapper<ReferenceDataService.Model.MetaData.MetaData, MetaData> metaDataMapper,
            IMapper<IEnumerable<ReferenceDataService.Model.AppEarningsHistory.ApprenticeshipEarningsHistory>, IReadOnlyCollection<ApprenticeshipEarningsHistory>> apprenticeshipEarningsHistoryMapper,
            IMapper<IEnumerable<ReferenceDataService.Model.EAS.EasFundingLine>, IReadOnlyCollection<EasFundingLine>> easFundingLineMapper,
            IMapper<IEnumerable<ReferenceDataService.Model.FCS.FcsContractAllocation>, IReadOnlyCollection<FcsContractAllocation>> fcsContractAllocationMapper,
            IMapper<IEnumerable<ReferenceDataService.Model.Organisations.Organisation>, IReadOnlyCollection<Organisation>> organisationMapper,
            IMapper<ReferenceDataService.Model.PostcodesDevolution.DevolvedPostcodes, DevolvedPostcodes> devolvedPostcodeMapper,
            IMapper<IEnumerable<ReferenceDataService.Model.LARS.LARSLearningDelivery>, IReadOnlyCollection<LARSLearningDelivery>> larsLearningDeliveryMapper,
            IMapper<IEnumerable<ReferenceDataService.Model.LARS.LARSStandard>, IReadOnlyCollection<LARSStandard>> larsStandardMapper)
        {
            _metaDataMapper = metaDataMapper;
            _apprenticeshipEarningsHistoryMapper = apprenticeshipEarningsHistoryMapper;
            _easFundingLineMapper = easFundingLineMapper;
            _fcsContractAllocationMapper = fcsContractAllocationMapper;
            _organisationMapper = organisationMapper;
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
                FCSContractAllocations = _fcsContractAllocationMapper.MapData(root.FCSContractAllocations),
                LARSLearningDeliveries = _larsLearningDeliveryMapper.MapData(root.LARSLearningDeliveries),
                LARSStandards = _larsStandardMapper.MapData(root.LARSStandards),
                Organisations = _organisationMapper.MapData(root.Organisations),
                DevolvedPostocdes = _devolvedPostcodeMapper.MapData(root.DevolvedPostocdes),
                ULNs = root.ULNs
            };
        }
    }
}
