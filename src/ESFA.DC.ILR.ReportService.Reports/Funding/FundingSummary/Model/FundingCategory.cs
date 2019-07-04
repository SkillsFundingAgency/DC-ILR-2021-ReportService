using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model
{
    public class FundingCategory : IFundingCategory
    {
        private readonly IEnumerable<IFundingSubCategory> _fundingSubCategories;

        public FundingCategory(string title, string fundingCategoryTitle, string cumulativeFundingCategoryTitle, IEnumerable<IFundingSubCategory> fundingSubCategories)
        {
            Title = title;
            FundingCategoryTitle = fundingCategoryTitle;
            CumulativeFundingCategoryTitle = cumulativeFundingCategoryTitle;

            _fundingSubCategories = fundingSubCategories ?? new List<IFundingSubCategory>();
        }

        public string Title { get; }

        public string FundingCategoryTitle { get; }

        public string CumulativeFundingCategoryTitle { get; }

        public decimal Period1 => _fundingSubCategories.Sum(fsc => fsc.Period1);

        public decimal Period2 => _fundingSubCategories.Sum(fsc => fsc.Period2);

        public decimal Period3 => _fundingSubCategories.Sum(fsc => fsc.Period3);

        public decimal Period4 => _fundingSubCategories.Sum(fsc => fsc.Period4);

        public decimal Period5 => _fundingSubCategories.Sum(fsc => fsc.Period5);

        public decimal Period6 => _fundingSubCategories.Sum(fsc => fsc.Period6);

        public decimal Period7 => _fundingSubCategories.Sum(fsc => fsc.Period7);

        public decimal Period8 => _fundingSubCategories.Sum(fsc => fsc.Period8);

        public decimal Period9 => _fundingSubCategories.Sum(fsc => fsc.Period9);

        public decimal Period10 => _fundingSubCategories.Sum(fsc => fsc.Period10);

        public decimal Period11 => _fundingSubCategories.Sum(fsc => fsc.Period11);

        public decimal Period12 => _fundingSubCategories.Sum(fsc => fsc.Period12);

        public decimal Period1To8 => _fundingSubCategories.Sum(fsc => fsc.Period1To8);

        public decimal Period9To12 => _fundingSubCategories.Sum(fsc => fsc.Period9To12);

        public decimal YearToDate => _fundingSubCategories.Sum(fsc => fsc.YearToDate);

        public decimal Total => _fundingSubCategories.Sum(fsc => fsc.Total);

        public decimal CumulativePeriod1 => Period1;

        public decimal CumulativePeriod2 => CumulativePeriod1 + Period2;

        public decimal CumulativePeriod3 => CumulativePeriod2 + Period3;

        public decimal CumulativePeriod4 => CumulativePeriod3 + Period4;

        public decimal CumulativePeriod5 => CumulativePeriod4 + Period5;

        public decimal CumulativePeriod6 => CumulativePeriod5 + Period6;

        public decimal CumulativePeriod7 => CumulativePeriod6 + Period7;

        public decimal CumulativePeriod8 => CumulativePeriod7 + Period8;

        public decimal CumulativePeriod9 => CumulativePeriod8 + Period9;

        public decimal CumulativePeriod10 => CumulativePeriod9 + Period10;

        public decimal CumulativePeriod11 => CumulativePeriod10 + Period11;

        public decimal CumulativePeriod12 => CumulativePeriod11 + Period12;
    }
}
