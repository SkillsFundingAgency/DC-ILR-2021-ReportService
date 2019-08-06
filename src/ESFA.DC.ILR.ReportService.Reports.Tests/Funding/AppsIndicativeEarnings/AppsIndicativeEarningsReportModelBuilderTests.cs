using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using Moq;
using Xunit;
using LearningDelivery = ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model.Output.LearningDelivery;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.AppsIndicativeEarnings
{
    public class AppsIndicativeEarningsReportModelBuilderTests
    {
        [Fact]
        public void Build_SingleRow()
        {
            var reportServiceContext = Mock.Of<IReportServiceContext>();
            var dependentDataMock = new Mock<IReportServiceDependentData>();

            var adlLearningDeliveryFam = new Mock<ILearningDeliveryFAM>();

            adlLearningDeliveryFam.SetupGet(fam => fam.LearnDelFAMType).Returns("ADL");
            adlLearningDeliveryFam.SetupGet(fam => fam.LearnDelFAMCode).Returns("1");

            var albLearningDeliveryFam = new Mock<ILearningDeliveryFAM>();

            albLearningDeliveryFam.SetupGet(fam => fam.LearnDelFAMType).Returns("ALB");


            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                albLearningDeliveryFam.Object,
                adlLearningDeliveryFam.Object,
            };

            var providerSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
            {
                new TestProviderSpecDeliveryMonitoring(),
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 99,
                LearnAimRef = "learnAimRef",
                AimSeqNumber = 1,
                LearningDeliveryFAMs = learningDeliveryFams,
                ProviderSpecDeliveryMonitorings = providerSpecDeliveryMonitorings
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
                LearnAimRef = "learnAimRef"
            };

            var referenceDataRoot = new ReferenceDataRoot()
            {
                LARSLearningDeliveries = new List<LARSLearningDelivery>()
                {
                    larsLearningDelivery
                }
            };

            var learningDeliveryValue = new LearningDeliveryValue();
            var appsLearningDelivery = new LearningDelivery()
            {
                AimSeqNumber = 1,
                LearningDeliveryValue = learningDeliveryValue,
                LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>()
                {
                    BuildLearningDeliveryPeriodisedValuesForAttribute(AttributeConstants.Fm36MathEngOnProgPayment),
                    BuildLearningDeliveryPeriodisedValuesForAttribute(AttributeConstants.Fm36MathEngBalPayment)
                }
            };

            //var learningDeliveryValue = new PriceEpisodeValues();
            //var appsLearningDelivery = new LearningDelivery()
            //{
            //    AimSeqNumber = 1,
            //    LearningDeliveryValue = learningDeliveryValue,
            //    LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>()
            //    {
            //        BuildLearningDeliveryPeriodisedValuesForAttribute(AttributeConstants.Fm36MathEngOnProgPayment),
            //        BuildLearningDeliveryPeriodisedValuesForAttribute(AttributeConstants.Fm36MathEngBalPayment)
            //    }
            //};

        }

        private LearningDeliveryPeriodisedValue BuildLearningDeliveryPeriodisedValuesForAttribute(string attribute)
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
    }
}
