using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model
{
    public class DevolvedAdultEducationFundingArea : IDevolvedAdultEducationFundingArea
    {
        public DevolvedAdultEducationFundingArea(List<IDevolvedAdultEducationFundingCategory> fundingCategories)
        {
            FundingCategories = fundingCategories ?? new List<IDevolvedAdultEducationFundingCategory>();
        }

        public List<IDevolvedAdultEducationFundingCategory> FundingCategories { get; }
    }
}
