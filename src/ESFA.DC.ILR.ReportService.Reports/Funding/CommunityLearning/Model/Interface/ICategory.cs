namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface
{
    public interface ICategory
    {
        ISubCategory SixteenToEighteen { get; set; }

        ISubCategory Adult { get; set; }

        int TotalLearners { get; set; }

        int TotalStartedInFundingYear { get; set; }

        int TotalEnrolmentsInFundingYear { get; set; }
    }
}