using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.EAS;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Models.Fm99;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim
{
    public class AdultFundingClaimReportModelBuilder : AbstractReportModelBuilder, IModelBuilder<AdultFundingClaimReportModel>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        
        private const int MidYearMonths = 6;
        private const int YearEndMonths = 10;
        private const int FinalMonths = 12;
        private string[] EasProgrammeFundingFundlines => new[] { FundLineConstants.EasAebAdultSkillsNonProcured };
        private string[] EasProgrammeFundingAttributes => new[] { AttributeConstants.EasAuthorisedClaims, AttributeConstants.EasPrincesTrust };
        private string[] ProgrammeFundingFundlines => new[] { FundLineConstants.AebOtherLearningNonProcured };
        private string[] EasProgrammeFunding1924Fundlines => new[] { FundLineConstants.EasTraineeships1924NonProcured };
        private string[] EasProgrammeFunding1924Attributes => new[] { AttributeConstants.EasAuthorisedClaims };
        private string[] ProgrammeFunding1924Fundlines => new[] { FundLineConstants.Traineeship1924NonProcured };
        private string[] EasLearningSupportFundlines => new[] { FundLineConstants.EasAebAdultSkillsNonProcured };
        private string[] EasLearningSupportAttributes => new[] { AttributeConstants.EasExcessLearningSupport };
        private string[] LearningSupportFundlines => new[] { FundLineConstants.AebOtherLearningNonProcured };
        private string[] LearningSupportAttributes => new[] { AttributeConstants.Fm35LearnSuppFundCash };
        private string[] EasLearningSupport1924Fundlines => new[] { FundLineConstants.EasTraineeships1924NonProcured };
        private static string[] LearningSupport1924Fundlines => new[] { FundLineConstants.Traineeship1924NonProcured };
        private string[] EasAlbAreaCostsAttributes => new[] { AttributeConstants.EasAuthorisedClaims };
        private string[] AlbFundline => new[] { FundLineConstants.AdvancedLearnerLoansBursary };
        private string[] AlbAreaCostAttributes => new[] { AttributeConstants.Fm99AreaUpliftBalPayment, AttributeConstants.Fm99AreaUpliftOnProgPayment };

        private string[] ProgrammeFundingAttributes => new[]
        {
            AttributeConstants.Fm35OnProgPayment,
            AttributeConstants.Fm35BalancePayment,
            AttributeConstants.Fm35EmpOutcomePay,
            AttributeConstants.Fm35AchievePayment,
        };

        public AdultFundingClaimReportModelBuilder(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }
        public AdultFundingClaimReportModel Build(IReportServiceContext reportServiceContext,
            IReportServiceDependentData reportServiceDependentData)
        {
            var fm35Global = reportServiceDependentData.Get<FM35Global>();
            var albGlobal = reportServiceDependentData.Get<ALBGlobal>();
            var referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();
            var easFundingLines = reportServiceDependentData.Get<List<EasFundingLine>>();

            string organisationName = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.Name ?? string.Empty;
            var model = new AdultFundingClaimReportModel();
            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            // Header
            model.ProviderName = organisationName;
            model.Ukprn = reportServiceContext.Ukprn.ToString();
            model.IlrFile = ExtractFileName(reportServiceContext.IlrReportingFilename);
            model.LastIlrFileUpdate = reportServiceContext.LastIlrFileUpdate;
            model.EasFile = reportServiceContext.EasReportingFilename;
            model.LastEasFileUpdate = reportServiceContext.LastEasFileUpdate;
            model.Year = ReportingConstants.Year;

            //Body
            var fm35LearningDeliveryPeriodisedValues = GetFM35LearningDeliveryPeriodisedValues(fm35Global);
            var albLearningDeliveryPeriodisedValues = GetAlbLearningDeliveryPeriodisedValues(albGlobal);

            model.AEBProgrammeFunding = new ActualEarnings()
            {
                MidYearClaims = CalculateAEBClaims(MidYearMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, ProgrammeFundingAttributes, ProgrammeFundingFundlines, EasProgrammeFundingAttributes, EasProgrammeFundingFundlines),
                YearEndClaims = CalculateAEBClaims(YearEndMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, ProgrammeFundingAttributes, ProgrammeFundingFundlines, EasProgrammeFundingAttributes, EasProgrammeFundingFundlines),
                FinalClaims = CalculateAEBClaims(FinalMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, ProgrammeFundingAttributes, ProgrammeFundingFundlines, EasProgrammeFundingAttributes, EasProgrammeFundingFundlines),
            };

            model.AEBLearningSupport = new ActualEarnings()
            {
                MidYearClaims = CalculateAEBClaims(MidYearMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, LearningSupportAttributes, LearningSupportFundlines, EasLearningSupportAttributes, EasLearningSupportFundlines),
                YearEndClaims = CalculateAEBClaims(YearEndMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, LearningSupportAttributes, LearningSupportFundlines, EasLearningSupportAttributes, EasLearningSupportFundlines),
                FinalClaims = CalculateAEBClaims(FinalMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, LearningSupportAttributes, LearningSupportFundlines, EasLearningSupportAttributes, EasLearningSupportFundlines)
            };

            model.AEBProgrammeFunding1924 = new ActualEarnings()
            {
                MidYearClaims = CalculateAEBClaims(MidYearMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, ProgrammeFundingAttributes, ProgrammeFunding1924Fundlines, EasProgrammeFunding1924Attributes, EasProgrammeFunding1924Fundlines),
                YearEndClaims = CalculateAEBClaims(YearEndMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, ProgrammeFundingAttributes, ProgrammeFunding1924Fundlines, EasProgrammeFunding1924Attributes, EasProgrammeFunding1924Fundlines),
                FinalClaims = CalculateAEBClaims(FinalMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, ProgrammeFundingAttributes, ProgrammeFunding1924Fundlines, EasProgrammeFunding1924Attributes, EasProgrammeFunding1924Fundlines),
            };

            model.AEBLearningSupport1924 = new ActualEarnings
            {
                MidYearClaims = CalculateAEBClaims(MidYearMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, LearningSupportAttributes, LearningSupport1924Fundlines, EasLearningSupportAttributes, EasLearningSupport1924Fundlines),
                YearEndClaims = CalculateAEBClaims(YearEndMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, LearningSupportAttributes, LearningSupport1924Fundlines, EasLearningSupportAttributes, EasLearningSupport1924Fundlines),
                FinalClaims = CalculateAEBClaims(FinalMonths, fm35LearningDeliveryPeriodisedValues, easFundingLines, LearningSupportAttributes, LearningSupport1924Fundlines, EasLearningSupportAttributes, EasLearningSupport1924Fundlines)
            };
            
            model.ALBBursaryFunding = new ActualEarnings()
            {
                MidYearClaims = AlbDeliveryValues(MidYearMonths, albLearningDeliveryPeriodisedValues, new[] { AttributeConstants.Fm99AlbSupportPayment }, AlbFundline),
                YearEndClaims = AlbDeliveryValues(YearEndMonths, albLearningDeliveryPeriodisedValues, new[] { AttributeConstants.Fm99AlbSupportPayment }, AlbFundline),
                FinalClaims = AlbDeliveryValues(FinalMonths, albLearningDeliveryPeriodisedValues, new[] { AttributeConstants.Fm99AlbSupportPayment }, AlbFundline)
            };

            model.ALBAreaCosts = new ActualEarnings()
            {
                MidYearClaims = CalculateALBClaims(MidYearMonths, albLearningDeliveryPeriodisedValues, easFundingLines, AlbAreaCostAttributes, AlbFundline, EasAlbAreaCostsAttributes, AlbFundline),
                YearEndClaims = CalculateALBClaims(YearEndMonths, albLearningDeliveryPeriodisedValues, easFundingLines, AlbAreaCostAttributes, AlbFundline, EasAlbAreaCostsAttributes, AlbFundline),
                FinalClaims = CalculateALBClaims(FinalMonths, albLearningDeliveryPeriodisedValues, easFundingLines, AlbAreaCostAttributes, AlbFundline, EasAlbAreaCostsAttributes, AlbFundline)
            };

            model.ALBExcessSupport = new ActualEarnings()
            {
                MidYearClaims = EasValues(MidYearMonths, easFundingLines, new[] { AttributeConstants.EasAllbExcessSupport }, AlbFundline),
                YearEndClaims = EasValues(YearEndMonths, easFundingLines, new[] { AttributeConstants.EasAllbExcessSupport }, AlbFundline),
                FinalClaims = EasValues(FinalMonths, easFundingLines, new[] { AttributeConstants.EasAllbExcessSupport }, AlbFundline)
            };

            // Footer
            model.ReportGeneratedAt = "Report generated at: " + FormatReportGeneratedAtDateTime(dateTimeNowUk);
            model.ApplicationVersion = reportServiceContext.ServiceReleaseVersion;
            model.LarsData = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            model.OrganisationData = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            model.PostcodeData = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;
            model.CampusIdData = referenceDataRoot.MetaDatas.ReferenceDataVersions.CampusIdentifierVersion.Version;

            return model;
        }
        
        private decimal CalculateAEBClaims(int months, List<FM35LearningDeliveryValues> fm35LearningDeliveryPeriodisedValues,
                                            List<EasFundingLine> easFundingLines,
                                            string[] attributes,
                                            string[] fundlines,
                                            string[] easAttributes,
                                            string[] easFundlines)
        {
            return Fm35DeliveryValues(months, fm35LearningDeliveryPeriodisedValues, attributes, fundlines)
                   + EasValues(months, easFundingLines, easAttributes, easFundlines);
        }

        private decimal CalculateALBClaims(int months, List<ALBLearningDeliveryValues> albLearningDeliveryPeriodisedValues,
            List<EasFundingLine> easFundingLines,
            string[] attributes,
            string[] fundlines,
            string[] easAttributes,
            string[] easFundlines)
        {
            return AlbDeliveryValues(months, albLearningDeliveryPeriodisedValues, attributes, fundlines)
                   + EasValues(months, easFundingLines, easAttributes, easFundlines);
        }

        private static List<FM35LearningDeliveryValues> GetFM35LearningDeliveryPeriodisedValues(FM35Global fm35Global)
        {
            var result = new List<FM35LearningDeliveryValues>();
            if (fm35Global?.Learners == null)
            {
                return result;
            }

            foreach (var learner in fm35Global.Learners?.Where(x => x.LearningDeliveries != null))
            {
                foreach (var ld in learner.LearningDeliveries.Where(x => x.LearningDeliveryPeriodisedValues != null))
                {
                    foreach (var ldpv in ld.LearningDeliveryPeriodisedValues)
                    {
                        result.Add(new FM35LearningDeliveryValues
                        {
                            FundLine = ld.LearningDeliveryValue?.FundLine,
                            AttributeName = ldpv.AttributeName,
                            Periods = new[]
                            {
                                ldpv.Period1,
                                ldpv.Period2,
                                ldpv.Period3,
                                ldpv.Period4,
                                ldpv.Period5,
                                ldpv.Period6,
                                ldpv.Period7,
                                ldpv.Period8,
                                ldpv.Period9,
                                ldpv.Period10,
                                ldpv.Period11,
                                ldpv.Period12
                            }
                        });
                    }
                }
            }

            return result;
        }

        private static List<ALBLearningDeliveryValues> GetAlbLearningDeliveryPeriodisedValues(ALBGlobal albGlobal)
        {
            var result = new List<ALBLearningDeliveryValues>();
            if (albGlobal?.Learners == null)
            {
                return result;
            }

            foreach (var learner in albGlobal.Learners?.Where(x => x.LearningDeliveries != null))
            {
                foreach (var ld in learner.LearningDeliveries.Where(x => x.LearningDeliveryPeriodisedValues != null))
                {
                    foreach (var ldpv in ld.LearningDeliveryPeriodisedValues)
                    {
                        result.Add(new ALBLearningDeliveryValues
                        {
                            FundLine = ld.LearningDeliveryValue.FundLine,
                            AttributeName = ldpv.AttributeName,
                            Periods = new[]
                            {
                                ldpv.Period1,
                                ldpv.Period2,
                                ldpv.Period3,
                                ldpv.Period4,
                                ldpv.Period5,
                                ldpv.Period6,
                                ldpv.Period7,
                                ldpv.Period8,
                                ldpv.Period9,
                                ldpv.Period10,
                                ldpv.Period11,
                                ldpv.Period12
                            }
                        });
                    }
                }
            }

            return result;
        }

        private decimal AlbDeliveryValues(
            int forMonths,
            List<ALBLearningDeliveryValues> albLearningDeliveryValues,
            string[] attributes,
            string[] fundLines)
        {
            decimal value = 0;
            var deliveryValues = albLearningDeliveryValues.Where(x =>
                attributes.Any(a => a.CaseInsensitiveEquals(x.AttributeName))
                && fundLines.Any(f => f.CaseInsensitiveEquals(x.FundLine))).ToList();

            foreach (var deliveryValue in deliveryValues)
            {
                for (int period = 0; period < forMonths; period++)
                {
                    value = value + deliveryValue.Periods[period].GetValueOrDefault();
                }
            }

            return value;
        }

        private decimal Fm35DeliveryValues(
            int forMonths,
            List<FM35LearningDeliveryValues> fm35LearningDeliveryPeriodisedValues,
            string[] attributes,
            string[] fundLines)
        {
            decimal value = 0;
            var fm35LearningDeliveryValues = fm35LearningDeliveryPeriodisedValues.Where(x =>
                attributes.Any(a => a.CaseInsensitiveEquals(x.AttributeName))
                && fundLines.Any(f => f.CaseInsensitiveEquals(x.FundLine))).ToList();

            foreach (var deliveryValue in fm35LearningDeliveryValues)
            {
                for (int period = 0; period < forMonths; period++)
                {
                    value = value + deliveryValue.Periods[period].GetValueOrDefault();
                }
            }

            return value;
        }

        private decimal EasValues(
            int forMonths,
            List<EasFundingLine> easFundlines,
            string[] attributes,
            string[] fundLines)
        {
            decimal value = 0;

            var easSubmissionValues = easFundlines?.Where(x => fundLines.Any(f => f.CaseInsensitiveEquals(x.FundLine))).SelectMany(y => y.EasSubmissionValues).ToList();
            List<EasSubmissionValue> submissionValues = easSubmissionValues?.Where(y => attributes.Any(a => a.CaseInsensitiveEquals(y.AdjustmentTypeName))).ToList();
            if (submissionValues != null && submissionValues.Any())
            {
                foreach (var submissionValue in submissionValues)
                {
                    value = value +
                            (submissionValue.Period1?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0) +
                            (submissionValue.Period2?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0) +
                            (submissionValue.Period3?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0) +
                            (submissionValue.Period4?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0) +
                            (submissionValue.Period5?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0) +
                            (submissionValue.Period6?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0);

                    if (forMonths >= 10)
                    {
                        value = value +
                                (submissionValue.Period7?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0) +
                                (submissionValue.Period8?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0) +
                                (submissionValue.Period9?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0) +
                                (submissionValue.Period10?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0);
                    }

                    if (forMonths == 12)
                    {
                        value = value +
                                (submissionValue.Period11?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0) +
                                (submissionValue.Period12?.Where(x => x.PaymentValue != null).Sum(x => x.PaymentValue.Value) ?? 0);
                    }
                }
            }

            return value;
        }
    }
}
