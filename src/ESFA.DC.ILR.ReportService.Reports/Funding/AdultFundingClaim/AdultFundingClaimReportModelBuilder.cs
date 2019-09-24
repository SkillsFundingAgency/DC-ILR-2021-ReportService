using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.EAS;
using ESFA.DC.ILR.ReportService.Reports.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim
{
    public class AdultFundingClaimReportModelBuilder : AbstractReportModelBuilder, IModelBuilder<AdultFundingClaimReportModel>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPeriodisedValuesLookupProvider _periodisedValuesLookupProvider;
        private const string ReportGeneratedTimeStringFormat = "HH:mm:ss on dd/MM/yyyy";

        private const int MidYearMonths = 6;
        private const int YearEndMonths = 10;
        private const int FinalMonths = 12;

        private int[] First6MonthsArray => new[] { 1, 2, 3, 4, 5, 6 };

        private int[] First10MonthsArray => new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        private int[] First12MonthsArray => new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        public AdultFundingClaimReportModelBuilder(IDateTimeProvider dateTimeProvider, IPeriodisedValuesLookupProvider periodisedValuesLookupProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _periodisedValuesLookupProvider = periodisedValuesLookupProvider;
        }
        public AdultFundingClaimReportModel Build(IReportServiceContext reportServiceContext,
            IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm35Global = reportServiceDependentData.Get<FM35Global>();
            var albGlobal = reportServiceDependentData.Get<ALBGlobal>();

            //_periodisedValuesLookupProvider.Provide()BuildFm35Dictionary(FM35Global fm35Global)

            //var periodisedValues = _periodisedValuesLookupProvider.Provide(FundingDataSources, reportServiceDependentData);

            var referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();

        

            string organisationName = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.Name ?? string.Empty;
            var learners = message?.Learners ?? Enumerable.Empty<ILearner>();
            var model = new AdultFundingClaimReportModel();
            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            var reportGeneratedAt = "Report generated at: " + dateTimeNowUk.ToString(ReportGeneratedTimeStringFormat);


            //Body
            var fm35LearningDeliveryPeriodisedValues = GetFM35LearningDeliveryPeriodisedValues(fm35Global);
            var albLearningDeliveryPeriodisedValues = GetAlbLearningDeliveryPeriodisedValues(albGlobal);
            var easFundingLines = referenceDataRoot?.EasFundingLines;

            model.AEBProgrammeFunding = new ActualEarnings()
            {
                MidYearClaims = Fm35DeliveryValues(MidYearMonths, fm35LearningDeliveryPeriodisedValues,
                    new[]
                    {
                        AttributeConstants.Fm35OnProgPayment,
                        AttributeConstants.Fm35BalancePayment,
                        AttributeConstants.Fm35EmpOutcomePay,
                        AttributeConstants.Fm35AchievePayment,
                    }, 
                    new[]
                    {
                        FundLineConstants.AebOtherLearningNonProcured
                    }) 
                    + EasValues(MidYearMonths, easFundingLines, new[]
                    {
                        AttributeConstants.EasAuthorisedClaims, AttributeConstants.EasPrincesTrust

                    }, FundLineConstants.EasAebAdultSkillsNonProcured),

                YearEndClaims = Fm35DeliveryValues(YearEndMonths, fm35LearningDeliveryPeriodisedValues,
                            new[]
                            {
                                AttributeConstants.Fm35OnProgPayment,
                                AttributeConstants.Fm35BalancePayment,
                                AttributeConstants.Fm35EmpOutcomePay,
                                AttributeConstants.Fm35AchievePayment,
                            },
                            new[]
                            {
                                FundLineConstants.AebOtherLearningNonProcured
                            }) 
                            + EasValues(YearEndMonths, easFundingLines, new[]
                                        {
                                            AttributeConstants.EasAuthorisedClaims, AttributeConstants.EasPrincesTrust

                                        }, FundLineConstants.EasAebAdultSkillsNonProcured),
                FinalClaims = Fm35DeliveryValues(FinalMonths, fm35LearningDeliveryPeriodisedValues,
                            new[]
                            {
                                AttributeConstants.Fm35OnProgPayment,
                                AttributeConstants.Fm35BalancePayment,
                                AttributeConstants.Fm35EmpOutcomePay,
                                AttributeConstants.Fm35AchievePayment,
                            },
                            new[]
                            {
                                FundLineConstants.AebOtherLearningNonProcured
                            })
                            + EasValues(FinalMonths, easFundingLines, new[]
                                    {
                                        AttributeConstants.EasAuthorisedClaims, AttributeConstants.EasPrincesTrust

                                    }, FundLineConstants.EasAebAdultSkillsNonProcured),
            };

            model.AEBLearningSupport = new ActualEarnings()
            {
                MidYearClaims = Fm35DeliveryValues(MidYearMonths, fm35LearningDeliveryPeriodisedValues,
                                    new[]
                                    {
                                        AttributeConstants.Fm35LearnSuppFundCash
                                    },
                                    new[]
                                    {
                                        FundLineConstants.AebOtherLearningNonProcured
                                    })
                                + EasValues(MidYearMonths, easFundingLines, new[]
                                {
                                    AttributeConstants.EasExcessLearningSupport

                                }, FundLineConstants.EasAebAdultSkillsNonProcured),

                YearEndClaims = Fm35DeliveryValues(YearEndMonths, fm35LearningDeliveryPeriodisedValues,
                                    new[]
                                    {
                                        AttributeConstants.Fm35LearnSuppFundCash
                                    },
                                    new[]
                                    {
                                        FundLineConstants.AebOtherLearningNonProcured
                                    })
                                + EasValues(YearEndMonths, easFundingLines, new[]
                                {
                                    AttributeConstants.EasExcessLearningSupport

                                }, FundLineConstants.EasAebAdultSkillsNonProcured),

                FinalClaims = Fm35DeliveryValues(FinalMonths, fm35LearningDeliveryPeriodisedValues,
                                  new[]
                                  {
                                      AttributeConstants.Fm35LearnSuppFundCash
                                  },
                                  new[]
                                  {
                                      FundLineConstants.AebOtherLearningNonProcured
                                  })
                              + EasValues(FinalMonths, easFundingLines, new[]
                              {
                                  AttributeConstants.EasExcessLearningSupport

                              }, FundLineConstants.EasAebAdultSkillsNonProcured)

            };


            model.AEBProgrammeFunding1924 = new ActualEarnings()
            {
                MidYearClaims = Fm35DeliveryValues(MidYearMonths, fm35LearningDeliveryPeriodisedValues,
                                    new[]
                                    {
                                        AttributeConstants.Fm35OnProgPayment,
                                        AttributeConstants.Fm35BalancePayment,
                                        AttributeConstants.Fm35EmpOutcomePay,
                                        AttributeConstants.Fm35AchievePayment,
                                    },
                                    new[]
                                    {
                                        FundLineConstants.Traineeship1924NonProcured
                                    })
                                + EasValues(MidYearMonths, easFundingLines, new[]
                                {
                                    AttributeConstants.EasAuthorisedClaims

                                }, FundLineConstants.EasTraineeships1924NonProcured),

                YearEndClaims = Fm35DeliveryValues(YearEndMonths, fm35LearningDeliveryPeriodisedValues,
                                    new[]
                                    {
                                        AttributeConstants.Fm35OnProgPayment,
                                        AttributeConstants.Fm35BalancePayment,
                                        AttributeConstants.Fm35EmpOutcomePay,
                                        AttributeConstants.Fm35AchievePayment,
                                    },
                                    new[]
                                    {
                                        FundLineConstants.Traineeship1924NonProcured
                                    })
                                + EasValues(YearEndMonths, easFundingLines, new[]
                                {
                                    AttributeConstants.EasAuthorisedClaims

                                }, FundLineConstants.EasTraineeships1924NonProcured),

                FinalClaims = Fm35DeliveryValues(FinalMonths, fm35LearningDeliveryPeriodisedValues,
                                  new[]
                                  {
                                      AttributeConstants.Fm35OnProgPayment,
                                      AttributeConstants.Fm35BalancePayment,
                                      AttributeConstants.Fm35EmpOutcomePay,
                                      AttributeConstants.Fm35AchievePayment,
                                  },
                                  new[]
                                  {
                                      FundLineConstants.Traineeship1924NonProcured
                                  })
                              + EasValues(FinalMonths, easFundingLines, new[]
                              {
                                  AttributeConstants.EasAuthorisedClaims

                              }, FundLineConstants.EasTraineeships1924NonProcured)

            };

            model.AEBLearningSupport1924 = new ActualEarnings()
            {
                MidYearClaims = Fm35DeliveryValues(MidYearMonths, fm35LearningDeliveryPeriodisedValues,
                                  new[]
                                  {
                                        AttributeConstants.Fm35LearnSuppFundCash
                                  },
                                  new[]
                                  {
                                        FundLineConstants.Traineeship1924NonProcured
                                  })
                              + EasValues(MidYearMonths, easFundingLines, new[]
                              {
                                    AttributeConstants.EasExcessLearningSupport

                              }, FundLineConstants.EasTraineeships1924NonProcured),

                YearEndClaims = Fm35DeliveryValues(YearEndMonths, fm35LearningDeliveryPeriodisedValues,
                                  new[]
                                  {
                                        AttributeConstants.Fm35LearnSuppFundCash
                                  },
                                  new[]
                                  {
                                        FundLineConstants.Traineeship1924NonProcured
                                  })
                              + EasValues(YearEndMonths, easFundingLines, new[]
                              {
                                    AttributeConstants.EasExcessLearningSupport

                              }, FundLineConstants.EasTraineeships1924NonProcured),

                FinalClaims = Fm35DeliveryValues(FinalMonths, fm35LearningDeliveryPeriodisedValues,
                                new[]
                                {
                                      AttributeConstants.Fm35LearnSuppFundCash
                                },
                                new[]
                                {
                                      FundLineConstants.Traineeship1924NonProcured
                                })
                            + EasValues(FinalMonths, easFundingLines, new[]
                            {
                                  AttributeConstants.EasExcessLearningSupport

                            }, FundLineConstants.EasTraineeships1924NonProcured)
            };


            model.ALBBursaryFunding = new ActualEarnings()
            {
                MidYearClaims = AlbDeliveryValues(MidYearMonths, albLearningDeliveryPeriodisedValues, new[] {AttributeConstants.Fm99AlbSupportPayment}, new []{ FundLineConstants.AdvancedLearnerLoansBursary }),
                YearEndClaims = AlbDeliveryValues(YearEndMonths, albLearningDeliveryPeriodisedValues, new[] {AttributeConstants.Fm99AlbSupportPayment}, new []{ FundLineConstants.AdvancedLearnerLoansBursary }),
                FinalClaims = AlbDeliveryValues(FinalMonths, albLearningDeliveryPeriodisedValues, new[] {AttributeConstants.Fm99AlbSupportPayment}, new []{ FundLineConstants.AdvancedLearnerLoansBursary })
            };


            model.ALBAreaCosts = new ActualEarnings()
            {
                MidYearClaims = AlbDeliveryValues(MidYearMonths, albLearningDeliveryPeriodisedValues,
                    new[] {AttributeConstants.Fm99AreaUpliftBalPayment, AttributeConstants.Fm99AreaUpliftOnProgPayment},
                    new[] {FundLineConstants.AdvancedLearnerLoansBursary})
                                + 
                                EasValues(MidYearMonths, easFundingLines, new[]
                                {
                                    AttributeConstants.EasAuthorisedClaims

                                }, FundLineConstants.AdvancedLearnerLoansBursary),
                YearEndClaims = AlbDeliveryValues(YearEndMonths, albLearningDeliveryPeriodisedValues,
                    new[] {AttributeConstants.Fm99AreaUpliftBalPayment, AttributeConstants.Fm99AreaUpliftOnProgPayment},
                    new[] {FundLineConstants.AdvancedLearnerLoansBursary})
                    +
                    EasValues(YearEndMonths, easFundingLines, new[]
                    {
                        AttributeConstants.EasAuthorisedClaims

                    }, FundLineConstants.AdvancedLearnerLoansBursary),
                FinalClaims = AlbDeliveryValues(FinalMonths, albLearningDeliveryPeriodisedValues,
                    new[] {AttributeConstants.Fm99AreaUpliftBalPayment, AttributeConstants.Fm99AreaUpliftOnProgPayment},
                    new[] {FundLineConstants.AdvancedLearnerLoansBursary})
                              +
                              EasValues(FinalMonths, easFundingLines, new[]
                              {
                                  AttributeConstants.EasAuthorisedClaims

                              }, FundLineConstants.AdvancedLearnerLoansBursary),
            };

            model.ALBExcessSupport = new ActualEarnings()
            {
                MidYearClaims = EasValues(MidYearMonths, easFundingLines, new[] {AttributeConstants.EasAllbExcessSupport}, FundLineConstants.AdvancedLearnerLoansBursary),
                YearEndClaims = EasValues(YearEndMonths, easFundingLines, new[] {AttributeConstants.EasAllbExcessSupport}, FundLineConstants.AdvancedLearnerLoansBursary),
                FinalClaims = EasValues(FinalMonths, easFundingLines, new[] {AttributeConstants.EasAllbExcessSupport}, FundLineConstants.AdvancedLearnerLoansBursary)
            };

            // Header
            model.ProviderName = organisationName;
            model.Ukprn = reportServiceContext.Ukprn;
            model.IlrFile = ExtractFileName(reportServiceContext.OriginalFilename);
            model.Year = ReportingConstants.Year;


            return model;
        }

        private static List<FM35LearningDeliveryValues> GetFM35LearningDeliveryPeriodisedValues(FM35Global fm35Global)
        {
            var result = new List<FM35LearningDeliveryValues>();
            if (fm35Global?.Learners == null)
            {
                return result;
            }

            foreach (var learner in fm35Global.Learners)
            {
                if (learner.LearningDeliveries == null)
                {
                    continue;
                }

                foreach (var ld in learner.LearningDeliveries)
                {
                    if (ld.LearningDeliveryPeriodisedValues == null)
                    {
                        continue;
                    }

                    foreach (var ldpv in ld.LearningDeliveryPeriodisedValues)
                    {
                        result.Add(new FM35LearningDeliveryValues
                        {
                            AimSeqNumber = ld.AimSeqNumber ?? 0,
                            LearnRefNumber = learner.LearnRefNumber,
                            AttributeName = ldpv.AttributeName,
                            Period1 = ldpv.Period1,
                            Period2 = ldpv.Period2,
                            Period3 = ldpv.Period3,
                            Period4 = ldpv.Period4,
                            Period5 = ldpv.Period5,
                            Period6 = ldpv.Period6,
                            Period7 = ldpv.Period7,
                            Period8 = ldpv.Period8,
                            Period9 = ldpv.Period9,
                            Period10 = ldpv.Period10,
                            Period11 = ldpv.Period11,
                            Period12 = ldpv.Period12,
                            FundLine = ld.LearningDeliveryValue?.FundLine
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

            foreach (var learner in albGlobal.Learners)
            {
                if (learner.LearningDeliveries == null)
                {
                    continue;
                }

                foreach (var ld in learner.LearningDeliveries)
                {
                    if (ld.LearningDeliveryPeriodisedValues == null)
                    {
                        continue;
                    }

                    foreach (var ldpv in ld.LearningDeliveryPeriodisedValues)
                    {
                        result.Add(new ALBLearningDeliveryValues
                        {
                            AimSeqNumber = ld.AimSeqNumber,
                            LearnRefNumber = learner.LearnRefNumber,
                            AttributeName = ldpv.AttributeName,
                            Period1 = ldpv.Period1,
                            Period2 = ldpv.Period2,
                            Period3 = ldpv.Period3,
                            Period4 = ldpv.Period4,
                            Period5 = ldpv.Period5,
                            Period6 = ldpv.Period6,
                            Period7 = ldpv.Period7,
                            Period8 = ldpv.Period8,
                            Period9 = ldpv.Period9,
                            Period10 = ldpv.Period10,
                            Period11 = ldpv.Period11,
                            Period12 = ldpv.Period12,
                            FundLine = ld.LearningDeliveryValue?.FundLine
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
            var otherLearningProgramFunding = albLearningDeliveryValues.Where(x =>
                attributes.Any(a => a.CaseInsensitiveEquals(x.AttributeName))
                && fundLines.Any(f => f.CaseInsensitiveEquals(x.FundLine))).ToList();

            foreach (var deliveryValues in otherLearningProgramFunding)
            {
                value = value +
                        deliveryValues.Period1.GetValueOrDefault() + deliveryValues.Period2.GetValueOrDefault() + deliveryValues.Period3.GetValueOrDefault() +
                        deliveryValues.Period4.GetValueOrDefault() + deliveryValues.Period5.GetValueOrDefault() + deliveryValues.Period6.GetValueOrDefault();

                if (forMonths >= 10)
                {
                    value = value +
                            deliveryValues.Period7.GetValueOrDefault() + deliveryValues.Period8.GetValueOrDefault() + deliveryValues.Period9.GetValueOrDefault() +
                            deliveryValues.Period10.GetValueOrDefault();
                }

                if (forMonths == 12)
                {
                    value = value +
                            deliveryValues.Period11.GetValueOrDefault() + deliveryValues.Period12.GetValueOrDefault();
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
            var otherLearningProgramFunding = fm35LearningDeliveryPeriodisedValues.Where(x =>
                attributes.Any(a => a.CaseInsensitiveEquals(x.AttributeName))
                && fundLines.Any(f => f.CaseInsensitiveEquals(x.FundLine))).ToList();

            foreach (var deliveryValues in otherLearningProgramFunding)
            {
                value = value +
                        deliveryValues.Period1.GetValueOrDefault() + deliveryValues.Period2.GetValueOrDefault() + deliveryValues.Period3.GetValueOrDefault() +
                        deliveryValues.Period4.GetValueOrDefault() + deliveryValues.Period5.GetValueOrDefault() + deliveryValues.Period6.GetValueOrDefault();

                if (forMonths >= 10)
                {
                    value = value +
                            deliveryValues.Period7.GetValueOrDefault() + deliveryValues.Period8.GetValueOrDefault() + deliveryValues.Period9.GetValueOrDefault() +
                            deliveryValues.Period10.GetValueOrDefault();
                }

                if (forMonths == 12)
                {
                    value = value +
                            deliveryValues.Period11.GetValueOrDefault() + deliveryValues.Period12.GetValueOrDefault();
                }
            }

            return value;
        }


        private decimal EasValues(
            int forMonths,
            IReadOnlyCollection<EasFundingLine> easFundlines,
            string[] attributes,
            string fundLine)
        {
            decimal value = 0;

            var easSubmissionValues = easFundlines.Where(x => x.FundLine.CaseInsensitiveEquals(fundLine)).SelectMany(y => y.EasSubmissionValues).ToList();
            List<EasSubmissionValue> submissionValues = easSubmissionValues.Where(y => attributes.Any(a => a.CaseInsensitiveEquals(y.AdjustmentTypeName))).ToList();

            foreach (var submissionValue in submissionValues)
            {
                value = value +
                        (submissionValue.Period1?.Sum(x => x.PaymentValue.GetValueOrDefault())?? 0) +
                        (submissionValue.Period2?.Sum(x => x.PaymentValue.GetValueOrDefault()) ?? 0) +
                        (submissionValue.Period3?.Sum(x => x.PaymentValue.GetValueOrDefault()) ?? 0) +
                        (submissionValue.Period4?.Sum(x => x.PaymentValue.GetValueOrDefault()) ?? 0) +
                        (submissionValue.Period5?.Sum(x => x.PaymentValue.GetValueOrDefault()) ?? 0) +
                        (submissionValue.Period6?.Sum(x => x.PaymentValue.GetValueOrDefault()) ?? 0);

                if (forMonths >= 10)
                {
                    value = value +
                            (submissionValue.Period7?.Sum(x => x.PaymentValue.GetValueOrDefault()) ?? 0) +
                            (submissionValue.Period8?.Sum(x => x.PaymentValue.GetValueOrDefault()) ?? 0) +
                            (submissionValue.Period9?.Sum(x => x.PaymentValue.GetValueOrDefault()) ?? 0) +
                            (submissionValue.Period10?.Sum(x => x.PaymentValue.GetValueOrDefault()) ?? 0);
                }

                if (forMonths == 12)
                {
                    value = value +
                            (submissionValue.Period11?.Sum(x => x.PaymentValue.GetValueOrDefault()) ?? 0) +
                            (submissionValue.Period12?.Sum(x => x.PaymentValue.GetValueOrDefault()) ?? 0);
                }
            }

            return value;
        }
    }
}
