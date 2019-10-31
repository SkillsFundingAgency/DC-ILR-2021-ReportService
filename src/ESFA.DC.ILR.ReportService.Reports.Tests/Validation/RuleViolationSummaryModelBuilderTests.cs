using System;
using System.Collections.Generic;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Loose.Interface;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Validation.Summary;
using ESFA.DC.ILR.ReportService.Service.Interface;
using FluentAssertions;
using Moq;
using Xunit;
using ValidationError = ESFA.DC.ILR.ValidationErrors.Interface.Models.ValidationError;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Validation
{
    public class RuleViolationSummaryModelBuilderTests
    {
        [Fact]
        public void BuildTest()
        {
            var referenceDataRoot = BuildReferenceDataRoot();
            var message = BuildIlrMessage();
            var validationErrors = BuildValidationErrors();

            var dependentDataMock = new Mock<IReportServiceDependentData>();
            dependentDataMock.Setup(d => d.Get<ILooseMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<List<ValidationError>>()).Returns(validationErrors);

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

            result.ProviderName.Should().Be("Provider: Provider XYZ");
            result.Ukprn.Should().Be("UKPRN: 987654321");
            result.Year.Should().Be("2019/20");
            result.ApplicationVersion.Should().Be("11.22.3300.4321");
            result.OrganisationData.Should().Be("1.1.1.1");
            result.LargeEmployerData.Should().Be("2.2.2.2");
            result.LarsData.Should().Be("3.3.3.3");
            result.PostcodeData.Should().Be("4.4.4.4");
            result.FilePreparationDate.Should().Be("06/11/2019");
            result.IlrFile.Should().Be("ILR File: ILR-12345678-1920-20191005-151322-01.xml");
            result.ReportGeneratedAt.Should().Be("Report generated at: 01:01:01 on 01/01/2020");

            result.TotalNoOfErrors.Should().Be(5);
            result.TotalNoOfLearners.Should().Be(10);
            result.TotalNoOfWarnings.Should().Be(6);
            result.TotalNoOfLearnersWithWarnings.Should().Be(1);

            result.Errors.Count.Should().Be(2);
            result.Errors[0].RuleName.Should().Be("Rule1");
            result.Errors[0].Message.Should().Be("Rule1Message");
            result.Errors[0].Occurrences.Should().Be(2);
            result.Errors[1].RuleName.Should().Be("Rule2");
            result.Errors[1].Message.Should().Be("Rule2Message");
            result.Errors[1].Occurrences.Should().Be(3);

            result.Warnings.Count.Should().Be(3);
            result.Warnings[0].RuleName.Should().Be("Rule1");
            result.Warnings[0].Message.Should().Be("Rule1Message");
            result.Warnings[0].Occurrences.Should().Be(2);
            result.Warnings[1].RuleName.Should().Be("Rule2");
            result.Warnings[1].Message.Should().Be("Rule2Message");
            result.Warnings[1].Occurrences.Should().Be(3);
            result.Warnings[2].RuleName.Should().Be("Rule3");
            result.Warnings[2].Message.Should().Be("Rule3Message");
            result.Warnings[2].Occurrences.Should().Be(1);

            result.FullyValidLearners.Apprenticeships.Should().Be(5);
            result.FullyValidLearners.Funded1619.Should().Be(5);
            result.FullyValidLearners.AdultSkilledFunded.Should().Be(5);
            result.FullyValidLearners.CommunityLearningFunded.Should().Be(5);
            result.FullyValidLearners.ESFFunded.Should().Be(5);
            result.FullyValidLearners.OtherAdultFunded.Should().Be(5);
            result.FullyValidLearners.Other1619Funded.Should().Be(5);
            result.FullyValidLearners.NonFunded.Should().Be(5);
            result.FullyValidLearners.Total.Should().Be(5);

            result.InvalidLearners.Apprenticeships.Should().Be(5);
            result.InvalidLearners.Funded1619.Should().Be(5);
            result.InvalidLearners.AdultSkilledFunded.Should().Be(5);
            result.InvalidLearners.CommunityLearningFunded.Should().Be(5);
            result.InvalidLearners.ESFFunded.Should().Be(5);
            result.InvalidLearners.OtherAdultFunded.Should().Be(5);
            result.InvalidLearners.Other1619Funded.Should().Be(5);
            result.InvalidLearners.NonFunded.Should().Be(5);
            result.InvalidLearners.Total.Should().Be(5);

            result.LearningDeliveries.Apprenticeships.Should().Be(100);
            result.LearningDeliveries.Funded1619.Should().Be(100);
            result.LearningDeliveries.AdultSkilledFunded.Should().Be(100);
            result.LearningDeliveries.CommunityLearningFunded.Should().Be(100);
            result.LearningDeliveries.ESFFunded.Should().Be(100);
            result.LearningDeliveries.OtherAdultFunded.Should().Be(100);
            result.LearningDeliveries.Other1619Funded.Should().Be(100);
            result.LearningDeliveries.NonFunded.Should().Be(100);
            result.LearningDeliveries.Total.Should().Be(800);

            result.LearnerDestinationProgressionSummary.ValidLearnerDestinationProgressions.Should().Be(3);
            result.LearnerDestinationProgressionSummary.InValidLearnerDestinationProgressions.Should().Be(5);
            result.LearnerDestinationProgressionSummary.Total.Should().Be(8);
        }

        [Fact]
        public void Build_WithNoLearnersAndErrors()
        {
            var referenceDataRoot = BuildReferenceDataRoot();
            var message = MockExtensions.NewMock<ILooseMessage>()
                .With(h => h.HeaderEntity.CollectionDetailsEntity.FilePreparationDate, new DateTime(2019, 11, 06))
                .Build();
            var validationErrors = new List<ValidationError>();

            var dependentDataMock = new Mock<IReportServiceDependentData>();
            dependentDataMock.Setup(d => d.Get<ILooseMessage>()).Returns(message);
            dependentDataMock.Setup(d => d.Get<ReferenceDataRoot>()).Returns(referenceDataRoot);
            dependentDataMock.Setup(d => d.Get<List<ValidationError>>()).Returns(validationErrors);

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

            result.ProviderName.Should().Be("Provider: Provider XYZ");
            result.Ukprn.Should().Be("UKPRN: 987654321");
            result.Year.Should().Be("2019/20");
            result.ApplicationVersion.Should().Be("11.22.3300.4321");
            result.OrganisationData.Should().Be("1.1.1.1");
            result.LargeEmployerData.Should().Be("2.2.2.2");
            result.LarsData.Should().Be("3.3.3.3");
            result.PostcodeData.Should().Be("4.4.4.4");
            result.FilePreparationDate.Should().Be("06/11/2019");
            result.IlrFile.Should().Be("ILR File: ILR-12345678-1920-20191005-151322-01.xml");
            result.ReportGeneratedAt.Should().Be("Report generated at: 01:01:01 on 01/01/2020");

            result.TotalNoOfErrors.Should().Be(0);
            result.TotalNoOfLearners.Should().Be(0);
            result.TotalNoOfWarnings.Should().Be(0);
            result.TotalNoOfLearnersWithWarnings.Should().Be(0);

            result.Errors.Count.Should().Be(0);
            result.Warnings.Count.Should().Be(0);
            
            result.FullyValidLearners.Apprenticeships.Should().Be(0);
            result.FullyValidLearners.Funded1619.Should().Be(0);
            result.FullyValidLearners.AdultSkilledFunded.Should().Be(0);
            result.FullyValidLearners.CommunityLearningFunded.Should().Be(0);
            result.FullyValidLearners.ESFFunded.Should().Be(0);
            result.FullyValidLearners.OtherAdultFunded.Should().Be(0);
            result.FullyValidLearners.Other1619Funded.Should().Be(0);
            result.FullyValidLearners.NonFunded.Should().Be(0);
            result.FullyValidLearners.Total.Should().Be(0);

            result.InvalidLearners.Apprenticeships.Should().Be(0);
            result.InvalidLearners.Funded1619.Should().Be(0);
            result.InvalidLearners.AdultSkilledFunded.Should().Be(0);
            result.InvalidLearners.CommunityLearningFunded.Should().Be(0);
            result.InvalidLearners.ESFFunded.Should().Be(0);
            result.InvalidLearners.OtherAdultFunded.Should().Be(0);
            result.InvalidLearners.Other1619Funded.Should().Be(0);
            result.InvalidLearners.NonFunded.Should().Be(0);
            result.InvalidLearners.Total.Should().Be(0);

            result.LearningDeliveries.Apprenticeships.Should().Be(0);
            result.LearningDeliveries.Funded1619.Should().Be(0);
            result.LearningDeliveries.AdultSkilledFunded.Should().Be(0);
            result.LearningDeliveries.CommunityLearningFunded.Should().Be(0);
            result.LearningDeliveries.ESFFunded.Should().Be(0);
            result.LearningDeliveries.OtherAdultFunded.Should().Be(0);
            result.LearningDeliveries.Other1619Funded.Should().Be(0);
            result.LearningDeliveries.NonFunded.Should().Be(0);
            result.LearningDeliveries.Total.Should().Be(0);

            result.LearnerDestinationProgressionSummary.ValidLearnerDestinationProgressions.Should().Be(0);
            result.LearnerDestinationProgressionSummary.InValidLearnerDestinationProgressions.Should().Be(0);
            result.LearnerDestinationProgressionSummary.Total.Should().Be(0);
        }

        private ReferenceDataRoot BuildReferenceDataRoot()
        {
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
                                CoFRemoval = (decimal) 4500.12
                            }
                        },
                    }
                },
                MetaDatas = new MetaData()
                {
                    ReferenceDataVersions = new ReferenceDataVersion()
                    {
                        OrganisationsVersion = new OrganisationsVersion {Version = "1.1.1.1"},
                        Employers = new EmployersVersion {Version = "2.2.2.2"},
                        LarsVersion = new LarsVersion {Version = "3.3.3.3"},
                        PostcodesVersion = new PostcodesVersion {Version = "4.4.4.4"},
                    },
                    ValidationErrors =new List<ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData.ValidationError>()
                    {
                        new ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData.ValidationError()
                        {
                            RuleName = "Rule1",
                            Message = "Rule1Message",
                        },
                        new ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData.ValidationError()
                        {
                            RuleName = "Rule2",
                            Message = "Rule2Message",
                        },
                        new ReferenceDataService.Model.MetaData.ValidationError()
                        {
                            RuleName = "Rule3",
                            Message = "Rule3Message",
                        }
                    }
                }
            };
            return referenceDataRoot;
        }

        private RuleViolationSummaryReportModelBuilder NewBuilder(IDateTimeProvider dateTimeProvider = null)
        {
            return new RuleViolationSummaryReportModelBuilder(dateTimeProvider);
        }
        private ILooseMessage BuildIlrMessage()
        {
            return MockExtensions.NewMock<ILooseMessage>()
                .With(h => h.HeaderEntity.CollectionDetailsEntity.FilePreparationDate, new DateTime(2019, 11, 06))
                .With(m => m.Learners,BuildLearners())
                .With(l => l.LearnerDestinationAndProgressions, BuildLearnerDesintationAndProgressions()).Build();
        }

        private IReadOnlyCollection<ILooseLearnerDestinationAndProgression> BuildLearnerDesintationAndProgressions()
        {
            var list = new List<ILooseLearnerDestinationAndProgression>();
            for (int i = 1; i <= 8; i++)
            {
                var looseLearnerDestinationAndProgression = MockExtensions.NewMock<ILooseLearnerDestinationAndProgression>()
                    .With(l => l.LearnRefNumber, "LearnRefNumber" + i).Build();
                list.Add(looseLearnerDestinationAndProgression);
            }

            return list;
        }

        private static List<ILooseLearner> BuildLearners()
        {
            var looseLearners = new List<ILooseLearner>();
            for (int i = 1; i <= 10; i++)
            {
                var looseLearner = MockExtensions.NewMock<ILooseLearner>()
                    .With(l => l.LearnRefNumber, "LearnRefNumber"+i)
                    .With(l => l.LearningDeliveries, BuildLearningDeliveries()).Build();
                looseLearners.Add(looseLearner);
            }

            return looseLearners;
        }

        private static List<ILooseLearningDelivery> BuildLearningDeliveries()
        {
            var looseLearningDeliveries = new List<ILooseLearningDelivery>();
            var fundModels = new List<int> {36, 25, 35, 10, 70, 81, 82, 99};
            foreach (var fundModel in fundModels)
            {
                for (int i = 1; i <= 10; i++)
                {
                    var looseLearningDelivery = MockExtensions.NewMock<ILooseLearningDelivery>()
                        .With(ld => ld.FundModelNullable, fundModel)
                        .Build();
                    looseLearningDeliveries.Add(looseLearningDelivery);
                }
            }
            return looseLearningDeliveries;
        }

        private List<ValidationError> BuildValidationErrors()
        {
            var validationErrorList = new List<ValidationError>();
            var severityLevels = new List<string> { "W", "E" };
            foreach (var severityLevel in severityLevels)
            {
                for (var i = 1; i <= 5; i++)
                {
                    var validationError = new ValidationError
                    {
                        LearnerReferenceNumber = "LearnRefNumber" + i,
                        Severity = severityLevel,
                        RuleName = i % 2 == 0 ? "Rule1" : "Rule2"
                    };
                    validationErrorList.Add(validationError);
                }
            }

            var warning = new ValidationError
            {
                LearnerReferenceNumber = "LearnRefNumber" + 6,
                Severity = "W",
                RuleName = "Rule3"
            };

            validationErrorList.Add(warning);

            return validationErrorList;
        }
    }
}
