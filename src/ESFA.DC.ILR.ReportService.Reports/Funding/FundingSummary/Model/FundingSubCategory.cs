using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model
{
    public class FundingSubCategory : IFundingSubCategory
    {
        public FundingSubCategory(string fundingSubCategoryTitle, string title, IList<IFundLineGroup> fundLineGroups)
        {
            FundingSubCategoryTitle = fundingSubCategoryTitle;
            Title = title;

            FundLineGroups = fundLineGroups ?? new List<IFundLineGroup>();
        }

        public IList<IFundLineGroup> FundLineGroups { get; }

        public string FundingSubCategoryTitle { get; }

        public string Title { get; }

        public decimal Period1 => FundLineGroups.Sum(flg => flg.Period1);

        public decimal Period2 => FundLineGroups.Sum(flg => flg.Period2);

        public decimal Period3 => FundLineGroups.Sum(flg => flg.Period3);

        public decimal Period4 => FundLineGroups.Sum(flg => flg.Period4);

        public decimal Period5 => FundLineGroups.Sum(flg => flg.Period5);

        public decimal Period6 => FundLineGroups.Sum(flg => flg.Period6);

        public decimal Period7 => FundLineGroups.Sum(flg => flg.Period7);

        public decimal Period8 => FundLineGroups.Sum(flg => flg.Period8);

        public decimal Period9 => FundLineGroups.Sum(flg => flg.Period9);

        public decimal Period10 => FundLineGroups.Sum(flg => flg.Period10);

        public decimal Period11 => FundLineGroups.Sum(flg => flg.Period11);

        public decimal Period12 => FundLineGroups.Sum(flg => flg.Period12);

        public decimal Period1To8 => FundLineGroups.Sum(flg => flg.Period1To8);

        public decimal Period9To12 => FundLineGroups.Sum(flg => flg.Period9To12);

        public decimal YearToDate => FundLineGroups.Sum(flg => flg.YearToDate);

        public decimal Total => FundLineGroups.Sum(flg => flg.Total);
    }
}
