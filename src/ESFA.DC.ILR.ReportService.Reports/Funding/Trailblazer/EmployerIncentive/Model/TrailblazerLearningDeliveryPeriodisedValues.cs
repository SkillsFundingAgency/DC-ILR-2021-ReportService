using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.FundingService.FM81.FundingOutput.Model.Output;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive.Model
{
    public class TrailblazerLearningDeliveryPeriodisedValues
    {
        public Dictionary<string, int?> EmployerIds { get; set; }

        public LearningDeliveryPeriodisedValue Values { get; set; }

        public string AttributeName { get; set; }
        public Dictionary<string, decimal?> ValuesDictionary { get; set; }
    }
}
