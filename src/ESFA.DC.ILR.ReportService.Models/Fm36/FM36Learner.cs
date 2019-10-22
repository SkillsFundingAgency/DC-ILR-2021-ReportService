using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.Fm36
{
    public class FM36Learner
    {
        public string LearnRefNumber { get; set; }

        public List<PriceEpisode> PriceEpisodes { get; set; }

        public List<LearningDelivery> LearningDeliveries { get; set; }
    }
}
