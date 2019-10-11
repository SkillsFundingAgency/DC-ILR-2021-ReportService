using System;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.FCS
{
    public class FcsContractDeliverable
    {
        public int? DeliverableCode { get; set; }

        public string DeliverableDescription { get; set; }

        public string ExternalDeliverableCode { get; set; }

        public Decimal? UnitCost { get; set; }

        public int? PlannedVolume { get; set; }

        public Decimal? PlannedValue { get; set; }
    }
}
