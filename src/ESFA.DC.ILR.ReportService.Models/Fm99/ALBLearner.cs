using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm99
{
    public class ALBLearner
    {
        public string LearnRefNumber { get; set; }

        public List<LearningDelivery> LearningDeliveries { get; set; }
    }
}
