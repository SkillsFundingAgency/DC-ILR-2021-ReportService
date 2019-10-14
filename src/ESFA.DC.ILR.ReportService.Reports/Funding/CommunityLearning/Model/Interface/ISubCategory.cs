namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface
{
    public interface ISubCategory
    {
        int TotalLearners { get; set; }

        int TotalStartedInFundingYear { get; set; }

        int TotalEnrolmentsInFundingYear { get; set; }
    }
}