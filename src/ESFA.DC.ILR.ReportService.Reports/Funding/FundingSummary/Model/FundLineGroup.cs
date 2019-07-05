using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model
{
    public class FundLineGroup : IFundLineGroup
    {
        public FundLineGroup(string title, IList<IFundLine> fundLines)
        {
            Title = title;

            FundLines = fundLines ?? new List<IFundLine>();
        }

        public IList<IFundLine> FundLines { get; }

        public string Title { get; }

        public decimal Period1 => FundLines.Sum(fl => fl.Period1);

        public decimal Period2 => FundLines.Sum(fl => fl.Period2);

        public decimal Period3 => FundLines.Sum(fl => fl.Period3);

        public decimal Period4 => FundLines.Sum(fl => fl.Period4);

        public decimal Period5 => FundLines.Sum(fl => fl.Period5);

        public decimal Period6 => FundLines.Sum(fl => fl.Period6);

        public decimal Period7 => FundLines.Sum(fl => fl.Period7);

        public decimal Period8 => FundLines.Sum(fl => fl.Period8);

        public decimal Period9 => FundLines.Sum(fl => fl.Period9);

        public decimal Period10 => FundLines.Sum(fl => fl.Period10);

        public decimal Period11 => FundLines.Sum(fl => fl.Period11);

        public decimal Period12 => FundLines.Sum(fl => fl.Period12);

        public decimal Period1To8 => FundLines.Sum(fl => fl.Period1To8);

        public decimal Period9To12 => FundLines.Sum(fl => fl.Period9To12);

        public decimal YearToDate => FundLines.Sum(fl => fl.YearToDate);

        public decimal Total => FundLines.Sum(fl => fl.Total);
    }
}
