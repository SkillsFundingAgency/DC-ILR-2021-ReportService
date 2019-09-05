using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive.Model
{
    public class TrailblazerLearningDeliveryPeriodisedValues
    {
        public Dictionary<string, int?> EmployerIds { get; set; }

        public string AttributeName { get; set; }

        public decimal?[] Values { get; set; }
    }
}
