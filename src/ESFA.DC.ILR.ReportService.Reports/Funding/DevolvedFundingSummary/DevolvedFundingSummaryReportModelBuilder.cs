using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedFundingSummary
{
    public class DevolvedFundingSummaryReportModelBuilder : IModelBuilder<DevolvedAdultEducationFundingSummaryReportModel>
    {
        private readonly IPeriodisedValuesLookupProvider _periodisedValuesLookupProvider;

        private readonly IEnumerable<FundModels> _fundModels = new[]
        {
            FundModels.FM35
        };

        public DevolvedFundingSummaryReportModelBuilder(IPeriodisedValuesLookupProvider periodisedValuesLookupProvider)
        {
            _periodisedValuesLookupProvider = periodisedValuesLookupProvider;
        }

        public DevolvedAdultEducationFundingSummaryReportModel Build(IReportServiceContext reportServiceContext,
            IReportServiceDependentData reportServiceDependentData)
        {
            var periodisedValues = _periodisedValuesLookupProvider.Provide(_fundModels, reportServiceDependentData);

            var reportCurrentPeriod = reportServiceContext.ReturnPeriod > 12 ? 12 : reportServiceContext.ReturnPeriod;

            return new DevolvedAdultEducationFundingSummaryReportModel(
                new List<IDevolvedAdultEducationFundingArea>
                {
                    new DevolvedAdultEducationFundingArea
                    (
                        new List<IDevolvedAdultEducationFundingCategory>
                        {
                            new DevolvedAdultEducationFundingCategory
                                (@"Adult Education Budget - Eligible for MCA/GLA funding (non-procured)", reportCurrentPeriod,
                                new List<IDevolvedAdultEducationFundLineGroup>
                                {
                                    BuildIlrFm35FundLineGroup(reportCurrentPeriod, new [] { FundLineConstants.AdultEducationEligibleMCAGLANonProcured }, periodisedValues)
                                } ),
                            new DevolvedAdultEducationFundingCategory
                                (@"Adult Education Budget - Eligible for MCA/GLA funding (procured)", reportCurrentPeriod,
                                new List<IDevolvedAdultEducationFundLineGroup>
                                {
                                    BuildIlrFm35FundLineGroup(reportCurrentPeriod, new [] { FundLineConstants.AdultEducationEligibleMCAGLAProcured }, periodisedValues)
                                })
                        }
                    )
                });
        }

        private IDevolvedAdultEducationFundLineGroup BuildIlrFm35FundLineGroup(int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new DevolvedAdultEducationFundLineGroup($"ILR Total Adult Education (£)", currentPeriod, FundModels.FM35, fundLines, periodisedValues)
                .WithFundLine($"ILR Programme Funding (£)", new[] { AttributeConstants.Fm35OnProgPayment, AttributeConstants.Fm35AchievePayment, AttributeConstants.Fm35EmpOutcomePay, AttributeConstants.Fm35BalancePayment })
                .WithFundLine($"ILR Learning Support (£)", new[] { AttributeConstants.Fm35LearnSuppFundCash });
        }
    }
}
