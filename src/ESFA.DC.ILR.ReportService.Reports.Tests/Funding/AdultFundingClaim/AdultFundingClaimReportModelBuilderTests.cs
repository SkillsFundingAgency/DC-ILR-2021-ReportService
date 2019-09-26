using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData.ReferenceDataVersions;
using ESFA.DC.ILR.ReferenceDataService.Model.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim;
using ESFA.DC.ILR.ReportService.Service.Interface;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReferenceDataService.Model.EAS;
using Xunit;
using LearningDelivery = ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDelivery;
using LearningDeliveryPeriodisedValue = ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue;
using LearningDeliveryValue = ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDeliveryValue;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.AdultFundingClaim
{
    public class AdultFundingClaimReportModelBuilderTests
    {
        [Fact]
        public void BuildTest()
        {
            var dependentDataMock = new Mock<IReportServiceDependentData>();

            var referenceDataRoot = new ReferenceDataRoot()
            {
                Organisations = new List<Organisation>()
                {
                    new Organisation()
                    {
                        UKPRN = 987654321,
                        Name = "Provider XYZ",
                        OrganisationCoFRemovals = new List<OrganisationCoFRemoval>()
                        {
                            new OrganisationCoFRemoval()
                            {
                                EffectiveFrom = new DateTime(2019, 01, 01),
                                CoFRemoval = (decimal)4500.12
                            }
                        },
                    }
                },
                MetaDatas = new MetaData()
                {
                    ReferenceDataVersions = new ReferenceDataVersion()
                    {
                        OrganisationsVersion = new OrganisationsVersion { Version = "1.1.1.1" },
                        Employers = new EmployersVersion { Version = "2.2.2.2" },
                        LarsVersion = new LarsVersion { Version = "3.3.3.3" },
                        PostcodesVersion = new PostcodesVersion { Version = "4.4.4.4" },
                        EasUploadDateTime = new EasUploadDateTime { UploadDateTime = new DateTime(2019, 1, 1, 1, 1, 1) }
                    }
                },
                EasFundingLines = BuildEasFundingLines()
            };

            var fm35Learner = new FM35Learner
            {
                LearnRefNumber = "Learner1",
                LearningDeliveries = BuildFm35LearningDeliveries()
            };

            var fm35Global = new FM35Global()
            {
                Learners = new List<FM35Learner>()
                {
                    fm35Learner
                }
            };

            var albLearner = new ALBLearner()
            {
                LearnRefNumber = "Learner1",
                LearningDeliveries = BuildAlbLearningDeliveries()
            };
            var albGlobal = new ALBGlobal()
            {
                Learners = new List<ALBLearner>()
                {
                    albLearner
                }
            };

            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<FM35Global>()).Returns(fm35Global);
            dependentDataMock.Setup(d => d.Get<ALBGlobal>()).Returns(albGlobal);

            var submissionDateTime = new DateTime(2019, 1, 1, 1, 1, 1);
            var ukDateTime = new DateTime(2020, 1, 1, 1, 1, 1);
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            var reportServiceContextMock = new Mock<IReportServiceContext>();

            reportServiceContextMock.SetupGet(c => c.Ukprn).Returns(987654321);
            reportServiceContextMock.SetupGet(c => c.SubmissionDateTimeUtc).Returns(submissionDateTime);
            reportServiceContextMock.SetupGet(c => c.ServiceReleaseVersion).Returns("11.22.3300.4321");
            reportServiceContextMock.SetupGet(c => c.OriginalFilename).Returns("ILR-12345678-1920-20191005-151322-01.xml");

            dateTimeProvider.Setup(p => p.ConvertUtcToUk(submissionDateTime)).Returns(ukDateTime);
            dateTimeProvider.Setup(p => p.GetNowUtc()).Returns(submissionDateTime);

            var result = NewBuilder(dateTimeProvider.Object).Build(reportServiceContextMock.Object, dependentDataMock.Object);

            result.ProviderName.Should().Be("Provider XYZ");
            result.Ukprn.Should().Be(987654321);
            result.IlrFile.Should().Be("ILR-12345678-1920-20191005-151322-01.xml");
            result.Year.Should().Be("2019/20");
            result.ReportGeneratedAt.Should().Be("Report generated at: 01:01:01 on 01/01/2020");
            result.ApplicationVersion.Should().Be("11.22.3300.4321");
            result.LarsData.Should().Be("3.3.3.3");
            result.OrganisationData.Should().Be("1.1.1.1");
            result.PostcodeData.Should().Be("4.4.4.4");
            result.LastEASFileUpdate.Should().Be("01/01/2019 01:01:01");
            result.LastILRFileUpdate.Should().Be("05/10/2019 15:13:22");

            result.AEBProgrammeFunding.MidYearClaims.Should().Be(135.324m);
            result.AEBProgrammeFunding.YearEndClaims.Should().Be(350.384m);
            result.AEBProgrammeFunding.FinalClaims.Should().Be(489.3132m);

            result.AEBLearningSupport.MidYearClaims.Should().Be(44.331m);
            result.AEBLearningSupport.YearEndClaims.Should().Be(115.096m);
            result.AEBLearningSupport.FinalClaims.Should().Be(161.3283m);

            result.AEBProgrammeFunding1924.MidYearClaims.Should().Be(114.324m);
            result.AEBProgrammeFunding1924.YearEndClaims.Should().Be(295.384m);
            result.AEBProgrammeFunding1924.FinalClaims.Should().Be(411.3132m);

            result.AEBLearningSupport1924.MidYearClaims.Should().Be(44.331m);
            result.AEBLearningSupport1924.YearEndClaims.Should().Be(115.096m);
            result.AEBLearningSupport1924.FinalClaims.Should().Be(161.3283m);

            result.ALBBursaryFunding.MidYearClaims.Should().Be(23.331m);
            result.ALBBursaryFunding.YearEndClaims.Should().Be(60.096m);
            result.ALBBursaryFunding.FinalClaims.Should().Be(83.3283m);

            result.ALBAreaCosts.MidYearClaims.Should().Be(67.662m);
            result.ALBAreaCosts.YearEndClaims.Should().Be(175.192m);
            result.ALBAreaCosts.FinalClaims.Should().Be(244.6566m);

            result.ALBExcessSupport.MidYearClaims.Should().Be(21m);
            result.ALBExcessSupport.YearEndClaims.Should().Be(55m);
            result.ALBExcessSupport.FinalClaims.Should().Be(78m);
        }

        private List<LearningDelivery> BuildFm35LearningDeliveries()
        {
            var LearningDeliveries = new List<LearningDelivery>
            {
                new LearningDelivery
                {
                    AimSeqNumber = 1,
                    LearningDeliveryValue = new LearningDeliveryValue
                    {
                        FundLine = "ESFA AEB - Adult Skills (non-procured)"
                    },
                    LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>
                    {
                        BuildFm35PeriodisedValuesForAttribute("OnProgPayment"),
                        BuildFm35PeriodisedValuesForAttribute("BalancePayment"),
                        BuildFm35PeriodisedValuesForAttribute("EmpOutcomePay"),
                        BuildFm35PeriodisedValuesForAttribute("AchievePayment"),
                        BuildFm35PeriodisedValuesForAttribute("LearnSuppFundCash")
                    }
                },
                new LearningDelivery
                {
                    AimSeqNumber = 2,
                    LearningDeliveryValue = new LearningDeliveryValue
                    {
                        FundLine = "19-24 Traineeship (non-procured)"
                    },
                    LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>
                    {
                        BuildFm35PeriodisedValuesForAttribute("OnProgPayment"),
                        BuildFm35PeriodisedValuesForAttribute("BalancePayment"),
                        BuildFm35PeriodisedValuesForAttribute("EmpOutcomePay"),
                        BuildFm35PeriodisedValuesForAttribute("AchievePayment"),
                        BuildFm35PeriodisedValuesForAttribute("LearnSuppFundCash")
                    }
                }
            };
            return LearningDeliveries;
        }

        private LearningDeliveryPeriodisedValue BuildFm35PeriodisedValuesForAttribute(string attribute)
        {
            return new LearningDeliveryPeriodisedValue()
            {
                AttributeName = attribute,
                Period1 = 1.111m,
                Period2 = 2.222m,
                Period3 = 3.333m,
                Period4 = 4.444m,
                Period5 = 5.555m,
                Period6 = 6.666m,
                Period7 = 7.777m,
                Period8 = 8.888m,
                Period9 = 9.999m,
                Period10 = 10.1010m,
                Period11 = 11.1111m,
                Period12 = 12.1212m
            };
        }

        private List<FundingService.ALB.FundingOutput.Model.Output.LearningDelivery> BuildAlbLearningDeliveries()
        {
            var LearningDeliveries = new List<FundingService.ALB.FundingOutput.Model.Output.LearningDelivery>
            {
                new FundingService.ALB.FundingOutput.Model.Output.LearningDelivery
                {
                    AimSeqNumber = 1,
                    LearningDeliveryValue = new FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryValue
                    {
                        FundLine = "Advanced Learner Loans Bursary"
                    },
                    LearningDeliveryPeriodisedValues = new List<FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue>
                    {
                        BuildAlbPeriodisedValuesForAttribute("ALBSupportPayment"),
                        BuildAlbPeriodisedValuesForAttribute("AreaUpliftBalPayment"),
                        BuildAlbPeriodisedValuesForAttribute("AreaUpliftOnProgPayment")
                    }
                }
            };
            return LearningDeliveries;
        }

        private FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue BuildAlbPeriodisedValuesForAttribute(string attribute)
        {
            return new FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue()
            {
                AttributeName = attribute,
                Period1 = 1.111m,
                Period2 = 2.222m,
                Period3 = 3.333m,
                Period4 = 4.444m,
                Period5 = 5.555m,
                Period6 = 6.666m,
                Period7 = 7.777m,
                Period8 = 8.888m,
                Period9 = 9.999m,
                Period10 = 10.1010m,
                Period11 = 11.1111m,
                Period12 = 12.1212m
            };
        }

        private List<EasFundingLine> BuildEasFundingLines()
        {
            var easFundingLines = new List<EasFundingLine>
            {
                new EasFundingLine
                {
                    FundLine = "ESFA AEB - Adult Skills (non-procured)",
                    EasSubmissionValues = new List<EasSubmissionValue>
                    {
                        BuildEasSubmissionValues("Authorised Claims"),
                        BuildEasSubmissionValues("Princes Trust"),
                        BuildEasSubmissionValues("Excess Learning Support")
                    }
                },
                new EasFundingLine
                {
                    FundLine = "19-24 Traineeships (non-procured)",
                    EasSubmissionValues = new List<EasSubmissionValue>
                    {
                        BuildEasSubmissionValues("Authorised Claims"),
                        BuildEasSubmissionValues("Princes Trust"),
                        BuildEasSubmissionValues("Excess Learning Support")
                    }
                },
                new EasFundingLine
                {
                    FundLine = "Advanced Learner Loans Bursary",
                    EasSubmissionValues = new List<EasSubmissionValue>
                    {
                        BuildEasSubmissionValues("Authorised Claims"),
                        BuildEasSubmissionValues("ALLB Excess Support")
                    }
                }
            };
            return easFundingLines;
        }

        private static EasSubmissionValue BuildEasSubmissionValues(string attributeName)
        {
            return new EasSubmissionValue
            {
                AdjustmentTypeName = attributeName,
                Period1 = new List<EasPaymentValue>{new EasPaymentValue(1m,0)},
                Period2 = new List<EasPaymentValue>{new EasPaymentValue(2m,0)},
                Period3 = new List<EasPaymentValue>{new EasPaymentValue(3m,0)},
                Period4 = new List<EasPaymentValue>{new EasPaymentValue(4m,0)},
                Period5 = new List<EasPaymentValue>{new EasPaymentValue(5m,0)},
                Period6 = new List<EasPaymentValue>{new EasPaymentValue(6m,0)},
                Period7 = new List<EasPaymentValue>{new EasPaymentValue(7m,0)},
                Period8 = new List<EasPaymentValue>{new EasPaymentValue(8m,0)},
                Period9 = new List<EasPaymentValue>{new EasPaymentValue(9m,0)},
                Period10 = new List<EasPaymentValue>{new EasPaymentValue(10m,0)},
                Period11 = new List<EasPaymentValue>{new EasPaymentValue(11m,0)},
                Period12 = new List<EasPaymentValue>{new EasPaymentValue(12m,0)}
            };
        }

        private AdultFundingClaimReportModelBuilder NewBuilder(IDateTimeProvider dateTimeProvider = null)
        {
            return new AdultFundingClaimReportModelBuilder(dateTimeProvider);
        }
    }
}
