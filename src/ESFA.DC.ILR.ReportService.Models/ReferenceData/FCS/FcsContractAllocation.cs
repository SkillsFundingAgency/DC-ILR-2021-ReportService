using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.FCS
{
    public class FcsContractAllocation
    {
        public string ContractAllocationNumber { get; set; }

        public int DeliveryUKPRN { get; set; }

        public Decimal? LearningRatePremiumFactor { get; set; }

        public string TenderSpecReference { get; set; }

        public string LotReference { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? StopNewStartsFromDate { get; set; }

        public List<FcsContractDeliverable> FCSContractDeliverables { get; set; }

        public EsfEligibilityRule EsfEligibilityRule { get; set; }
    }
}
