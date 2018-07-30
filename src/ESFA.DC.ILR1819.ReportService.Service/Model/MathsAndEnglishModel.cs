﻿namespace ESFA.DC.ILR1819.ReportService.Service.Model
{
    public sealed class MathsAndEnglishModel
    {
        public string FundLine { get; set; }

        public string LearnRefNumber { get; set; }

        public string FamilyName { get; set; }

        public string GivenNames { get; set; }

        public string DateOfBirth { get; set; }

        public string CampId { get; set; }

        public string ConditionOfFundingMaths { get; set; }

        public string ConditionOfFundingEnglish { get; set; }

        public string RateBand { get; set; }

        public string OfficalSensitive { get; }
    }
}
