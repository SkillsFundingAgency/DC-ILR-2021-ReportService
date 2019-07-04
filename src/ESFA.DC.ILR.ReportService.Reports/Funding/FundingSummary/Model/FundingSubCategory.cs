using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model
{
    public class FundingSubCategory : IFundingSubCategory
    {
        private readonly IEnumerable<IFundLineGroup> _fundLineGroups;

        public FundingSubCategory(string fundingSubCategoryTitle, string title, IEnumerable<IFundLineGroup> fundLineGroups)
        {
            FundingSubCategoryTitle = fundingSubCategoryTitle;
            Title = title;

            _fundLineGroups = fundLineGroups ?? new List<IFundLineGroup>();
        }

        public string FundingSubCategoryTitle { get; }

        public string Title { get; }

        public decimal Period1 => _fundLineGroups.Sum(flg => flg.Period1);

        public decimal Period2 => _fundLineGroups.Sum(flg => flg.Period2);

        public decimal Period3 => _fundLineGroups.Sum(flg => flg.Period3);

        public decimal Period4 => _fundLineGroups.Sum(flg => flg.Period4);

        public decimal Period5 => _fundLineGroups.Sum(flg => flg.Period5);

        public decimal Period6 => _fundLineGroups.Sum(flg => flg.Period6);

        public decimal Period7 => _fundLineGroups.Sum(flg => flg.Period7);

        public decimal Period8 => _fundLineGroups.Sum(flg => flg.Period8);

        public decimal Period9 => _fundLineGroups.Sum(flg => flg.Period9);

        public decimal Period10 => _fundLineGroups.Sum(flg => flg.Period10);

        public decimal Period11 => _fundLineGroups.Sum(flg => flg.Period11);

        public decimal Period12 => _fundLineGroups.Sum(flg => flg.Period12);

        public decimal Period1To8 => _fundLineGroups.Sum(flg => flg.Period1To8);

        public decimal Period9To12 => _fundLineGroups.Sum(flg => flg.Period9To12);

        public decimal YearToDate => _fundLineGroups.Sum(flg => flg.YearToDate);

        public decimal Total => _fundLineGroups.Sum(flg => flg.Total);
    }
}
