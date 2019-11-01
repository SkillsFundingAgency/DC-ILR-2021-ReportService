using System;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model
{
    public class CommunityLearningData
    {
        public string LearnerRefNumber { get; set; }
        public int AimSeqNumber { get; set; }
        public DateTime LearnStartDate { get; set; }
        public bool SixteenToEighteen { get; set; }
        public bool Adult { get; set; }
        public bool EarliestStartDate { get; set; }
        public bool EarliestStartDatePersonalAndCommunityDevelopmentLearning { get; set; }
        public bool EarliestStartDateNeighbourhoodLearningInDeprivedCommunities { get; set; }
        public bool EarliestStartDateFamilyEnglishMathsAndLanguage { get; set; }
        public bool EarliestStartDateWiderFamilyLearning { get; set; }
        public bool LearnStartDateIsInYear { get; set; }
        public bool PersonalAndCommunityDevelopmentLearning { get; set; }
        public bool NeighbourhoodLearningInDeprivedCommunities { get; set; }
        public bool FamilyEnglishMathsAndLanguage { get; set; }
        public bool WiderFamilyLearning { get; set; }
    }
}
