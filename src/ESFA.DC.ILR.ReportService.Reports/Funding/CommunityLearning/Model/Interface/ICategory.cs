﻿namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface
{
    public interface ICategory
    {
        ICategory SixteenToEighteen { get; set; }

        ICategory Adult { get; set; }

        int TotalLearners { get; set; }

        int TotalStartedInFundingYear { get; set; }

        int TotalEnrolmentsInFundingYear { get; set; }
    }
}