using ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model
{
    public class Category : ICategory
    {
        public ISubCategory SixteenToEighteen { get; set; }

        public ISubCategory Adult { get; set; }

        public int TotalLearners
        {
            get => SixteenToEighteen != null || Adult != null ? SixteenToEighteen.TotalLearners + Adult.TotalLearners : totalLearners;
            set => totalLearners = value;
        }

        public int TotalStartedInFundingYear
        {
            get => SixteenToEighteen != null || Adult != null ? SixteenToEighteen.TotalStartedInFundingYear + Adult.TotalStartedInFundingYear : totalStartedInFundingYear;
            set => totalStartedInFundingYear = value;
        }

        public int TotalEnrolmentsInFundingYear
        {
            get => SixteenToEighteen != null || Adult != null ? SixteenToEighteen.TotalEnrolmentsInFundingYear + Adult.TotalEnrolmentsInFundingYear : totalEnrolmentsInFundingYear;
            set => totalEnrolmentsInFundingYear = value;
        }

        private int totalLearners;
        private int totalStartedInFundingYear;
        private int totalEnrolmentsInFundingYear;
    }
}
