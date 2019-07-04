using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model
{
    public class FundLineGroup : IFundLineGroup
    {
        private readonly IEnumerable<IFundLine> _fundLines;

        public FundLineGroup(string title, IEnumerable<IFundLine> fundLines)
        {
            Title = title;

            _fundLines = fundLines ?? new List<IFundLine>();
        }

        public string Title { get; }

        public decimal Period1 => _fundLines.Sum(fl => fl.Period1);

        public decimal Period2 => _fundLines.Sum(fl => fl.Period2);

        public decimal Period3 => _fundLines.Sum(fl => fl.Period3);

        public decimal Period4 => _fundLines.Sum(fl => fl.Period4);

        public decimal Period5 => _fundLines.Sum(fl => fl.Period5);

        public decimal Period6 => _fundLines.Sum(fl => fl.Period6);

        public decimal Period7 => _fundLines.Sum(fl => fl.Period7);

        public decimal Period8 => _fundLines.Sum(fl => fl.Period8);

        public decimal Period9 => _fundLines.Sum(fl => fl.Period9);

        public decimal Period10 => _fundLines.Sum(fl => fl.Period10);

        public decimal Period11 => _fundLines.Sum(fl => fl.Period11);

        public decimal Period12 => _fundLines.Sum(fl => fl.Period12);

        public decimal Period1To8 => _fundLines.Sum(fl => fl.Period1To8);

        public decimal Period9To12 => _fundLines.Sum(fl => fl.Period9To12);

        public decimal YearToDate => _fundLines.Sum(fl => fl.YearToDate);

        public decimal Total => _fundLines.Sum(fl => fl.Total);
    }
}
