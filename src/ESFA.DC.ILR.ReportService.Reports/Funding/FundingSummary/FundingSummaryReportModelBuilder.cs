using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary
{
    public class FundingSummaryReportModelBuilder
    {
        private readonly IPeriodisedValuesLookupProvider _periodisedValuesLookupProvider;

        private readonly IEnumerable<FundModels> _fundModels = new[]
        {
            FundModels.FM35,
        };

        public FundingSummaryReportModelBuilder(IPeriodisedValuesLookupProvider periodisedValuesLookupProvider )
        {
            _periodisedValuesLookupProvider = periodisedValuesLookupProvider;
        }

        public FundingSummaryReportModel BuildModel(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var periodisedValues = _periodisedValuesLookupProvider.Provide(_fundModels, reportServiceDependentData);

            var currentPeriod = reportServiceContext.ReturnPeriod;

            return new FundingSummaryReportModel(
                new List<IFundingCategory>()
                {
                    new FundingCategory(
                        @"Carry-in Apprenticeships Budget (for starts before 1 May 2017 and non-procured delivery)",
                        @"Total Carry-in Apprenticeships Budget (for starts before 1 May 2017 and non-procured delivery) (£)",
                        @"Total Carry-in Apprenticeships Budget (for starts before 1 May 2017 and non-procured delivery) Cumulative (£)",
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory(
                                @"16-18 Apprenticeship Frameworks for starts before 1 May 2017",
                                @"Total 16-18 Apprenticeship Frameworks for starts before 1 May 2017 (£)",
                                new List<IFundLineGroup>()
                                {
                                    new FundLineGroup(
                                        @"ILR Total 16-18 Apprenticeship Frameworks (£)",
                                        new List<IFundLine>()
                                        {
                                        })
                                })
                        })
                });
        }




        public IFundLine BuildFundLine(
            string title,
            int currentPeriod,
            FundModels fundModel,
            IEnumerable<string> fundLines,
            IEnumerable<string> attributes,
            IPeriodisedValuesLookup periodisedValues)
        {

            var periodisedValuesList = periodisedValues.GetPeriodisedValues(fundModel, fundLines, attributes);

            if (periodisedValuesList != null)
            {
                return new FundLine(
                    currentPeriod,
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

            return BuildEmptyFundLine(title, currentPeriod);
        }

        private IFundLine BuildEmptyFundLine(string title, int currentPeriod)
        {
            return new FundLine(currentPeriod, title, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }
    }
}
