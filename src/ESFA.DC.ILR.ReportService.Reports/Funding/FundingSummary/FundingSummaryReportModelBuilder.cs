using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Reports.Constants;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary
{
    public class FundingSummaryReportModelBuilder : IModelBuilder<FundingSummaryReportModel>
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

        public FundingSummaryReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var periodisedValues = _periodisedValuesLookupProvider.Provide(_fundModels, reportServiceDependentData);

            var currentPeriod = reportServiceContext.ReturnPeriod;

            return new FundingSummaryReportModel(
                new List<IFundingCategory>()
                {
                    new FundingCategory(
                        @"Carry-in Apprenticeships Budget (for starts before 1 May 2017 and non-procured delivery)",
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory(
                                @"16-18 Apprenticeship Frameworks for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildIlrFm35ApprenticeshipsFundLineGroup("16-18", currentPeriod, new [] { FundLineConstants.Apprenticeship1618 }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"16-18 Trailblazer Apprenticeships for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildTrailblazerApprenticeshipsFundLineGroup("16-18", currentPeriod, new [] { FundLineConstants.TrailblazerApprenticeship1618 }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"16-18 Non-Levy Contracted Apprenticeships - Non-procured delivery",
                                new List<IFundLineGroup>()
                                {
                                    BuildNonLevyApprenticeshipsFundLineGroup("16-18", currentPeriod, new [] { FundLineConstants.NonLevyApprenticeship1618, FundLineConstants.NonLevyApprenticeship1618NonProcured }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"19-23 Apprenticeship Frameworks for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildIlrFm35ApprenticeshipsFundLineGroup("19-23", currentPeriod, new [] { FundLineConstants.Apprenticeship1923 }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"19-23 Trailblazer Apprenticeships for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildTrailblazerApprenticeshipsFundLineGroup("19-23", currentPeriod, new [] { FundLineConstants.TrailblazerApprenticeship1923 }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"24+ Apprenticeship Frameworks for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildIlrFm35ApprenticeshipsFundLineGroup("24+", currentPeriod, new [] { FundLineConstants.Apprenticeship24Plus }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"24+ Trailblazer Apprenticeships for starts before 1 May 2017",
                                new List<IFundLineGroup>()
                                {
                                    BuildTrailblazerApprenticeshipsFundLineGroup("24+", currentPeriod, new [] { FundLineConstants.TrailblazerApprenticeship24Plus }, periodisedValues)
                                }),
                            new FundingSubCategory(
                                @"Adult Non-Levy Contracted Apprenticeships - Non-procured delivery",
                                new List<IFundLineGroup>()
                                {
                                    BuildNonLevyApprenticeshipsFundLineGroup("Adult", currentPeriod, new [] { FundLineConstants.NonLevyApprenticeship19Plus, FundLineConstants.NonLevyApprenticeship19PlusNonProcured }, periodisedValues)
                                })

                        })
                });
        }

        private IFundLineGroup BuildIlrFm35ApprenticeshipsFundLineGroup(string ageRange, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total {ageRange} Apprenticeship Frameworks (£)", currentPeriod, FundModels.FM35, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} Apprenticeship Frameworks Programme Funding (£)", new[] { AttributeConstants.Fm35OnProgPayment, AttributeConstants.Fm35AchievePayment, AttributeConstants.Fm35EmpOutcomePay, AttributeConstants.Fm35BalancePayment })
                        .WithFundLine($"ILR {ageRange} Apprenticeship Frameworks Learning Support (£)", new[] {AttributeConstants.Fm35LearnSuppFundCash});
        }

        private IFundLineGroup BuildTrailblazerApprenticeshipsFundLineGroup(string ageRange, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total {ageRange} Trailblazer Apprenticeships (£)", currentPeriod, FundModels.FM81, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} Trailblazer Apprenticeships Programme Funding (Core Government Contribution, Maths and English) (£)", new [] { AttributeConstants.Fm81CoreGovContPayment, AttributeConstants.Fm81MathEngBalPayment, AttributeConstants.Fm81MathEngOnProgPayment })
                        .WithFundLine($"ILR {ageRange} Trailblazer Apprenticeships Employer Incentive Payments (Achievement, Small Employer, 16-18) (£)", new []{ AttributeConstants.Fm81AchPayment, AttributeConstants.Fm81SmallBusPayment, AttributeConstants.Fm81YoungAppPayment })
                        .WithFundLine($"ILR {ageRange} Trailblazer Apprenticeships Learning Support (£)", new [] { AttributeConstants.Fm81LearnSuppFundCash });
        }
             
        private IFundLineGroup BuildNonLevyApprenticeshipsFundLineGroup(string ageRange, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total {ageRange} Non-Levy Contracted Apprenticeships (£)", currentPeriod, FundModels.FM36, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Programme Aim Indicative Earnings (£)", new[] { AttributeConstants.Fm36ProgrammeAimOnProgPayment, AttributeConstants.Fm36ProgrammeAimBalPayment, AttributeConstants.Fm36ProgrammeAimCompletionPayment })
                        .WithFundLine($"...of which Indicative Government Co-Investment Earnings (£)", new[] { AttributeConstants.Fm36ProgrammeAimProgFundIndMinCoInvest})
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Maths & English Programme Funding (£)", new[]{ AttributeConstants.Fm36MathEngOnProgPayment, AttributeConstants.Fm36MathEngBalPayment })
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Framework Uplift (£)", new[] { AttributeConstants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftOnProgPayment })
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Disadvantage Payments (£)", new[] { AttributeConstants.Fm36DisadvFirstPayment, AttributeConstants.Fm36DisadvSecondPayment })
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Additional Payments for Providers(£)", new[] { AttributeConstants.Fm36LearnDelFirstProv1618Pay, AttributeConstants.Fm36LearnDelSecondProv1618Pay })
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Additional Payments for Employers(£)", new[] { AttributeConstants.Fm36LearnDelFirstEmp1618Pay, AttributeConstants.Fm36LearnDelSecondEmp1618Pay })
                        .WithFundLine($"ILR {ageRange} Non-Levy Contracted Apprenticeships Learning Support(£)", new[] {AttributeConstants.Fm36LearnSuppFundCash });
        }
    }
}
