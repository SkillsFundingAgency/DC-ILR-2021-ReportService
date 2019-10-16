using System;
using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model
{
    public class Category : ICategory
    {
        public ICategory SixteenToEighteen { get; set; }

        public ICategory Adult { get; set; }

        public int TotalLearners
        {
            get => CalculateTotal(c => c.TotalLearners, totalLearners);
            set => totalLearners = value;
        }

        public int TotalStartedInFundingYear
        {
            get => CalculateTotal(c => c.TotalStartedInFundingYear, totalStartedInFundingYear);
            set => totalStartedInFundingYear = value;
        }

        public int TotalEnrolmentsInFundingYear
        {
            get => CalculateTotal(c => c.TotalEnrolmentsInFundingYear, totalEnrolmentsInFundingYear);
            set => totalEnrolmentsInFundingYear = value;
        }

        private int totalLearners;
        private int totalStartedInFundingYear;
        private int totalEnrolmentsInFundingYear;

        private int CalculateTotal(Func<ICategory, int> selector, int defaultValue)
        {
            if (SixteenToEighteen != null && Adult != null)
            {
                return selector(Adult) + selector(SixteenToEighteen);
            }

            return defaultValue;
        }
    }
}
