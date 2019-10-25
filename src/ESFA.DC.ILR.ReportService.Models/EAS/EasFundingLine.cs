using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.EAS
{
    public class EasFundingLine
    {
        public string FundLine { get; set; }

        public IReadOnlyCollection<EasSubmissionValue> EasSubmissionValues { get; set; }
    }
}
