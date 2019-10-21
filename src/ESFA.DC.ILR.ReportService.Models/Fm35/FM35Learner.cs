using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm35
{
    public class FM35Learner
    {
        public string LearnRefNumber { get; set; }

        public List<LearningDelivery> LearningDeliveries { get; set; }
    }
}
