using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Interface.Mappers;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.FCS;

namespace ESFA.DC.ILR.ReportService.Data.Mappers.ReferenceData
{
    public class FcsContractAllocationMapper : IMapper<IEnumerable<ReferenceDataService.Model.FCS.FcsContractAllocation>, IReadOnlyCollection<FcsContractAllocation>>
    {
        public IReadOnlyCollection<FcsContractAllocation> MapData(IEnumerable<ReferenceDataService.Model.FCS.FcsContractAllocation> fcsContractAllocations)
        {
            return fcsContractAllocations?.Select(MapFcsContractAllocation).ToList();
        }

        private FcsContractAllocation MapFcsContractAllocation(ReferenceDataService.Model.FCS.FcsContractAllocation fcsContractAllocation)
        {
            return new FcsContractAllocation()
            {
                ContractAllocationNumber = fcsContractAllocation.ContractAllocationNumber,
                DeliveryUKPRN = fcsContractAllocation.DeliveryUKPRN,
                LearningRatePremiumFactor = fcsContractAllocation.LearningRatePremiumFactor,
                TenderSpecReference = fcsContractAllocation.TenderSpecReference,
                LotReference = fcsContractAllocation.LotReference,
                FundingStreamPeriodCode = fcsContractAllocation.FundingStreamPeriodCode,
                StartDate = fcsContractAllocation.StartDate,
                EndDate = fcsContractAllocation.EndDate,
                StopNewStartsFromDate = fcsContractAllocation.StopNewStartsFromDate,
                FCSContractDeliverables = fcsContractAllocation.FCSContractDeliverables?.Select(MapFcsContractDeliverable).ToList(),
                EsfEligibilityRule = MapEsfEligibilityRule(fcsContractAllocation.EsfEligibilityRule)
            };
        }

        private FcsContractDeliverable MapFcsContractDeliverable(ReferenceDataService.Model.FCS.FcsContractDeliverable fcsContractDeliverable)
        {
            return new FcsContractDeliverable()
            {
                DeliverableCode = fcsContractDeliverable.DeliverableCode,
                DeliverableDescription = fcsContractDeliverable.DeliverableDescription,
                ExternalDeliverableCode = fcsContractDeliverable.ExternalDeliverableCode,
                UnitCost = fcsContractDeliverable.UnitCost,
                PlannedVolume = fcsContractDeliverable.PlannedVolume,
                PlannedValue = fcsContractDeliverable.PlannedValue
            };
        }

        private EsfEligibilityRule MapEsfEligibilityRule(ReferenceDataService.Model.FCS.EsfEligibilityRule esfEligibilityRule)
        {
            return new EsfEligibilityRule()
            {
                Benefits = esfEligibilityRule.Benefits,
                CalcMethod = esfEligibilityRule.CalcMethod,
                TenderSpecReference = esfEligibilityRule.TenderSpecReference,
                LotReference = esfEligibilityRule.LotReference,
                MinAge = esfEligibilityRule.MinAge,
                MaxAge = esfEligibilityRule.MaxAge,
                MinLengthOfUnemployment = esfEligibilityRule.MinLengthOfUnemployment,
                MaxLengthOfUnemployment = esfEligibilityRule.MaxLengthOfUnemployment,
                MinPriorAttainment = esfEligibilityRule.MinPriorAttainment,
                MaxPriorAttainment = esfEligibilityRule.MaxPriorAttainment,
                EmploymentStatuses = esfEligibilityRule.EmploymentStatuses?.Select(MapEsfEligibilityRuleEmploymentStatus).ToList(),
                LocalAuthorities = esfEligibilityRule.LocalAuthorities?.Select(MapEsfEligibilityRuleLocalAuthority).ToList(),
                LocalEnterprisePartnerships = esfEligibilityRule.LocalEnterprisePartnerships?.Select(MapEsfEligibilityRuleLocalEnterprisePartnership).ToList(),
                SectorSubjectAreaLevels = esfEligibilityRule.SectorSubjectAreaLevels?.Select(MapEsfEligibilityRuleSectorSubjectAreaLevel).ToList()
            };
        }

        private EsfEligibilityRuleEmploymentStatus MapEsfEligibilityRuleEmploymentStatus(ReferenceDataService.Model.FCS.EsfEligibilityRuleEmploymentStatus eligibilityRuleEmploymentStatus)
        {
            return new EsfEligibilityRuleEmploymentStatus()
            {
                Code = eligibilityRuleEmploymentStatus.Code
            };
        }

        private EsfEligibilityRuleLocalAuthority MapEsfEligibilityRuleLocalAuthority(ReferenceDataService.Model.FCS.EsfEligibilityRuleLocalAuthority eligibilityRuleLocalAuthority)
        {
            return new EsfEligibilityRuleLocalAuthority()
            {
                Code = eligibilityRuleLocalAuthority.Code
            };
        }

        private EsfEligibilityRuleLocalEnterprisePartnership MapEsfEligibilityRuleLocalEnterprisePartnership(ReferenceDataService.Model.FCS.EsfEligibilityRuleLocalEnterprisePartnership eligibilityRuleLocalEnterprisePartnership)
        {
            return new EsfEligibilityRuleLocalEnterprisePartnership()
            {
                Code = eligibilityRuleLocalEnterprisePartnership.Code
            };
        }

        private EsfEligibilityRuleSectorSubjectAreaLevel MapEsfEligibilityRuleSectorSubjectAreaLevel(ReferenceDataService.Model.FCS.EsfEligibilityRuleSectorSubjectAreaLevel eligibilityRuleSectorSubjectAreaLevel)
        {
            return new EsfEligibilityRuleSectorSubjectAreaLevel()
            {
                SectorSubjectAreaCode = eligibilityRuleSectorSubjectAreaLevel.SectorSubjectAreaCode,
                MaxLevelCode = eligibilityRuleSectorSubjectAreaLevel.MaxLevelCode,
                MinLevelCode = eligibilityRuleSectorSubjectAreaLevel.MinLevelCode
            };
        }
    }
}
