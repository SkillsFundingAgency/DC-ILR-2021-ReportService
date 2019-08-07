using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.AppsIndicativeEarnings
{
    public class AppsIndicativeEarningsReportModelBuilderTests
    {
        [Fact]
        public void Build_SingleRow()
        {
            var reportServiceContext = Mock.Of<IReportServiceContext>();
            var dependentDataMock = new Mock<IReportServiceDependentData>();

            var actLearningDeliveryFam = new Mock<ILearningDeliveryFAM>();

            actLearningDeliveryFam.SetupGet(fam => fam.LearnDelFAMType).Returns("ACT");
            actLearningDeliveryFam.SetupGet(fam => fam.LearnDelFAMCode).Returns("1");

            var albLearningDeliveryFam = new Mock<ILearningDeliveryFAM>();

            albLearningDeliveryFam.SetupGet(fam => fam.LearnDelFAMType).Returns("ALB");

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                actLearningDeliveryFam.Object,
                albLearningDeliveryFam.Object,
                
            };

            var providerSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
            {
                new TestProviderSpecDeliveryMonitoring(),
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                LearnAimRef = "learnAimRef",
                AimSeqNumber = 1,
                LearningDeliveryFAMs = learningDeliveryFams,
                ProviderSpecDeliveryMonitorings = providerSpecDeliveryMonitorings,
                StdCodeNullable = 1
            };

            var providerSpecLearnerMonitorings = new List<IProviderSpecLearnerMonitoring>()
            {
                new TestProviderSpecLearnerMonitoring(),
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "LearnRefNumber",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    learningDelivery
                },
                ProviderSpecLearnerMonitorings = providerSpecLearnerMonitorings,
            };

            var message = new TestMessage()
            {
                Learners = new List<ILearner>()
                {
                    learner
                }
            };

            var larsLearningDelivery = new LARSLearningDelivery()
            {
                LearnAimRef = "learnAimRef",
                LearnAimRefTitle = "A",
                NotionalNVQLevel = "B",
                SectorSubjectAreaTier2 = 3
                
            };

            var larsStandard = new LARSStandard()
            {
               StandardCode = 1,
                NotionalEndLevel = "NotionalEndLevel"
            };

            var referenceDataRoot = new ReferenceDataRoot()
            {
                LARSLearningDeliveries = new List<LARSLearningDelivery>()
                {
                    larsLearningDelivery
                },
                LARSStandards = new List<LARSStandard>()
                {
                    larsStandard
                }
            };

            var priceEpisodeValues = new PriceEpisodeValues()
            {
                EpisodeStartDate = new DateTime(2019, 08, 08),
                PriceEpisodeAimSeqNumber = 1
            };

            var priceEpisodes = new PriceEpisode()
            {
                PriceEpisodeValues = priceEpisodeValues,
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>()
                {
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeOnProgPaymentAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm3PriceEpisodeBalancePaymentAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeCompletionPaymentAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeLSFCashAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeFirstDisadvantagePaymentAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeSecondDisadvantagePaymentAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeFirstEmp1618PayAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeSecondEmp1618PayAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeFirstProv1618PayAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeSecondProv1618PayAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeLearnerAdditionalPaymentAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftOnProgPaymentAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftBalancingAttributeName),
                    BuildPriceEpisodePeriodisedValuesForAttribute(AttributeConstants.Fm36PriceEpisodeApplic1618FrameworkUpliftCompletionPaymentAttributeName),
                }
            };

            var learningDeliveryValues = new LearningDeliveryValues();
            var appsLearningDelivery = new LearningDelivery()
            {
                AimSeqNumber = 1,
                LearningDeliveryValues = learningDeliveryValues,
                LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>()
                {
                    BuildLearningDeliveryPeriodisedValuesForAttribute(AttributeConstants.Fm36MathEngOnProgPayment),
                    BuildLearningDeliveryPeriodisedValuesForAttribute(AttributeConstants.Fm36MathEngBalPayment),
                    BuildLearningDeliveryPeriodisedValuesForAttribute(AttributeConstants.Fm36LearnSuppFundCash)
                }
            };

            var fm36Global = new FM36Global()
            {
                Learners = new List<FM36Learner>()
                {
                    new FM36Learner()
                    {
                        LearnRefNumber = "LearnRefNumber",
                        LearningDeliveries = new List<LearningDelivery>()
                        {
                            appsLearningDelivery,
                        },
                        PriceEpisodes = new List<PriceEpisode>()
                        {
                            priceEpisodes
                        }
                    }
                }
            };

            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<FM36Global>()).Returns(fm36Global);

            var providerSpecDeliveryMonitoringModel = new ProviderSpecDeliveryMonitoringModel();
            var providerSpecLearnerMonitoringModel = new ProviderSpecLearnerMonitoringModel();
            var learningDeliveryFamsModel = new LearningDeliveryFAMsModel();

            var ilrModelMapperMock = new Mock<IIlrModelMapper>();

            ilrModelMapperMock.Setup(m => m.MapLearningDeliveryFAMs(learningDeliveryFams)).Returns(learningDeliveryFamsModel);
            ilrModelMapperMock.Setup(m => m.MapProviderSpecDeliveryMonitorings(providerSpecDeliveryMonitorings)).Returns(providerSpecDeliveryMonitoringModel);
            ilrModelMapperMock.Setup(m => m.MapProviderSpecLearnerMonitorings(providerSpecLearnerMonitorings)).Returns(providerSpecLearnerMonitoringModel);

            var result = NewBuilder(ilrModelMapperMock.Object).Build(reportServiceContext, dependentDataMock.Object).ToList();

            result.Should().HaveCount(1);

            var row = result[0];

            row.Learner.Should().Be(learner);
            row.LearningDelivery.Should().Be(learningDelivery);
            row.LarsLearningDelivery.Should().Be(larsLearningDelivery);
            row.Fm36LearningDelivery.Should().Be(learningDeliveryValues);

            row.ProviderSpecDeliveryMonitoring.Should().Be(providerSpecDeliveryMonitoringModel);
            row.ProviderSpecLearnerMonitoring.Should().Be(providerSpecLearnerMonitoringModel);
            row.PeriodisedValues.Should().NotBeNull();
            row.LearningDeliveryFAMs.Should().Be(learningDeliveryFamsModel);

            //var csvService = new CsvService(new FileServiceStub());
            //csvService.WriteAsync<AppsIndicativeEarningsReportModel, AppsIndicativeEarningsReportClassMap>(result, "AppsIndicativeEarningsReport.csv", "", CancellationToken.None).ConfigureAwait(false);
        }

        private AppsIndicativeEarningsReportModelBuilder NewBuilder(IIlrModelMapper ilrModelMapper = null)
        {
            return new AppsIndicativeEarningsReportModelBuilder(ilrModelMapper);
        }

        private LearningDeliveryPeriodisedValues BuildLearningDeliveryPeriodisedValuesForAttribute(string attribute)
        {
            return new LearningDeliveryPeriodisedValues()
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

        private PriceEpisodePeriodisedValues BuildPriceEpisodePeriodisedValuesForAttribute(string attribute)
        {
            return new PriceEpisodePeriodisedValues()
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
    }
}
