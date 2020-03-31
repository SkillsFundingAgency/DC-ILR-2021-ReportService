using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using ESFA.DC.ILR.ReportService.Models.FRM;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class FrmReferenceDataProvider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public FrmReferenceDataProvider(
            IFileService fileService,
            IJsonSerializationService serializationService) 
            : base(fileService, serializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var referenceData = await ProvideAsync<ReferenceDataService.Model.FRM.FrmReferenceData>(reportServiceContext.FrmReferenceDataOutputKey, reportServiceContext.Container, cancellationToken) as ReferenceDataService.Model.FRM.FrmReferenceData;

            return MapData(referenceData);
        }

        public FrmReferenceData MapData(ReferenceDataService.Model.FRM.FrmReferenceData referenceData)
        {
            return new FrmReferenceData
            {
                Frm06Learners = MapData(referenceData.Frm06Learners)
            };
        }

        // Frm06 Data Mapper
        private IReadOnlyCollection<FrmLearner> MapData(IEnumerable<ReferenceDataService.Model.FRM.FrmLearner> frmLearners)
        {
            return frmLearners?.Select(MapFrm06Learner).ToList();
        }

        private FrmLearner MapFrm06Learner(ReferenceDataService.Model.FRM.FrmLearner learner)
        {
            return new FrmLearner
            {
                UKPRN = learner.UKPRN,
                OrgName = learner.OrgName,
                LearnRefNumber = learner.LearnRefNumber,
                LearnAimRef = learner.LearnAimRef,
                ProgTypeNullable = learner.ProgTypeNullable,
                StdCodeNullable = learner.StdCodeNullable,
                FworkCodeNullable = learner.FworkCodeNullable,
                PwayCodeNullable = learner.PwayCodeNullable,
                LearnStartDate = learner.LearnStartDate,
                AimType = learner.AimType,
                FundModel = learner.FundModel,
                ULN = learner.ULN,
                CompStatus = learner.CompStatus,
                PrevLearnRefNumber = learner.PrevLearnRefNumber,
                Outcome = learner.Outcome,
                AimSeqNumber = learner.AimSeqNumber,
                LearnPlanEndDate = learner.LearnPlanEndDate,
                PMUKPRN = learner.PMUKPRN,
                PartnerUKPRN = learner.PartnerUKPRN,
                PartnerOrgName = learner.PartnerOrgName,
                PrevUKPRN = learner.PrevUKPRN,
                LearnActEndDate = learner.LearnActEndDate,
                OtherFundAdj = learner.OtherFundAdj,
                PriorLearnFundAdj = learner.PriorLearnFundAdj,
                SWSupAimId = learner.SWSupAimId,
                LearningDeliveryFAMs = MapData(learner.LearningDeliveryFAMs),
                ProvSpecDeliveryMonitorings = MapData(learner.ProvSpecDeliveryMonitorings),
                ProviderSpecLearnerMonitorings = MapData(learner.ProviderSpecLearnerMonitorings)
            };
        }

        private IReadOnlyCollection<LearningDeliveryFAM> MapData(
            IEnumerable<ReferenceDataService.Model.FRM.LearningDeliveryFAM> deliveryFams)
        {
            return deliveryFams?.Select(MapLearningDeliveryFAM).ToList();
        }

        private LearningDeliveryFAM MapLearningDeliveryFAM(
            ReferenceDataService.Model.FRM.LearningDeliveryFAM deliveryFam)
        {
            return new LearningDeliveryFAM
            {
                LearnDelFAMCode = deliveryFam.LearnDelFAMCode,
                LearnDelFAMType = deliveryFam.LearnDelFAMType,
                LearnDelFAMDateFrom = deliveryFam.LearnDelFAMDateFrom,
                LearnDelFAMDateTo = deliveryFam.LearnDelFAMDateTo
            };
        }

        private IReadOnlyCollection<ProviderSpecLearnerMonitoring> MapData(
            IEnumerable<ReferenceDataService.Model.FRM.ProviderSpecLearnerMonitoring> learnerMonitorings)
        {
            return learnerMonitorings?.Select(MapLearnerMonitoring).ToList();
        }

        private ProviderSpecLearnerMonitoring MapLearnerMonitoring(
            ReferenceDataService.Model.FRM.ProviderSpecLearnerMonitoring learnerMonitoring)
        {
            return new ProviderSpecLearnerMonitoring
            {
                ProvSpecLearnMon = learnerMonitoring.ProvSpecLearnMon,
                ProvSpecLearnMonOccur = learnerMonitoring.ProvSpecLearnMonOccur
            };
        }

        private IReadOnlyCollection<ProviderSpecDeliveryMonitoring> MapData(
            IEnumerable<ReferenceDataService.Model.FRM.ProviderSpecDeliveryMonitoring> deliveryMonitorings)
        {
            return deliveryMonitorings?.Select(MapDeliveryMonitoring).ToList();
        }

        private ProviderSpecDeliveryMonitoring MapDeliveryMonitoring(
            ReferenceDataService.Model.FRM.ProviderSpecDeliveryMonitoring deliveryMonitoring)
        {
            return new ProviderSpecDeliveryMonitoring
            {
                ProvSpecDelMon = deliveryMonitoring.ProvSpecDelMon,
                ProvSpecDelMonOccur = deliveryMonitoring.ProvSpecDelMonOccur
            };
        }
    }
}
