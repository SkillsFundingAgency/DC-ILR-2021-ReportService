using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Devolved.Model
{
    public class DevolvedAdultEducationFundingCategory : IDevolvedAdultEducationFundingCategory
    {
        public DevolvedAdultEducationFundingCategory(string fundingCategoryTitle)
        {
            FundingCategoryTitle = fundingCategoryTitle;
        }

        public string FundingCategoryTitle { get; }

        public IList<IDevolvedAdultEducationFundLineGroup> FundLineGroups { get; set; } = new List<IDevolvedAdultEducationFundLineGroup>();

        public string CumulativeFundingCategoryTitle => $"Total {FundingCategoryTitle} Cumulative (£)";

        public string Title => $"Total {FundingCategoryTitle} (£)";

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

        public int CurrentPeriod { get; }

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

        public DevolvedAdultEducationFundingCategory WithFundLineGroup(IDevolvedAdultEducationFundLineGroup fundLineGroup)
        {
            if (fundLineGroup != null)
            {
                FundLineGroups.Add(fundLineGroup);
            }

            return this;
        }
    }
}
