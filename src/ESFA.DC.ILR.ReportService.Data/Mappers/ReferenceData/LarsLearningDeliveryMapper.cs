using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class LarsLearningDeliveryMapper : ILarsLearningDeliveryMapper
    {
        public IReadOnlyCollection<LARSLearningDelivery> MapData(IEnumerable<ReferenceDataService.Model.LARS.LARSLearningDelivery> larsLearningDeliveries)
        {
            return larsLearningDeliveries?.Select(MapLarsLearningDelivery).ToList();
        }

        private LARSLearningDelivery MapLarsLearningDelivery(ReferenceDataService.Model.LARS.LARSLearningDelivery larsLearningDelivery)
        {
            return new LARSLearningDelivery()
            {
                LearnAimRef = larsLearningDelivery.LearnAimRef,
                LearnAimRefTitle = larsLearningDelivery.LearnAimRefTitle,
                LearnAimRefType = larsLearningDelivery.LearnAimRefType,
                LearningDeliveryGenre = larsLearningDelivery.LearningDeliveryGenre,
                NotionalNVQLevel = larsLearningDelivery.NotionalNVQLevel,
                NotionalNVQLevelv2 = larsLearningDelivery.NotionalNVQLevelv2,
                RegulatedCreditValue = larsLearningDelivery.RegulatedCreditValue,
                EnglandFEHEStatus = larsLearningDelivery.EnglandFEHEStatus,
                EnglPrscID = larsLearningDelivery.EnglPrscID,
                FrameworkCommonComponent = larsLearningDelivery.FrameworkCommonComponent,
                LearnDirectClassSystemCode1 = larsLearningDelivery.LearnDirectClassSystemCode1,
                LearnDirectClassSystemCode2 = larsLearningDelivery.LearnDirectClassSystemCode2,
                LearnDirectClassSystemCode3 = larsLearningDelivery.LearnDirectClassSystemCode3,
                SectorSubjectAreaTier1 = larsLearningDelivery.SectorSubjectAreaTier1,
                AwardOrgCode = larsLearningDelivery.AwardOrgCode,
                EFACOFType = larsLearningDelivery.EFACOFType,
                SectorSubjectAreaTier2 = larsLearningDelivery.SectorSubjectAreaTier2,
                LARSAnnualValues = larsLearningDelivery.LARSAnnualValues?.Select(MapLarsAnnualValue).ToList(),
                LARSLearningDeliveryCategories = larsLearningDelivery.LARSLearningDeliveryCategories?.Select(MapLarsLearningDeliveryCategory).ToList(),
                LARSFundings = larsLearningDelivery.LARSFundings?.Select(MapLarsFunding).ToList(),
                LARSFrameworks = larsLearningDelivery.LARSFrameworks?.Select(MapLarsFramework).ToList(),
                LARSValidities = larsLearningDelivery.LARSValidities?.Select(MapLarsValidity).ToList()
            };
        }

        private LARSAnnualValue MapLarsAnnualValue(ReferenceDataService.Model.LARS.LARSAnnualValue larsAnnualValue)
        {
            return new LARSAnnualValue()
            {
                LearnAimRef = larsAnnualValue.LearnAimRef,
                BasicSkills = larsAnnualValue.BasicSkills,
                BasicSkillsType = larsAnnualValue.BasicSkillsType,
                FullLevel2EntitlementCategory = larsAnnualValue.FullLevel2EntitlementCategory,
                FullLevel3EntitlementCategory = larsAnnualValue.FullLevel3EntitlementCategory,
                FullLevel2Percent = larsAnnualValue.FullLevel2Percent,
                FullLevel3Percent = larsAnnualValue.FullLevel3Percent,
                EffectiveFrom = larsAnnualValue.EffectiveFrom,
                EffectiveTo = larsAnnualValue.EffectiveTo
            };
        }

        private LARSLearningDeliveryCategory MapLarsLearningDeliveryCategory(ReferenceDataService.Model.LARS.LARSLearningDeliveryCategory larsLearningDeliveryCategory)
        {
            return new LARSLearningDeliveryCategory()
            {
                LearnAimRef = larsLearningDeliveryCategory.LearnAimRef,
                CategoryRef = larsLearningDeliveryCategory.CategoryRef,
                EffectiveFrom = larsLearningDeliveryCategory.EffectiveFrom,
                EffectiveTo = larsLearningDeliveryCategory.EffectiveTo
            };
        }

        private LARSFunding MapLarsFunding(ReferenceDataService.Model.LARS.LARSFunding larsFunding)
        {
            return new LARSFunding()
            {
                LearnAimRef = larsFunding.LearnAimRef,
                FundingCategory = larsFunding.FundingCategory,
                RateUnWeighted = larsFunding.RateUnWeighted,
                RateWeighted = larsFunding.RateWeighted,
                WeightingFactor = larsFunding.WeightingFactor,
                EffectiveFrom = larsFunding.EffectiveFrom,
                EffectiveTo = larsFunding.EffectiveTo
            };
        }

        private LARSValidity MapLarsValidity(ReferenceDataService.Model.LARS.LARSValidity larsValidity)
        {
            return new LARSValidity()
            {
                LearnAimRef = larsValidity.LearnAimRef,
                ValidityCategory = larsValidity.ValidityCategory,
                LastNewStartDate = larsValidity.LastNewStartDate,
                EffectiveFrom = larsValidity.EffectiveFrom,
                EffectiveTo = larsValidity.EffectiveTo
            };
        }

        private LARSFramework MapLarsFramework(ReferenceDataService.Model.LARS.LARSFramework larsFramework)
        {
            return new LARSFramework()
            {
                FworkCode = larsFramework.FworkCode,
                ProgType = larsFramework.ProgType,
                PwayCode = larsFramework.PwayCode,
                EffectiveFromNullable = larsFramework.EffectiveFromNullable,
                EffectiveTo = larsFramework.EffectiveTo,
                LARSFrameworkCommonComponents = larsFramework.LARSFrameworkCommonComponents?.Select(MapLarsFrameworkCommonComponent).ToList(),
                LARSFrameworkApprenticeshipFundings = larsFramework.LARSFrameworkApprenticeshipFundings?.Select(MapLarsFrameworkApprenticeshipFunding).ToList(),
                LARSFrameworkAim = MapLarsFrameworkAim(larsFramework.LARSFrameworkAim),
            };
        }

        private LARSFrameworkCommonComponent MapLarsFrameworkCommonComponent(ReferenceDataService.Model.LARS.LARSFrameworkCommonComponent larsFrameworkCommonComponent)
        {
            return new LARSFrameworkCommonComponent()
            {
                CommonComponent = larsFrameworkCommonComponent.CommonComponent,
                EffectiveFrom = larsFrameworkCommonComponent.EffectiveFrom,
                EffectiveTo = larsFrameworkCommonComponent.EffectiveTo
            };
        }

        private LARSFrameworkApprenticeshipFunding MapLarsFrameworkApprenticeshipFunding(ReferenceDataService.Model.LARS.LARSFrameworkApprenticeshipFunding larsFrameworkApprenticeshipFunding)
        {
            return new LARSFrameworkApprenticeshipFunding()
            {
                FundingCategory = larsFrameworkApprenticeshipFunding.FundingCategory,
                BandNumber = larsFrameworkApprenticeshipFunding.BandNumber,
                CoreGovContributionCap = larsFrameworkApprenticeshipFunding.CoreGovContributionCap,
                SixteenToEighteenIncentive = larsFrameworkApprenticeshipFunding.SixteenToEighteenIncentive,
                SixteenToEighteenEmployerAdditionalPayment = larsFrameworkApprenticeshipFunding.SixteenToEighteenEmployerAdditionalPayment,
                SixteenToEighteenFrameworkUplift = larsFrameworkApprenticeshipFunding.SixteenToEighteenFrameworkUplift,
                SixteenToEighteenProviderAdditionalPayment = larsFrameworkApprenticeshipFunding.SixteenToEighteenProviderAdditionalPayment,
                CareLeaverAdditionalPayment = larsFrameworkApprenticeshipFunding.CareLeaverAdditionalPayment,
                Duration = larsFrameworkApprenticeshipFunding.Duration,
                ReservedValue2 = larsFrameworkApprenticeshipFunding.ReservedValue2,
                ReservedValue3 = larsFrameworkApprenticeshipFunding.ReservedValue3,
                MaxEmployerLevyCap = larsFrameworkApprenticeshipFunding.MaxEmployerLevyCap,
                FundableWithoutEmployer = larsFrameworkApprenticeshipFunding.FundableWithoutEmployer,
                EffectiveFrom = larsFrameworkApprenticeshipFunding.EffectiveFrom,
                EffectiveTo = larsFrameworkApprenticeshipFunding.EffectiveTo
            };
        }

        private LARSFrameworkAim MapLarsFrameworkAim(ReferenceDataService.Model.LARS.LARSFrameworkAim larsFrameworkAim)
        {
            return new LARSFrameworkAim()
            {
                LearnAimRef = larsFrameworkAim.LearnAimRef,
                FrameworkComponentType = larsFrameworkAim.FrameworkComponentType,
                EffectiveFrom = larsFrameworkAim.EffectiveFrom,
                EffectiveTo = larsFrameworkAim.EffectiveTo
            };
        }
    }
}
