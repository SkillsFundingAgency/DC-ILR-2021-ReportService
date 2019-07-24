using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model
{
    public class FundLineGroup : IFundLineGroup
    {
        private readonly int _currentPeriod;
        private readonly FundModels _fundModel;
        private readonly IEnumerable<string> _fundLines;
        private readonly IPeriodisedValuesLookup _periodisedValues;

        public FundLineGroup(
            string title,
            int currentPeriod,
            FundModels fundModel,
            IEnumerable<string> fundLines,
            IPeriodisedValuesLookup periodisedValues)
        {
            _currentPeriod = currentPeriod;
            _fundModel = fundModel;
            _fundLines = fundLines;
            _periodisedValues = periodisedValues;
            Title = title;
        }

        public IList<IFundLine> FundLines { get; set; } = new List<IFundLine>();

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

        public FundLineGroup WithFundLine(string title, IEnumerable<string> fundLines, IEnumerable<string> attributes)
        {
            var fundLine = BuildFundLine(title, attributes, fundLines);

            FundLines.Add(fundLine);

            return this;
        }

        public FundLineGroup WithFundLine(string title, IEnumerable<string> attributes)
        {
            var fundLine = BuildFundLine(title, attributes);

            FundLines.Add(fundLine);

            return this;
        }

        public IFundLine BuildFundLine(string title, IEnumerable<string> attributes, IEnumerable<string> fundLines = null)
        {
            var periodisedValuesList = _periodisedValues.GetPeriodisedValues(_fundModel, fundLines ?? _fundLines, attributes);

            if (periodisedValuesList != null)
            {
                return new FundLine(
                    _currentPeriod,
                    title,
                    periodisedValuesList.Where(pv => pv[0].HasValue).Sum(pv => pv[0].Value),
                    periodisedValuesList.Where(pv => pv[1].HasValue).Sum(pv => pv[1].Value),
                    periodisedValuesList.Where(pv => pv[2].HasValue).Sum(pv => pv[2].Value),
                    periodisedValuesList.Where(pv => pv[3].HasValue).Sum(pv => pv[3].Value),
                    periodisedValuesList.Where(pv => pv[4].HasValue).Sum(pv => pv[4].Value),
                    periodisedValuesList.Where(pv => pv[5].HasValue).Sum(pv => pv[5].Value),
                    periodisedValuesList.Where(pv => pv[6].HasValue).Sum(pv => pv[6].Value),
                    periodisedValuesList.Where(pv => pv[7].HasValue).Sum(pv => pv[7].Value),
                    periodisedValuesList.Where(pv => pv[8].HasValue).Sum(pv => pv[8].Value),
                    periodisedValuesList.Where(pv => pv[9].HasValue).Sum(pv => pv[9].Value),
                    periodisedValuesList.Where(pv => pv[10].HasValue).Sum(pv => pv[10].Value),
                    periodisedValuesList.Where(pv => pv[11].HasValue).Sum(pv => pv[11].Value)
                );
            }

            return BuildEmptyFundLine(title);
        }

        private IFundLine BuildEmptyFundLine(string title)
        {
            return new FundLine(_currentPeriod, title, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }
    }
}
