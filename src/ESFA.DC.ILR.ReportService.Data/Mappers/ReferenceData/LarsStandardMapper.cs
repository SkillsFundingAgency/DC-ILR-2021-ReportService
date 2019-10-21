using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class LarsStandardMapper : IMapper<IEnumerable<ReferenceDataService.Model.LARS.LARSStandard>, IReadOnlyCollection<LARSStandard>>
    {
        public IReadOnlyCollection<LARSStandard> MapData(IEnumerable<ReferenceDataService.Model.LARS.LARSStandard> larsStandards)
        {
            return larsStandards?.Select(MapLarsStandard).ToList();
        }

        private LARSStandard MapLarsStandard(ReferenceDataService.Model.LARS.LARSStandard larsStandard)
        {
            return new LARSStandard()
            {
                StandardCode = larsStandard.StandardCode,
                StandardSectorCode = larsStandard.StandardSectorCode,
                NotionalEndLevel = larsStandard.NotionalEndLevel,
                EffectiveFrom = larsStandard.EffectiveFrom,
                EffectiveTo = larsStandard.EffectiveTo,
                LARSStandardApprenticeshipFundings = larsStandard.LARSStandardApprenticeshipFundings?.Select(MapLarsStandardApprenticeshipFunding).ToList(),
                LARSStandardCommonComponents = larsStandard.LARSStandardCommonComponents?.Select(MapStandardCommonComponent).ToList(),
                LARSStandardFundings = larsStandard.LARSStandardFundings?.Select(MapStandardFunding).ToList(),
                LARSStandardValidities = larsStandard.LARSStandardValidities?.Select(MapLarsStandardValidity).ToList(),
            };
        }

        private LARSStandardApprenticeshipFunding MapLarsStandardApprenticeshipFunding(ReferenceDataService.Model.LARS.LARSStandardApprenticeshipFunding larsStandardApprenticeshipFunding)
        {
            return new LARSStandardApprenticeshipFunding()
            {
                ProgType = larsStandardApprenticeshipFunding.ProgType,
                PwayCode = larsStandardApprenticeshipFunding.PwayCode,
                FundingCategory = larsStandardApprenticeshipFunding.FundingCategory,
                BandNumber = larsStandardApprenticeshipFunding.BandNumber,
                CoreGovContributionCap = larsStandardApprenticeshipFunding.CoreGovContributionCap,
                SixteenToEighteenIncentive = larsStandardApprenticeshipFunding.SixteenToEighteenIncentive,
                SixteenToEighteenEmployerAdditionalPayment = larsStandardApprenticeshipFunding.SixteenToEighteenEmployerAdditionalPayment,
                SixteenToEighteenFrameworkUplift = larsStandardApprenticeshipFunding.SixteenToEighteenFrameworkUplift,
                SixteenToEighteenProviderAdditionalPayment = larsStandardApprenticeshipFunding.SixteenToEighteenProviderAdditionalPayment,
                CareLeaverAdditionalPayment = larsStandardApprenticeshipFunding.CareLeaverAdditionalPayment,
                Duration = larsStandardApprenticeshipFunding.Duration,
                ReservedValue2 = larsStandardApprenticeshipFunding.ReservedValue2,
                ReservedValue3 = larsStandardApprenticeshipFunding.ReservedValue3,
                MaxEmployerLevyCap = larsStandardApprenticeshipFunding.MaxEmployerLevyCap,
                FundableWithoutEmployer = larsStandardApprenticeshipFunding.FundableWithoutEmployer,
                EffectiveFrom = larsStandardApprenticeshipFunding.EffectiveFrom,
                EffectiveTo = larsStandardApprenticeshipFunding.EffectiveTo
            };
        }

        private LARSStandardCommonComponent MapStandardCommonComponent(ReferenceDataService.Model.LARS.LARSStandardCommonComponent larsStandardCommonComponent)
        {
            return new LARSStandardCommonComponent()
            {
                CommonComponent = larsStandardCommonComponent.CommonComponent,
                EffectiveFrom = larsStandardCommonComponent.EffectiveFrom,
                EffectiveTo = larsStandardCommonComponent.EffectiveTo
            };
        }

        private LARSStandardFunding MapStandardFunding(ReferenceDataService.Model.LARS.LARSStandardFunding larsStandardFunding)
        {
            return new LARSStandardFunding()
            {
                FundingCategory = larsStandardFunding.FundingCategory,
                BandNumber = larsStandardFunding.BandNumber,
                CoreGovContributionCap = larsStandardFunding.CoreGovContributionCap,
                SixteenToEighteenIncentive = larsStandardFunding.SixteenToEighteenIncentive,
                SmallBusinessIncentive = larsStandardFunding.SmallBusinessIncentive,
                AchievementIncentive = larsStandardFunding.AchievementIncentive,
                FundableWithoutEmployer = larsStandardFunding.FundableWithoutEmployer,
                EffectiveFrom = larsStandardFunding.EffectiveFrom,
                EffectiveTo = larsStandardFunding.EffectiveTo
            };
        }

        private LARSStandardValidity MapLarsStandardValidity(ReferenceDataService.Model.LARS.LARSStandardValidity larsStandardValidity)
        {
            return new LARSStandardValidity()
            {
                ValidityCategory = larsStandardValidity.ValidityCategory,
                LastNewStartDate = larsStandardValidity.LastNewStartDate,
                EffectiveFrom = larsStandardValidity.EffectiveFrom,
                EffectiveTo = larsStandardValidity.EffectiveTo
            };
        }
    }
}
