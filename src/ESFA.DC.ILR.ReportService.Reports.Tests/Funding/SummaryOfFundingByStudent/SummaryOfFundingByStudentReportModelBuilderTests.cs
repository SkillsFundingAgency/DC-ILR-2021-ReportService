using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.MathsAndEnglish;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.SummaryOfFundingByStudent;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.SummaryOfFundingByStudent
{
    public class SummaryOfFundingByStudentReportModelBuilderTests
    {
        [Fact]
        public void FilterStartFund_True()
        {
            NewBuilder().FilterStartFund(true).Should().BeTrue();
        }

        [Fact]
        public void FilterStartFund_False()
        {
            NewBuilder().FilterStartFund(false).Should().BeFalse();
        }

        [Fact]
        public void FilterStartFund_False_Null()
        {
            NewBuilder().FilterStartFund(null).Should().BeFalse();
        }

        [Theory]
        [InlineData("14-16 Direct Funded Students")]
        [InlineData("16-19 Students (excluding High Needs Students)")]
        [InlineData("16-19 High Needs Students")]
        [InlineData("19-24 Students with an EHCP")]
        [InlineData("19+ Continuing Students (excluding EHCP)")]
        public void FilterFundLine(string fundLine)
        {
            NewBuilder().FilterFundLine(fundLine).Should().BeTrue();
        }

        [Fact]
        public void FilterFundLine_Null()
        {
            NewBuilder().FilterFundLine(null).Should().BeFalse();
        }

        [Fact]
        public void FilterFundLine_Mismatch()
        {
            NewBuilder().FilterFundLine("Junk").Should().BeFalse();
        }

        [Fact]
        public void FilterSOF_False_Type()
        {
            var learningDeliveryFamMock = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMock.SetupGet(fam => fam.LearnDelFAMType).Returns("ABC");
            learningDeliveryFamMock.SetupGet(fam => fam.LearnDelFAMCode).Returns("107");

            NewBuilder().FilterSOF(learningDeliveryFamMock.Object).Should().BeFalse();
        }

        [Fact]
        public void FilterSOF_False_Code()
        {
            var learningDeliveryFamMock = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMock.SetupGet(fam => fam.LearnDelFAMType).Returns("SOF");
            learningDeliveryFamMock.SetupGet(fam => fam.LearnDelFAMCode).Returns("105");

            NewBuilder().FilterSOF(learningDeliveryFamMock.Object).Should().BeFalse();
        }

        [Fact]
        public void FilterSOF_True()
        {
            var learningDeliveryFamMock = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFamMock.SetupGet(fam => fam.LearnDelFAMType).Returns("SOF");
            learningDeliveryFamMock.SetupGet(fam => fam.LearnDelFAMCode).Returns("107");

            NewBuilder().FilterSOF(learningDeliveryFamMock.Object).Should().BeTrue();
        }

        [Fact]
        public void Order()
        {
            var one = BuildModelFor("14-16 Direct Funded Students", "A");
            var two = BuildModelFor("16-19 Students (excluding High Needs Students)", "A");
            var three = BuildModelFor("16-19 High Needs Students", "A");
            var four = BuildModelFor("19-24 Students with an EHCP", "A");
            var five = BuildModelFor("19-24 Students with an EHCP", "B");
            var six = BuildModelFor("19+ Continuing Students (excluding EHCP)", "A");
            var seven = BuildModelFor("Anything else that should not be in the list really but will go to the end of the report", "A");

            var models = new List<AbstractSixteenToNineteenModel>()
            {
                four,
                one,
                six,
                seven,
                two,
                three,
                five,
            };

            var ordered = NewBuilder().Order(models).ToList();

            ordered.Should().ContainInOrder(one, two, three, four, five, six, seven);
        }

        [Fact]
        public void Build_One()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "LearnRefNumber",
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "SOF",
                                LearnDelFAMCode = "107",
                            }
                        }
                    }
                }
            };

            var fm25Learner = new FM25Learner()
            {
                LearnRefNumber = "LearnRefNumber",
                StartFund = true,
                FundLine = "14-16 Direct Funded Students",
            };

            var message = new TestMessage()
            {
                Learners = new List<ILearner>()
                {
                    learner
                }
            };

            var fm25Global = new FM25Global()
            {
                Learners = new List<FM25Learner>()
                {
                    fm25Learner
                }
            };

            var dependentDataMock = new Mock<IReportServiceDependentData>();

            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<FM25Global>()).Returns(fm25Global);

            var models = NewBuilder().Build(null, dependentDataMock.Object).ToList();

            models.Should().HaveCount(1);

            models[0].Learner.Should().Be(learner);
            models[0].FM25Learner.Should().Be(fm25Learner);
        }

        [Fact]
        public void Build_FiftyThousand()
        {
            var count = 50000;
            
            var message = new TestMessage()
            {
                Learners = Enumerable.Range(0, count)
                    .Select(i => new TestLearner()
                    {
                        LearnRefNumber = i.ToString(),
                        LearningDeliveries = new List<ILearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                                {
                                    new TestLearningDeliveryFAM()
                                    {
                                        LearnDelFAMType = "SOF",
                                        LearnDelFAMCode = "107",
                                    }
                                }
                            }
                        }
                    }).ToList()
            };

            var fm25Global = new FM25Global()
            {
                Learners = Enumerable.Range(0, count)
                    .Select(i => new FM25Learner()
                    {
                        LearnRefNumber = i.ToString(),
                        StartFund = true,
                        FundLine = "14-16 Direct Funded Students",
                    }).ToList()
            };

            var dependentDataMock = new Mock<IReportServiceDependentData>();

            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<FM25Global>()).Returns(fm25Global);

            var models = NewBuilder().Build(null, dependentDataMock.Object).ToList();

            models.Should().HaveCount(count);
        }

        private SummaryOfFundingByStudentReportModel BuildModelFor(string fundLine, string learnRefNumber)
        {
            return new SummaryOfFundingByStudentReportModel()
            {
                Learner = new TestLearner()
                {
                    LearnRefNumber = learnRefNumber,
                },
                FM25Learner = new FM25Learner()
                {
                    FundLine = fundLine
                }
            };
        }

        private SummaryOfFundingByStudentModelBuilder NewBuilder() => new SummaryOfFundingByStudentModelBuilder();
    }
}
