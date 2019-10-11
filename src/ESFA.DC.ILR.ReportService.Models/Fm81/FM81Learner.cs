using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm81
{
    public class FM81Learner
    {
        public string LearnRefNumber { get; set; }

        public List<LearningDelivery> LearningDeliveries { get; set; }
    }
}
