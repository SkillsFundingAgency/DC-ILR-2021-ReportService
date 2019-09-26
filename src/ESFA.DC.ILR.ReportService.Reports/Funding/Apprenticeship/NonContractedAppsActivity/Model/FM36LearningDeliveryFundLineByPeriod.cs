using System;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity.Model
{
    public struct FM36LearningDeliveryFundLineByPeriod
    {
        public FM36LearningDeliveryFundLineByPeriod(
            string learnRefNumber,
            int aimSeqNumber,
            int period,
            string conRefNumber,
            DateTime? appAdjLearnStartDate,
            int? ageAtProgStart,
            bool? learnDelMathEng,
            string fundLineType
            )
        {
            LearnRefNumber = learnRefNumber;
            AimSeqNumber = aimSeqNumber;
            Period = period;
            ConRefNumber = conRefNumber;
            AppAdjLearnStartDate = appAdjLearnStartDate;
            AgeAtProgStart = ageAtProgStart;
            LearnDelMathEng = learnDelMathEng;
            FundLineType = fundLineType;
        }

        public string LearnRefNumber { get; set; }

        public int AimSeqNumber { get; set; }

        public string ConRefNumber { get; set; }

        public int Period { get; set; }

        public DateTime? AppAdjLearnStartDate { get; set; }

        public int? AgeAtProgStart { get; set; }

        public bool? LearnDelMathEng { get; set; }

        public string FundLineType { get; set; }
    }
}
