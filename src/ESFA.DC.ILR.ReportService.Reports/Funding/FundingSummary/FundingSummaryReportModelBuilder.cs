﻿using System;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.FundingSummary
{
    public class FundingSummaryReportModelBuilder : AbstractReportModelBuilder, IModelBuilder<FundingSummaryReportModel>
    {
        private string AdultEducationBudgetNote =
            "Please note that devolved adult education funding for learners who are funded through the Mayoral Combined Authorities or Greater London Authority is not included here.\nPlease refer to the separate Devolved Adult Education Funding Summary Report.";
        private const string reportGeneratedTimeStringFormat = "HH:mm:ss on dd/MM/yyyy";

        private const string ProviderName = "Provider Name:";
        private const string UKPRN = "UKPRN:";
        private const string ILRFile = "ILR File:";
        private const string LastILRFileUpdate = "Last ILR File Update:";
        private const string LastEASUpdate = "Last EAS Update:";
        private const string SecurityClassification = "Security Classification:";

        private const string ApplicationVersion = "Application Version:";
        private const string FilePreparationDate = "File Preparation Date:";
        private const string LARSVersion = "LARS Data:";
        private const string PostcodeVersion = "Postcode Data:";
        private const string OrganisationVersion = "Organisation Data:";
        private const string LargeEmployersVersion = "Large Employers Data:";
        private const string ReportGeneratedAt = "Report Generated at:";

        private readonly IPeriodisedValuesLookupProvider _periodisedValuesLookupProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        protected IEnumerable<FundingDataSources> FundingDataSources { private get; set; }

        public FundingSummaryReportModelBuilder(IPeriodisedValuesLookupProvider periodisedValuesLookupProvider, IDateTimeProvider dateTimeProvider)
        {
            _periodisedValuesLookupProvider = periodisedValuesLookupProvider;
            _dateTimeProvider = dateTimeProvider;

            FundingDataSources = new[]
            {
                Funding.FundingDataSources.FM25,
                Funding.FundingDataSources.FM35,
                Funding.FundingDataSources.FM36,
                Funding.FundingDataSources.FM81,
                Funding.FundingDataSources.FM99,
                Funding.FundingDataSources.EAS,
            };
        }

        public FundingSummaryReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var periodisedValues = _periodisedValuesLookupProvider.Provide(FundingDataSources, reportServiceDependentData);

            var reportCurrentPeriod = reportServiceContext.ReturnPeriod > 12 ? 12 : reportServiceContext.ReturnPeriod;

            return new FundingSummaryReportModel(
                new List<IFundingCategory>()
                {
                    new FundingCategory(
                        @"Carry-in Apprenticeships Budget (for starts before 1 May 2017 and non-procured delivery)", reportCurrentPeriod,
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory(@"16-18 Apprenticeship Frameworks for starts before 1 May 2017", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrFm35FundLineGroup("16-18", "Apprenticeship Frameworks", reportCurrentPeriod, new [] { FundLineConstants.Apprenticeship1618 }, periodisedValues))
                                .WithFundLineGroup(BuildEasFm35FundLineGroup("16-18", "Apprenticeship Frameworks", reportCurrentPeriod, new [] { FundLineConstants.EasApprenticeship1618 }, periodisedValues)),
                            new FundingSubCategory(@"16-18 Trailblazer Apprenticeships for starts before 1 May 2017", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrTrailblazerApprenticeshipsFundLineGroup("16-18", reportCurrentPeriod, new [] { FundLineConstants.TrailblazerApprenticeship1618 }, periodisedValues))
                                .WithFundLineGroup(BuildEasAuthorisedClaimsExcessLearningSupportFundLineGroup("16-18", "Trailblazer Apprenticeships", reportCurrentPeriod, new [] { FundLineConstants.EasTrailblazerApprenticeship1618 }, periodisedValues)),
                            new FundingSubCategory(@"16-18 Non-Levy Contracted Apprenticeships - Non-procured delivery", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrNonLevyApprenticeshipsFundLineGroup("16-18", reportCurrentPeriod, new [] { FundLineConstants.NonLevyApprenticeship1618, FundLineConstants.NonLevyApprenticeship1618NonProcured }, periodisedValues))
                                .WithFundLineGroup(BuildEasNonLevyApprenticeshipsFundLineGroup("16-18", reportCurrentPeriod, new [] { FundLineConstants.NonLevyApprenticeship1618NonProcured }, periodisedValues)),
                            new FundingSubCategory(@"19-23 Apprenticeship Frameworks for starts before 1 May 2017", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrFm35FundLineGroup("19-23", "Apprenticeship Frameworks", reportCurrentPeriod, new [] { FundLineConstants.Apprenticeship1923 }, periodisedValues))
                                .WithFundLineGroup(BuildEasFm35FundLineGroup("19-23", "Apprenticeship Frameworks", reportCurrentPeriod, new [] { FundLineConstants.EasApprenticeship1923 }, periodisedValues)),
                            new FundingSubCategory(@"19-23 Trailblazer Apprenticeships for starts before 1 May 2017", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrTrailblazerApprenticeshipsFundLineGroup("19-23", reportCurrentPeriod, new [] { FundLineConstants.TrailblazerApprenticeship1923 }, periodisedValues))
                                .WithFundLineGroup(BuildEasAuthorisedClaimsExcessLearningSupportFundLineGroup("19-23", "Trailblazer Apprenticeships", reportCurrentPeriod, new [] { FundLineConstants.EasTrailblazerApprenticeship1923 }, periodisedValues)),
                            new FundingSubCategory(@"24+ Apprenticeship Frameworks for starts before 1 May 2017", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrFm35FundLineGroup("24+", "Apprenticeship Frameworks", reportCurrentPeriod, new [] { FundLineConstants.Apprenticeship24Plus }, periodisedValues))
                                .WithFundLineGroup(BuildEasFm35FundLineGroup("24+", "Apprenticeship Frameworks", reportCurrentPeriod, new [] { FundLineConstants.EasApprenticeship24Plus }, periodisedValues)),
                            new FundingSubCategory(@"24+ Trailblazer Apprenticeships for starts before 1 May 2017", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrTrailblazerApprenticeshipsFundLineGroup("24+", reportCurrentPeriod, new [] { FundLineConstants.TrailblazerApprenticeship24Plus }, periodisedValues))
                                .WithFundLineGroup(BuildEasAuthorisedClaimsExcessLearningSupportFundLineGroup("24+", "Trailblazer Apprenticeships", reportCurrentPeriod, new [] { FundLineConstants.EasTrailblazerApprenticeship24Plus }, periodisedValues)),
                            new FundingSubCategory(@"Adult Non-Levy Contracted Apprenticeships - Non-procured delivery", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrNonLevyApprenticeshipsFundLineGroup("Adult", reportCurrentPeriod, new [] { FundLineConstants.NonLevyApprenticeship19Plus, FundLineConstants.NonLevyApprenticeship19PlusNonProcured }, periodisedValues))
                                .WithFundLineGroup(BuildEasNonLevyApprenticeshipsFundLineGroup("Adult", reportCurrentPeriod, new [] { FundLineConstants.NonLevyApprenticeship19PlusNonProcured }, periodisedValues))
                        }),
                    new FundingCategory(
                        "Apprenticeships – Employers on Apprenticeship Service", reportCurrentPeriod,
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("16-18 Apprenticeship (Employer on App Service)", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrLevyApprenticeshipsFundLineGroup("16-18", "Apprenticeship (Employer on App Service)", reportCurrentPeriod, new []{ FundLineConstants.ApprenticeshipEmployerOnAppService1618 }, periodisedValues))
                                .WithFundLineGroup(BuildEasLevyApprenticeshipsFundLineGroup("16-18", "Apprenticeship (Employer on App Service)", reportCurrentPeriod, new[] { FundLineConstants.EasLevyApprenticeship1618, FundLineConstants.EasNonLevyApprenticeshipEmployerOnAppService1618 }, periodisedValues)),
                            new FundingSubCategory("Adult Apprenticeship (Employer on App Service)", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrLevyApprenticeshipsFundLineGroup("Adult", "Apprenticeship (Employer on App Service)", reportCurrentPeriod, new [] { FundLineConstants.ApprenticeshipEmployerOnAppService19Plus }, periodisedValues))
                                .WithFundLineGroup(BuildEasLevyApprenticeshipsFundLineGroup("Adult", "Apprenticeship (Employer on App Service)", reportCurrentPeriod, new [] { FundLineConstants.EasLevyApprenticeship19Plus, FundLineConstants.EasNonLevyApprenticeshipEmployerOnAppService19Plus }, periodisedValues)),
                        }),
                    new FundingCategory(
                        "16-18 Non-Levy Contracted Apprenticeships Budget - Procured delivery", reportCurrentPeriod,
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("16-18 Non-Levy Contracted Apprenticeships", reportCurrentPeriod)
                                .WithFundLineGroup(BuildNonLevyContractedApprenticeshipsFundLineGroup("16-18", "Non-Levy Contracted Apprenticeships", reportCurrentPeriod, new []{ FundLineConstants.NonLevyApprenticeship1618Procured }, periodisedValues))
                                .WithFundLineGroup(BuildEasLevyApprenticeshipsFundLineGroup("16-18", "Non-Levy Contracted Apprenticeships", reportCurrentPeriod, new [] { FundLineConstants.NonLevyApprenticeship1618Procured }, periodisedValues)),
                        }),
                    new FundingCategory(
                        "Adult Non-Levy Contracted Apprenticeships Budget - Procured delivery", reportCurrentPeriod,
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("Adult Non-Levy Contracted Apprenticeships", reportCurrentPeriod)
                                .WithFundLineGroup(BuildNonLevyContractedApprenticeshipsFundLineGroup("Adult", "Non-Levy Contracted Apprenticeships", reportCurrentPeriod, new [] { FundLineConstants.NonLevyApprenticeship19PlusProcured }, periodisedValues))
                                .WithFundLineGroup(BuildEasLevyApprenticeshipsFundLineGroup("Adult", "Non-Levy Contracted Apprenticeships", reportCurrentPeriod, new [] { FundLineConstants.NonLevyApprenticeship19PlusProcured }, periodisedValues)),
                        }),
                    new FundingCategory(
                        "16-18 Traineeships Budget", reportCurrentPeriod,
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("16-18 Traineeships", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrFm25FundLineGroup(reportCurrentPeriod, periodisedValues))
                                .WithFundLineGroup(BuildEasFm25FundLineGroup(reportCurrentPeriod, periodisedValues))
                        }),
                    new FundingCategory(
                        "19-24 Traineeships - Non-procured delivery", reportCurrentPeriod,
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("19-24 Traineeships", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrFm35FundLineGroup("19-24", "Traineeships", reportCurrentPeriod, new [] { FundLineConstants.Traineeship1924, FundLineConstants.Traineeship1924NonProcured }, periodisedValues))
                                .WithFundLineGroup(BuildEasAuthorisedClaimsExcessLearningSupportFundLineGroup("19-24", "Traineeships", reportCurrentPeriod, new [] { FundLineConstants.EasTraineeships1924NonProcured }, periodisedValues)),
                        }),
                    new FundingCategory(
                        "19-24 Traineeships - Procured delivery from 1 Nov 2017", reportCurrentPeriod,
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("19-24 Traineeships", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrFm35FundLineGroup("19-24", "Traineeships", reportCurrentPeriod, new [] { FundLineConstants.Traineeship1924ProcuredFromNov2017 }, periodisedValues))
                                .WithFundLineGroup(BuildEasAuthorisedClaimsExcessLearningSupportFundLineGroup("19-24", "Traineeships", reportCurrentPeriod, new [] { FundLineConstants.EasTraineeships1924ProcuredFromNov2017 }, periodisedValues)),
                        }),
                    new FundingCategory(
                        "ESFA Adult Education Budget – Non-procured delivery", reportCurrentPeriod,
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("ESFA AEB – Adult Skills (non-procured)", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrFm35FundLineGroup("ESFA", "AEB - Adult Skills (non-procured)", reportCurrentPeriod, new [] { FundLineConstants.AebOtherLearningNonProcured }, periodisedValues))
                                .WithFundLineGroup(BuildEasAebFundLineGroup("ESFA", "AEB - Adult Skills (non-procured)", reportCurrentPeriod, new [] { FundLineConstants.EasAebAdultSkillsNonProcured }, periodisedValues))
                        },
                        AdultEducationBudgetNote),
                    new FundingCategory(
                        "ESFA Adult Education Budget – Procured delivery from 1 Nov 2017 ", reportCurrentPeriod,
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("ESFA AEB – Adult Skills (procured from Nov 2017)", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrFm35FundLineGroup("ESFA", "AEB - Adult Skills (procured from Nov 2017)", reportCurrentPeriod, new [] { FundLineConstants.AebOtherLearningProcuredFromNov2017 }, periodisedValues))
                                .WithFundLineGroup(BuildEasAebFundLineGroup("ESFA", "AEB - Adult Skills (procured from Nov 2017)", reportCurrentPeriod, new [] { FundLineConstants.EasAebAdultSkillsProcuredFromNov2017 }, periodisedValues))
                        },
                        AdultEducationBudgetNote),
                    new FundingCategory(
                        "Advanced Loans Bursary Budget", reportCurrentPeriod,
                        new List<IFundingSubCategory>()
                        {
                            new FundingSubCategory("Advanced Loans Bursary", reportCurrentPeriod)
                                .WithFundLineGroup(BuildIlrFm99FundLineGroup(reportCurrentPeriod, periodisedValues))
                                .WithFundLineGroup(BuildEasFm99FundLineGroup(reportCurrentPeriod, periodisedValues))
                        })
                }, BuildSummaryPage(reportServiceContext, reportServiceDependentData));
        }

        public SummaryPageModel BuildSummaryPage(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();

            var organisationName = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.Name ?? string.Empty;
            var orgVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            var larsVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            var employersVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.Employers.Version;
            var postcodesVersion = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;
            var easLastUpdate = referenceDataRoot.MetaDatas.ReferenceDataVersions?.EasUploadDateTime.UploadDateTime.ToString();

            var filePreparationDate = message.HeaderEntity.CollectionDetailsEntity.FilePreparationDate.ToString();

            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            var reportGeneratedAt = dateTimeNowUk.ToString(reportGeneratedTimeStringFormat);

            var headerDataDictionary = new Dictionary<string, string>()
            {
                {ProviderName, organisationName},
                {UKPRN, reportServiceContext.Ukprn.ToString()},
                {ILRFile, reportServiceContext.OriginalFilename},
                {LastILRFileUpdate, ExtractDisplayDateTimeFromFileName(reportServiceContext.OriginalFilename)},
                {LastEASUpdate, easLastUpdate},
                {SecurityClassification, ReportingConstants.OfficialSensitive}
            };

            var footerDataDictionary = new Dictionary<string, string>()
            {
                {ApplicationVersion, reportServiceContext.ServiceReleaseVersion},
                {FilePreparationDate, filePreparationDate},
                {LARSVersion, larsVersion},
                {PostcodeVersion, postcodesVersion},
                {OrganisationVersion, orgVersion},
                {LargeEmployersVersion, employersVersion},
                {ReportGeneratedAt, reportGeneratedAt}
            };

            return new SummaryPageModel()
            {
                HeaderData = headerDataDictionary,
                FooterData = footerDataDictionary
            };
        }

        private IFundLineGroup BuildIlrFm99FundLineGroup(int currentPeriod, IPeriodisedValuesLookup periodisedValues)
        {
            var description = "Advanced Loans Bursary";

            return new FundLineGroup($"ILR Total {description} (£)", currentPeriod, Funding.FundingDataSources.FM99, new [] { FundLineConstants.AdvancedLearnerLoansBursary }, periodisedValues)
                .WithFundLine($"ILR {description} Funding (£)", new [] { AttributeConstants.Fm99AlbSupportPayment })
                .WithFundLine($"ILR {description} Area Costs (£)", new [] { AttributeConstants.Fm99AreaUpliftBalPayment, AttributeConstants.Fm99AreaUpliftOnProgPayment });
        }

        private IFundLineGroup BuildIlrFm25FundLineGroup(int currentPeriod, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total 16-18 Traineeships (£)", currentPeriod, Funding.FundingDataSources.FM25, null, periodisedValues)
                .WithFundLine($"ILR 16-18 Traineeships Programme Funding (£)", new [] { FundLineConstants.TraineeshipsAdultFunded1618 }, new [] { AttributeConstants.Fm25LrnOnProgPay })
                .WithFundLine($"ILR 19-24 Traineeships (16-19 Model) Programme Funding (£)", new [] { FundLineConstants.TraineeshipsAdultFunded19Plus }, new [] { AttributeConstants.Fm25LrnOnProgPay });
        }

        private IFundLineGroup BuildIlrFm35FundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total {ageRange} {description} (£)", currentPeriod, Funding.FundingDataSources.FM35, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} {description} Programme Funding (£)", new[] { AttributeConstants.Fm35OnProgPayment, AttributeConstants.Fm35AchievePayment, AttributeConstants.Fm35EmpOutcomePay, AttributeConstants.Fm35BalancePayment })
                        .WithFundLine($"ILR {ageRange} {description} Learning Support (£)", new[] {AttributeConstants.Fm35LearnSuppFundCash});
        }

        private IFundLineGroup BuildIlrTrailblazerApprenticeshipsFundLineGroup(string ageRange, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            var description = "Trailblazer Apprenticeships";

            return new FundLineGroup($"ILR Total {ageRange} {description} (£)", currentPeriod, Funding.FundingDataSources.FM81, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} {description} Programme Funding (Core Government Contribution, Maths and English) (£)", new [] { AttributeConstants.Fm81CoreGovContPayment, AttributeConstants.Fm81MathEngBalPayment, AttributeConstants.Fm81MathEngOnProgPayment })
                        .WithFundLine($"ILR {ageRange} {description} Employer Incentive Payments (Achievement, Small Employer, 16-18) (£)", new []{ AttributeConstants.Fm81AchPayment, AttributeConstants.Fm81SmallBusPayment, AttributeConstants.Fm81YoungAppPayment })
                        .WithFundLine($"ILR {ageRange} {description} Learning Support (£)", new [] { AttributeConstants.Fm81LearnSuppFundCash });
        }
             
        private IFundLineGroup BuildIlrNonLevyApprenticeshipsFundLineGroup(string ageRange, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            var description = "Non-Levy Contracted Apprenticeships";

            return new FundLineGroup($"ILR Total {ageRange} {description} (£)", currentPeriod, Funding.FundingDataSources.FM36, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} {description} Programme Aim Indicative Earnings (£)", new[] { AttributeConstants.Fm36ProgrammeAimOnProgPayment, AttributeConstants.Fm36ProgrammeAimBalPayment, AttributeConstants.Fm36ProgrammeAimCompletionPayment }, false)
                        .WithFundLine($"...of which Indicative Government Co-Investment Earnings (£)", new[] { AttributeConstants.Fm36ProgrammeAimProgFundIndMinCoInvest})
                        .WithFundLine($"ILR {ageRange} {description} Maths & English Programme Funding (£)", new[]{ AttributeConstants.Fm36MathEngOnProgPayment, AttributeConstants.Fm36MathEngBalPayment })
                        .WithFundLine($"ILR {ageRange} {description} Framework Uplift (£)", new[] { AttributeConstants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftOnProgPayment })
                        .WithFundLine($"ILR {ageRange} {description} Disadvantage Payments (£)", new[] { AttributeConstants.Fm36DisadvFirstPayment, AttributeConstants.Fm36DisadvSecondPayment })
                        .WithFundLine($"ILR {ageRange} {description} Additional Payments for Providers (£)", new[] { AttributeConstants.Fm36LearnDelFirstProv1618Pay, AttributeConstants.Fm36LearnDelSecondProv1618Pay })
                        .WithFundLine($"ILR {ageRange} {description} Additional Payments for Employers (£)", new[] { AttributeConstants.Fm36LearnDelFirstEmp1618Pay, AttributeConstants.Fm36LearnDelSecondEmp1618Pay })
                        .WithFundLine($"ILR {ageRange} {description} Learning Support (£)", new[] {AttributeConstants.Fm36LearnSuppFundCash });
        }

        private IFundLineGroup BuildIlrLevyApprenticeshipsFundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total {ageRange} {description} (£)", currentPeriod, Funding.FundingDataSources.FM36, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} {description} Programme Aim Indicative Earnings (£)", new[] { AttributeConstants.Fm36ProgrammeAimOnProgPayment, AttributeConstants.Fm36ProgrammeAimBalPayment, AttributeConstants.Fm36ProgrammeAimCompletionPayment })
                        .WithFundLine($"ILR {ageRange} {description} Maths & English Programme Funding (£)", new[] { AttributeConstants.Fm36MathEngOnProgPayment, AttributeConstants.Fm36MathEngBalPayment })
                        .WithFundLine($"ILR {ageRange} {description} Framework Uplift (£)", new[] { AttributeConstants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftOnProgPayment })
                        .WithFundLine($"ILR {ageRange} {description} Disadvantage Payments (£)", new[] { AttributeConstants.Fm36DisadvFirstPayment, AttributeConstants.Fm36DisadvSecondPayment })
                        .WithFundLine($"ILR {ageRange} {description} Additional Payments for Providers (£)", new[] { AttributeConstants.Fm36LearnDelFirstProv1618Pay, AttributeConstants.Fm36LearnDelSecondProv1618Pay })
                        .WithFundLine($"ILR {ageRange} {description} Additional Payments for Employers (£)", new[] { AttributeConstants.Fm36LearnDelFirstEmp1618Pay, AttributeConstants.Fm36LearnDelSecondEmp1618Pay })
                        .WithFundLine($"ILR {ageRange} {description} Additional Payments for Apprentices (£)", new[] { AttributeConstants.Fm36LearnDelLearnAddPayment })
                        .WithFundLine($"ILR {ageRange} {description} Learning Support (£)", new[] { AttributeConstants.Fm36LearnSuppFundCash });
        }

        private IFundLineGroup BuildNonLevyContractedApprenticeshipsFundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"ILR Total {ageRange} {description} (£)", currentPeriod, Funding.FundingDataSources.FM36, fundLines, periodisedValues)
                        .WithFundLine($"ILR {ageRange} {description} Programme Aim Indicative Earnings (£)", new[] { AttributeConstants.Fm36ProgrammeAimOnProgPayment, AttributeConstants.Fm36ProgrammeAimBalPayment, AttributeConstants.Fm36ProgrammeAimCompletionPayment }, false)
                        .WithFundLine($"...of which Indicative Government Co-Investment Earnings (£)", new[] { AttributeConstants.Fm36ProgrammeAimProgFundIndMinCoInvest })
                        .WithFundLine($"ILR {ageRange} {description} Maths & English Programme Funding (£)", new[] { AttributeConstants.Fm36MathEngOnProgPayment, AttributeConstants.Fm36MathEngBalPayment })
                        .WithFundLine($"ILR {ageRange} {description} Framework Uplift (£)", new[] { AttributeConstants.Fm36LDApplic1618FrameworkUpliftBalancingPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftCompletionPayment, AttributeConstants.Fm36LDApplic1618FrameworkUpliftOnProgPayment })
                        .WithFundLine($"ILR {ageRange} {description} Disadvantage Payments (£)", new[] { AttributeConstants.Fm36DisadvFirstPayment, AttributeConstants.Fm36DisadvSecondPayment })
                        .WithFundLine($"ILR {ageRange} {description} Additional Payments for Providers (£)", new[] { AttributeConstants.Fm36LearnDelFirstProv1618Pay, AttributeConstants.Fm36LearnDelSecondProv1618Pay })
                        .WithFundLine($"ILR {ageRange} {description} Additional Payments for Employers (£)", new[] { AttributeConstants.Fm36LearnDelFirstEmp1618Pay, AttributeConstants.Fm36LearnDelSecondEmp1618Pay })
                        .WithFundLine($"ILR {ageRange} {description} Additional Payments for Apprentices (£)", new[] { AttributeConstants.Fm36LearnDelLearnAddPayment })
                        .WithFundLine($"ILR {ageRange} {description} Learning Support (£)", new[] { AttributeConstants.Fm36LearnSuppFundCash });
        }

        protected virtual IFundLineGroup BuildEasFm35FundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"EAS Total {ageRange} {description} Earnings Adjustment (£)", currentPeriod, Funding.FundingDataSources.EAS, fundLines, periodisedValues)
                .WithFundLine($"EAS {ageRange} {description} Authorised Claims (£)", new [] { AttributeConstants.EasAuthorisedClaims })
                .WithFundLine($"EAS {ageRange} {description} Excess Learning Support (£)", new[] { AttributeConstants.EasExcessLearningSupport })
                .WithFundLine($"EAS {ageRange} {description} Learner Support (£)", new[] { AttributeConstants.EasLearnerSupport });
        }

        protected virtual IFundLineGroup BuildEasAuthorisedClaimsExcessLearningSupportFundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"EAS Total {ageRange} {description} Earnings Adjustment (£)", currentPeriod, Funding.FundingDataSources.EAS, fundLines, periodisedValues)
                .WithFundLine($"EAS {ageRange} {description} Authorised Claims (£)", new[] { AttributeConstants.EasAuthorisedClaims })
                .WithFundLine($"EAS {ageRange} {description} Excess Learning Support (£)", new[] { AttributeConstants.EasExcessLearningSupport });
        }

        protected virtual IFundLineGroup BuildEasNonLevyApprenticeshipsFundLineGroup(string ageRange, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            var description = "Non-Levy Contracted Apprenticeships";

            return new FundLineGroup($"EAS Total {ageRange} {description} Earnings Adjustment (£)", currentPeriod, Funding.FundingDataSources.EAS, fundLines, periodisedValues)
                .WithFundLine($"EAS {ageRange} {description} Training Authorised Claims (£)", new [] { AttributeConstants.EasAuthorisedClaimsTraining })
                .WithFundLine($"EAS {ageRange} {description} Additional Payments for Providers Authorised Claims (£)", new[] { AttributeConstants.EasAuthorisedClaimsProvider })
                .WithFundLine($"EAS {ageRange} {description} Additional Payments for Employers Authorised Claims (£)", new[] { AttributeConstants.EasAuthorisedClaimsEmployer })
                .WithFundLine($"EAS {ageRange} {description} Excess Learning Support (£)", new [] { AttributeConstants.EasExcessLearningSupport });
        }

        protected virtual IFundLineGroup BuildEasLevyApprenticeshipsFundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundLines, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"EAS Total {ageRange} {description} Earnings Adjustment (£)", currentPeriod, Funding.FundingDataSources.EAS, fundLines, periodisedValues)
                .WithFundLine($"EAS {ageRange} {description} - Training Authorised Claims (£)", new[] { AttributeConstants.EasAuthorisedClaimsTraining })
                .WithFundLine($"EAS {ageRange} {description} - Additional Payments for Providers Authorised Claims (£)", new[] { AttributeConstants.EasAuthorisedClaimsProvider })
                .WithFundLine($"EAS {ageRange} {description} - Additional Payments for Employers Authorised Claims (£)", new[] { AttributeConstants.EasAuthorisedClaimsEmployer })
                .WithFundLine($"EAS {ageRange} {description} - Additional Payments for Apprentices Authorised Claims (£)", new[] { AttributeConstants.EasAuthorisedClaimsApprentice })
                .WithFundLine($"EAS {ageRange} {description} - Excess Learning Support (£)", new[] { AttributeConstants.EasExcessLearningSupport });
        }

        protected virtual IFundLineGroup BuildEasFm25FundLineGroup(int currentPeriod, IPeriodisedValuesLookup periodisedValues)
        {
            var description = "Traineeships";

            return new FundLineGroup($"EAS Total 16-18 {description} Earnings Adjustment (£)", currentPeriod, Funding.FundingDataSources.EAS, new[] { FundLineConstants.Traineeships1618 }, periodisedValues)
                .WithFundLine($"EAS 16-18 {description} Authorised Claims (£)", new[] { AttributeConstants.EasAuthorisedClaims })
                .WithFundLine($"EAS 16-18 {description} Excess Learning Support (£)", new[] { AttributeConstants.EasExcessLearningSupport })
                .WithFundLine($"EAS 16-19 {description} Vulnerable Bursary (£)", new [] { AttributeConstants.EasVulnerableBursary })
                .WithFundLine($"EAS 16-19 {description} Free Meals (£)", new [] { AttributeConstants.EasFreeMeals })
                .WithFundLine($"EAS 16-19 {description} Discretionary Bursary (£)", new[] { AttributeConstants.EasDiscretionaryBursary });
        }

        protected virtual IFundLineGroup BuildEasAebFundLineGroup(string ageRange, string description, int currentPeriod, IEnumerable<string> fundModels, IPeriodisedValuesLookup periodisedValues)
        {
            return new FundLineGroup($"EAS Total {ageRange} {description} Earnings Adjustment (£)", currentPeriod, Funding.FundingDataSources.EAS, fundModels, periodisedValues)
                .WithFundLine($"EAS {ageRange} {description} Authorised Claims (£)", new[] { AttributeConstants.EasAuthorisedClaims })
                .WithFundLine($"EAS {ageRange} {description} Prince's Trust (£)", new[] { AttributeConstants.EasPrincesTrust })
                .WithFundLine($"EAS {ageRange} {description} Excess Learning Support (£)", new[] { AttributeConstants.EasExcessLearningSupport });
        }

        protected virtual IFundLineGroup BuildEasFm99FundLineGroup(int currentPeriod, IPeriodisedValuesLookup periodisedValues)
        {
            var description = "Advanced Loans Bursary";

            return new FundLineGroup($"EAS Total {description} Earnings Adjustment (£)", currentPeriod, Funding.FundingDataSources.EAS, new [] { FundLineConstants.AdvancedLearnerLoansBursary }, periodisedValues)
                .WithFundLine($"EAS {description} Excess Support (£)", new[] { AttributeConstants.EasAllbExcessSupport })
                .WithFundLine($"EAS {description} Authorised Claims (£)", new[] { AttributeConstants.EasAuthorisedClaims });
        }
    }
}
