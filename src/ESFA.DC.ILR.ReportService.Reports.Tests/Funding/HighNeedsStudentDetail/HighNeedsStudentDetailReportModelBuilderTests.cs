using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentDetail;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.HighNeedsStudentDetail
{
    public class HighNeedsStudentDetailReportModelBuilderTests
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
            var one = BuildModelFor("14-16 Direct Funded Students", "14-16 Direct Funded Students", "A");
            var two = BuildModelFor("16-19 Students (excluding High Needs Students)", "16-19 Students (including High Needs Students)", "A");
            var three = BuildModelFor("16-19 High Needs Students", "16-19 Students (including High Needs Students)", "A");
            var four = BuildModelFor("19-24 Students with an EHCP", "19-24 Students with an EHCP", "A");
            var five = BuildModelFor("19-24 Students with an EHCP", "19-24 Students with an EHCP", "B");
            var six = BuildModelFor("19+ Continuing Students (excluding EHCP)", "19+ Continuing Students (excluding EHCP)", "A");
            var seven = BuildModelFor("Anything else that should not be in the list really but will go to the end of the report", "Any", "A");

            var models = new List<HighNeedsStudentDetailReportModel>()
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
                },
                LearnerFAMs = new List<ILearnerFAM>()
                {
                    new TestLearnerFAM() { LearnFAMType = "EHC", LearnFAMCode = 1 },
                    new TestLearnerFAM() { LearnFAMType = "HNS", LearnFAMCode = 1 }
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
            models[0].DerivedFundline.Should().Be(fm25Learner.FundLine);
            models[0].StudentsWithAnEhcp.Should().BeTrue();
            models[0].StudentsWithoutAnEhcp.Should().BeFalse();
            models[0].HighNeedsStudentsWithoutAnEhcp.Should().BeFalse();
            models[0].StudentsWithAnEhcpAndHns.Should().BeTrue();
            models[0].StudentWithAnEhcpAndNotHns.Should().BeFalse();
        }

        [Fact]
        public void Build_One_1619()
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
                },
                LearnerFAMs = new List<ILearnerFAM>()
                {
                    new TestLearnerFAM() { LearnFAMType = "EHC", LearnFAMCode = 1 },
                    new TestLearnerFAM() { LearnFAMType = "HNS", LearnFAMCode = 1 }
                }
            };

            var fm25Learner = new FM25Learner()
            {
                LearnRefNumber = "LearnRefNumber",
                StartFund = true,
                FundLine = "16-19 Students (excluding High Needs Students)",
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
            models[0].DerivedFundline.Should().Be("16-19 Students (including High Needs Students)");
            models[0].StudentsWithAnEhcp.Should().BeTrue();
            models[0].StudentsWithoutAnEhcp.Should().BeFalse();
            models[0].HighNeedsStudentsWithoutAnEhcp.Should().BeFalse();
            models[0].StudentsWithAnEhcpAndHns.Should().BeTrue();
            models[0].StudentWithAnEhcpAndNotHns.Should().BeFalse();
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

        [Fact]
        public void StudentsWithAnEhcp_True()
        {
            NewBuilder().StudentWithAnEhcp(1).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void StudentsWithAnEhcp_False(int? ehc)
        {
            NewBuilder().StudentWithAnEhcp(ehc).Should().BeFalse();
        }

        [Fact]
        public void StudentsWithoutAnEhcp_True()
        {
            NewBuilder().StudentWithoutAnEhcp(null).Should().BeTrue();
        }

        [Fact]
        public void StudentsWithoutAnEhcp_False()
        {
            NewBuilder().StudentWithoutAnEhcp(1).Should().BeFalse();
        }

        [Fact]
        public void HighNeedsStudentsWithoutAnEhcp_True()
        {
            NewBuilder().HighNeedsStudentWithoutAnEhcp(1, null).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(0, null)]
        [InlineData(1, 1)]
        public void HighNeedsStudentsWithoutAnEhcp_False(int? hns, int? ehc)
        {
            NewBuilder().HighNeedsStudentWithoutAnEhcp(hns, ehc).Should().BeFalse();
        }

        [Fact]
        public void StudentsWithAnEhcpAndHns_True()
        {
            NewBuilder().StudentWithAnEhcpAndHns(1, 1).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(1, null)]
        [InlineData(null, 1)]
        public void StudentsWithAnEhcpAndHns_False(int? hns, int? ehc)
        {
            NewBuilder().StudentWithAnEhcpAndHns(hns, ehc).Should().BeFalse();
        }

        [Fact]
        public void StudentsWithAnEhcpAndNoHns_True()
        {
            NewBuilder().StudentWithAnEhcpAndNotHns(null, 1).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(1, null)]
        [InlineData(1, 1)]
        public void StudentsWithAnEhcpAndNoHns_False(int? hns, int? ehc)
        {
            NewBuilder().StudentWithAnEhcpAndNotHns(hns, ehc).Should().BeFalse();
        }


        private HighNeedsStudentDetailReportModel BuildModelFor(string fundLine, string derivedFundLine, string learnRefNumber)
        {
            return new HighNeedsStudentDetailReportModel()
            {
                Learner = new TestLearner()
                {
                    LearnRefNumber = learnRefNumber,
                },
                FM25Learner = new FM25Learner()
                {
                    FundLine = fundLine
                },
                DerivedFundline = derivedFundLine
            };
        }

        private HighNeedsStudentDetailReportModelBuilder NewBuilder() => new HighNeedsStudentDetailReportModelBuilder();
    }
}
