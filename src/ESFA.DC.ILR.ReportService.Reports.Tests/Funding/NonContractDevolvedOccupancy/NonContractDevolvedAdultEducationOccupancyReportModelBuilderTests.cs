using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MCAGLA;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.NonContractDevolved;
using ESFA.DC.ILR.ReportService.Reports.Model;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.Tests.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Funding.NonContractDevolvedOccupancy
{
    public class NonContractDevolvedAdultEducationOccupancyReportModelBuilderTests
    {
        [Fact]
        public void LearningDeliveryFilter_False_Null()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(null as IReadOnlyCollection<ILearningDeliveryFAM>);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Empty()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(new List<ILearningDeliveryFAM>());

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Type()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("NotSOF");

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };
            
            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFilter_False_Code()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("SOF");
            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMCode).Returns("999");

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeFalse();
        }

        [InlineData("110")]
        [InlineData("111")]
        [InlineData("112")]
        [InlineData("113")]
        [InlineData("114")]
        [InlineData("115")]
        [InlineData("116")]
        [Theory]
        public void LearningDeliveryFilter_True(string code)
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();
            var learningDeliveryFAM = new Mock<ILearningDeliveryFAM>();

            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMType).Returns("SOF");
            learningDeliveryFAM.SetupGet(fam => fam.LearnDelFAMCode).Returns(code);

            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                learningDeliveryFAM.Object
            };

            learningDeliveryMock.SetupGet(ld => ld.LearningDeliveryFAMs).Returns(learningDeliveryFAMs);

            NewBuilder().LearningDeliveryReportFilter(learningDeliveryMock.Object).Should().BeTrue();
        }

        [Fact]
        public void OrderBy()
        {
            var six = new NonContractDevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "D" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var five = new NonContractDevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "C" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var two = new NonContractDevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 2 } };
            var four = new NonContractDevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "B" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };
            var three = new NonContractDevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 3 } };
            var one = new NonContractDevolvedAdultEducationOccupancyReportModel() { Learner = new TestLearner() { LearnRefNumber = "A" }, LearningDelivery = new TestLearningDelivery() { AimSeqNumber = 1 } };

            var rows = new List<NonContractDevolvedAdultEducationOccupancyReportModel>()
            {
                six, five, two, four, three, one
            };

            var result = NewBuilder().Order(rows).ToList();

            result.Should().ContainInOrder(one, two, three, four, five, six);
        }

        [Fact]
        public void Build_SingleRow()
        {
            var reportServiceContext = new Mock<IReportServiceContext>();
            reportServiceContext.Setup(rsc => rsc.Ukprn).Returns(1);

            var dependentDataMock = new Mock<IReportServiceDependentData>();

            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "110",
                }
            };

            var providerSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
            {
                new TestProviderSpecDeliveryMonitoring(),
            };

            var learningDelivery = new TestLearningDelivery()
            {
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
                LearnAimRef = "learnAimRef",
                LARSLearningDeliveryCategories = new HashSet<LARSLearningDeliveryCategory>
                {
                    new LARSLearningDeliveryCategory
                    {
                        CategoryRef = 123
                    }
                }
            };

            var mcaGlaSofLookup = new McaGlaSofLookup()
            {
                SofCode = "110",
                McaGlaShortCode = "GMCA",
                McaGlaFullName = "GMCA Full",
            };

            var mcaDevolvedContract = new McaDevolvedContract()
            {
                Ukprn = 1,
                McaGlaShortCode = "GMCB",
                EffectiveFrom = new DateTime(2000,01,01),
                EffectiveTo = new DateTime(2000,01,01)
            };

            var referenceDataRoot = new ReferenceDataRoot()
            {
                LARSLearningDeliveries = new List<LARSLearningDelivery>()
                {
                    larsLearningDelivery
                },
                DevolvedPostocdes = new DevolvedPostcodes()
                {
                    McaGlaSofLookups = new List<McaGlaSofLookup>()
                    {
                        mcaGlaSofLookup
                    }
                },
                McaDevolvedContracts = new List<McaDevolvedContract>()
                {
                    mcaDevolvedContract
                },
                Organisations = new List<Organisation>()
                {
                    new Organisation(){UKPRN = 12345678, Name = "Partner Provider"}
                }
            };

            var fm35LearningDeliveryValue = new LearningDeliveryValue();

            var fm35LearningDelivery = new LearningDelivery()
            {
                AimSeqNumber = 1,
                LearningDeliveryValue = fm35LearningDeliveryValue,
                LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>()
                {
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35OnProgPayment),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35AchievePayPct),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35AchievePayment),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35EmpOutcomePay),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35BalancePayment),
                    BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35LearnSuppFundCash),
                }
            };

            var fm35Global = new FM35Global()
            {
                Learners = new List<FM35Learner>()
                {
                    new FM35Learner()
                    {
                        LearnRefNumber = "LearnRefNumber",
                        LearningDeliveries = new List<LearningDelivery>()
                        {
                            fm35LearningDelivery,
                        }
                    }
                }
            };

            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<FM35Global>()).Returns(fm35Global);

            var providerSpecDeliveryMonitoringModel = new ProviderSpecDeliveryMonitoringModel();
            var providerSpecLearnerMonitoringModel = new ProviderSpecLearnerMonitoringModel();
            var employmentStatusMonitoring = new EmploymentStatusMonitoringModel();
            var learningDeliveryFamsModel = new LearningDeliveryFAMsModel()
            {
                SOF = "110"
            };

            var ilrModelMapperMock = new Mock<IIlrModelMapper>();
            var academicYearServiceMock = new Mock<IAcademicYearService>();

            ilrModelMapperMock.Setup(m => m.MapLearningDeliveryFAMs(learningDeliveryFams)).Returns(learningDeliveryFamsModel);
            ilrModelMapperMock.Setup(m => m.MapProviderSpecDeliveryMonitorings(providerSpecDeliveryMonitorings)).Returns(providerSpecDeliveryMonitoringModel);
            ilrModelMapperMock.Setup(m => m.MapProviderSpecLearnerMonitorings(providerSpecLearnerMonitorings)).Returns(providerSpecLearnerMonitoringModel);
            ilrModelMapperMock.Setup(m => m.MapEmploymentStatusMonitorings(It.IsAny<IEnumerable<IEmploymentStatusMonitoring>>())).Returns(employmentStatusMonitoring);

            var result = NewBuilder(ilrModelMapperMock.Object, academicYearServiceMock.Object).Build(reportServiceContext.Object, dependentDataMock.Object).ToList();   

            result.Should().HaveCount(1);

            var row = result[0];

            row.Learner.Should().Be(learner);
            row.LearningDelivery.Should().Be(learningDelivery);
            row.LarsLearningDelivery.Should().Be(larsLearningDelivery);
            row.Fm35LearningDelivery.Should().Be(fm35LearningDeliveryValue);

            row.ProviderSpecDeliveryMonitoring.Should().Be(providerSpecDeliveryMonitoringModel);
            row.ProviderSpecLearnerMonitoring.Should().Be(providerSpecLearnerMonitoringModel);
            row.PeriodisedValues.Should().NotBeNull();
            row.LearningDeliveryFAMs.Should().Be(learningDeliveryFamsModel);

            row.McaGlaShortCode.Should().Be("GMCA");
        }

        [Fact]
        public void Build_FiftyThousandLearners()
        {
            var reportServiceContext = Mock.Of<IReportServiceContext>();
            var dependentDataMock = new Mock<IReportServiceDependentData>();

            var ilrModelMapperMock = new Mock<IIlrModelMapper>();
            var academicYearServiceMock = new Mock<IAcademicYearService>();

            var providerSpecDeliveryMonitoring = new ProviderSpecDeliveryMonitoringModel();
            var providerSpecLearnMonitoring = new ProviderSpecLearnerMonitoringModel();
            var famModel = new LearningDeliveryFAMsModel();
            var employmentStatusMonitoring = new EmploymentStatusMonitoringModel();

            ilrModelMapperMock.Setup(m => m.MapProviderSpecDeliveryMonitorings(It.IsAny<IEnumerable<IProviderSpecDeliveryMonitoring>>())).Returns(providerSpecDeliveryMonitoring);
            ilrModelMapperMock.Setup(m => m.MapProviderSpecLearnerMonitorings(It.IsAny<IEnumerable<IProviderSpecLearnerMonitoring>>())).Returns(providerSpecLearnMonitoring);
            ilrModelMapperMock.Setup(m => m.MapLearningDeliveryFAMs(It.IsAny<IEnumerable<ILearningDeliveryFAM>>())).Returns(famModel);
            ilrModelMapperMock.Setup(m => m.MapEmploymentStatusMonitorings(It.IsAny<IEnumerable<IEmploymentStatusMonitoring>>())).Returns(employmentStatusMonitoring);

            var learnerCount = 50000;
            
            var message = new TestMessage()
            {
                Learners = 
                    Enumerable.Range(0, learnerCount).Select(
                        l => new TestLearner()
                        {
                            LearnRefNumber = "LearnRefNumber" + l,
                            LearningDeliveries = new List<ILearningDelivery>()
                            {
                                new TestLearningDelivery()
                                {
                                    LearnAimRef = "learnAimRef" + l,
                                    AimSeqNumber = 1,
                                    LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                                    {
                                        new TestLearningDeliveryFAM()
                                        {
                                            LearnDelFAMType = "SOF",
                                            LearnDelFAMCode = "110",
                                        }
                                    }
                                }
                            }
                        }).ToList()
            };

            var mcaGlaSofLookup = new McaGlaSofLookup()
            {
                SofCode = "110",
                McaGlaShortCode = "GMCA",
                McaGlaFullName = "GMCA Full",
            };

            var referenceDataRoot = new ReferenceDataRoot()
            {
                LARSLearningDeliveries = Enumerable.Range(0, learnerCount)
                    .Select(ld =>
                        new LARSLearningDelivery()
                        {
                            LearnAimRef = "learnAimRef" + ld,
                            LARSLearningDeliveryCategories = new HashSet<LARSLearningDeliveryCategory>
                            {
                                new LARSLearningDeliveryCategory
                                {
                                    CategoryRef = 123
                                }
                            },
                        }).ToList(),
                DevolvedPostocdes = new DevolvedPostcodes
                {
                    McaGlaSofLookups = new List<McaGlaSofLookup>
                    {
                        mcaGlaSofLookup
                    }
                },
                Organisations = new List<Organisation>()
                {
                    new Organisation(){UKPRN = 12345678, Name = "Partner Provider"}
                }
            };
            
            var fm35Global = new FM35Global()
            {
                Learners = Enumerable.Range(0, learnerCount)
                    .Select(
                        l =>
                        new FM35Learner()
                        {
                            LearnRefNumber = "LearnRefNumber" + l,
                            LearningDeliveries = new List<LearningDelivery>()
                            {
                                new LearningDelivery()
                                {
                                    AimSeqNumber = 1,
                                    LearningDeliveryValue = new LearningDeliveryValue(),
                                    LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValue>()
                                    {
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35OnProgPayment),
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35AchievePayPct),
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35AchievePayment),
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35EmpOutcomePay),
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35BalancePayment),
                                        BuildPeriodisedValuesForAttribute(AttributeConstants.Fm35LearnSuppFundCash),
                                    }
                                }
                            }
                        }).ToList()
            };

            dependentDataMock.Setup(d => d.Get<IMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<FM35Global>()).Returns(fm35Global);

            var result = NewBuilder(ilrModelMapperMock.Object, academicYearServiceMock.Object).Build(reportServiceContext, dependentDataMock.Object).ToList();

            result.Should().HaveCount(learnerCount);
        }

        [Fact]
        public void BuildSofDictionary()
        {
            var mcaSofList = new List<McaGlaSofLookup>
            {
                new McaGlaSofLookup
                {
                    SofCode = "10",
                    McaGlaShortCode = "Code1",
                    EffectiveFrom = new System.DateTime(2017, 8, 1)
                },
                new McaGlaSofLookup
                {
                    SofCode = "110",
                    McaGlaShortCode = "Code1",
                    EffectiveFrom = new System.DateTime(2018, 8, 1)
                },
                new McaGlaSofLookup
                {
                    SofCode = "120",
                    McaGlaShortCode = "Code2",
                    EffectiveFrom = new System.DateTime(2020, 8, 1),
                    EffectiveTo = new System.DateTime(2021, 7, 31)
                },
                new McaGlaSofLookup
                {
                    SofCode = "115",
                    McaGlaShortCode = "Code3",
                    EffectiveFrom = new System.DateTime(2019, 8, 1),
                    EffectiveTo = new System.DateTime(2020, 7, 31)
                }
            };

            var expectedDictionary = new Dictionary<string, string>
            {
                { "110", "Code1"},
                { "115", "Code3"},
            };

            var academicYearServiceMock = new Mock<IAcademicYearService>();
            academicYearServiceMock.Setup(am => am.YearStart).Returns(new System.DateTime(2019, 8, 1));
            academicYearServiceMock.Setup(am => am.YearEnd).Returns(new System.DateTime(2020, 7, 31, 23, 59, 59));

            NewBuilder(academicYearService: academicYearServiceMock.Object).BuildSofDictionary(mcaSofList).Should().BeEquivalentTo(expectedDictionary);
        }

        [Fact]
        public void HasValidContractTrue_LearnStartDate()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2019, 10, 01),
                LearnActEndDateNullable = new DateTime(2020, 04, 01),
                LearnPlanEndDate = new DateTime(2020, 05, 01)
            };

            var contracts = new List<McaDevolvedContract>()
            {
                new McaDevolvedContract()
                {
                    Ukprn = 123456789,
                    McaGlaShortCode = "GMCA",
                    EffectiveFrom = new DateTime(2019, 09, 01),
                    EffectiveTo = new DateTime(2020, 06, 30)
                }
            };

            var academicYearServiceMock = new Mock<IAcademicYearService>();
            academicYearServiceMock.Setup(ay => ay.YearEnd).Returns(new DateTime(2020, 08, 31));

            var result = NewBuilder(academicYearService: academicYearServiceMock.Object).HasValidContract(learningDelivery, contracts);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("01/12/2021", "11/06/2021")]
        [InlineData("11/12/2020", "01/12/2022")]
        [InlineData("10/26/2020", "12/20/2021")]
        [InlineData("11/15/2020", "06/15/2022")]
        [InlineData("12/12/2020", "08/15/2021")]
        [InlineData("03/05/2021", "09/10/2021")]
        [InlineData("11/12/2020", "09/06/2021")]
        [InlineData("11/13/2020", "12/13/2021")]
        [InlineData("11/26/2020", "09/20/2021")]
        [InlineData("12/14/2020", "09/07/2021")]
        public void HasValidContractTrue_EffectiveToSameDayAsAcademicYearEnd(string learnStartDate, string learnPlanEndDate)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = DateTime.ParseExact(learnStartDate, "MM/dd/yyyy", CultureInfo.CurrentCulture),
                LearnActEndDateNullable = null,
                LearnPlanEndDate = DateTime.ParseExact(learnPlanEndDate, "MM/dd/yyyy", CultureInfo.CurrentCulture)
            };

            var contracts = new List<McaDevolvedContract>()
            {
                new McaDevolvedContract()
                {
                    Ukprn = 123456789,
                    McaGlaShortCode = "CPCA",
                    EffectiveFrom = new DateTime(2019, 08, 01),
                    EffectiveTo = new DateTime(2021, 07, 31)
                }
            };

            var academicYearServiceMock = new Mock<IAcademicYearService>();
            academicYearServiceMock.Setup(ay => ay.YearEnd).Returns(new DateTime(2021, 7, 31, 23, 59, 59));

            var result = NewBuilder(academicYearService: academicYearServiceMock.Object).HasValidContract(learningDelivery, contracts);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("10/20/2020", "05/06/2021")]
        [InlineData("08/12/2020", "06/15/2021")]
        [InlineData("10/15/2020", "06/15/2021")]
        public void HasValidContractFalse_LearnPlanEndDateAfterEffective(string learnStartDate, string learnPlanEndDate)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = DateTime.ParseExact(learnStartDate, "MM/dd/yyyy", CultureInfo.CurrentCulture),
                LearnActEndDateNullable = null,
                LearnPlanEndDate = DateTime.ParseExact(learnPlanEndDate, "MM/dd/yyyy", CultureInfo.CurrentCulture)
            };

            var contracts = new List<McaDevolvedContract>()
            {
                new McaDevolvedContract()
                {
                    Ukprn = 123456789,
                    McaGlaShortCode = "WECA",
                    EffectiveFrom = new DateTime(2019, 08, 01),
                    EffectiveTo = new DateTime(2020, 12, 31)
                }
            };

            var academicYearServiceMock = new Mock<IAcademicYearService>();
            academicYearServiceMock.Setup(ay => ay.YearEnd).Returns(new DateTime(2021, 7, 31, 23, 59, 59));

            var result = NewBuilder(academicYearService: academicYearServiceMock.Object).HasValidContract(learningDelivery, contracts);

            result.Should().BeFalse();
        }

        [Fact]
        public void HasValidContractFalse_LearnStartDate()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2018, 10, 01),
                LearnActEndDateNullable = new DateTime(2020, 04, 01),
                LearnPlanEndDate = new DateTime(2020, 05, 01)
            };

            var contracts = new List<McaDevolvedContract>()
            {
                new McaDevolvedContract()
                {
                    Ukprn = 123456789,
                    McaGlaShortCode = "GMCA",
                    EffectiveFrom = new DateTime(2019, 09, 01),
                    EffectiveTo = new DateTime(2020, 06, 30)
                }
            };

            var academicYearServiceMock = new Mock<IAcademicYearService>();
            academicYearServiceMock.Setup(ay => ay.YearEnd).Returns(new DateTime(2020, 08, 31));

            var result = NewBuilder(academicYearService: academicYearServiceMock.Object).HasValidContract(learningDelivery, contracts);

            result.Should().BeFalse();
        }

        [Fact]
        public void HasValidContractFalse_LearnActEndDate()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2019, 10, 01),
                LearnActEndDateNullable = new DateTime(2020, 08, 01),
                LearnPlanEndDate = new DateTime(2020, 05, 01)
            };

            var contracts = new List<McaDevolvedContract>()
            {
                new McaDevolvedContract()
                {
                    Ukprn = 123456789,
                    McaGlaShortCode = "GMCA",
                    EffectiveFrom = new DateTime(2019, 09, 01),
                    EffectiveTo = new DateTime(2020, 06, 30)
                }
            };

            var academicYearServiceMock = new Mock<IAcademicYearService>();
            academicYearServiceMock.Setup(ay => ay.YearEnd).Returns(new DateTime(2020, 08, 31));

            var result = NewBuilder(academicYearService: academicYearServiceMock.Object).HasValidContract(learningDelivery, contracts);

            result.Should().BeFalse();
        }

        [Fact]
        public void HasValidContractTrue_NullLearnActEndDate()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2019, 10, 01),
                LearnPlanEndDate = new DateTime(2020, 05, 01)
            };

            var contracts = new List<McaDevolvedContract>()
            {
                new McaDevolvedContract()
                {
                    Ukprn = 123456789,
                    McaGlaShortCode = "GMCA",
                    EffectiveFrom = new DateTime(2019, 09, 01),
                    EffectiveTo = new DateTime(2020, 06, 30)
                }
            };

            var academicYearServiceMock = new Mock<IAcademicYearService>();
            academicYearServiceMock.Setup(ay => ay.YearEnd).Returns(new DateTime(2020, 08, 31));

            var result = NewBuilder(academicYearService: academicYearServiceMock.Object).HasValidContract(learningDelivery, contracts);

            result.Should().BeTrue();
        }

        [Fact]
        public void HasValidContractTrue_AcademicYearNotLessThanEffectiveTo()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnStartDate = new DateTime(2019, 10, 01),
                LearnPlanEndDate = new DateTime(2021, 06, 30)
            };

            var contracts = new List<McaDevolvedContract>()
            {
                new McaDevolvedContract()
                {
                    Ukprn = 123456789,
                    McaGlaShortCode = "GMCA",
                    EffectiveFrom = new DateTime(2019, 08, 18),
                    EffectiveTo = new DateTime(2020, 07, 31)
                }
            };

            var academicYearServiceMock = new Mock<IAcademicYearService>();
            academicYearServiceMock.Setup(ay => ay.YearEnd).Returns(new DateTime(2020, 07, 31));

            var result = NewBuilder(academicYearService: academicYearServiceMock.Object).HasValidContract(learningDelivery, contracts);

            result.Should().BeTrue();
        }

        private NonContractDevolvedAdultEducationOccupancyReportModelBuilder NewBuilder(IIlrModelMapper ilrModelMapper = null, IAcademicYearService academicYearService = null)
        {
            return new NonContractDevolvedAdultEducationOccupancyReportModelBuilder(ilrModelMapper, academicYearService);
        } 

        private LearningDeliveryPeriodisedValue BuildPeriodisedValuesForAttribute(string attribute)
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
                Period12 =  12.1212m
            };
        }
    }
}
