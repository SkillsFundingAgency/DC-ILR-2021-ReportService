using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Builders;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ReportService.Reports.Tests.Builders
{
    public class ValidationErrorsReportBuilderTests
    {
        [Fact]
        public void TestValidationReportBuilder()
        {
            var sut = new ValidationErrorsReportBuilder();
            var buildIlrValidationErrors = BuildIlrValidationErrors();
            var buildIlrMessage = BuildIlrMessage();
            var buildIlrValidationErrorsMetadata = BuildIlrValidationErrorsMetadata();
            var result = sut.Build(buildIlrValidationErrors, buildIlrMessage, buildIlrValidationErrorsMetadata);

            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(3);
            result.First().AimSequenceNumber.Should().Be(1);
            result.First().ErrorMessage.Should().Be("The Achievement date must not be before the Learning start date");
            result.First().FieldValues.Should().Be("LearnStartDate=14/10/2018|AchDate=12/10/2018|");
            result.First().FundModel.Should().Be(82);
            result.First().LearnAimRef.Should().Be("60133533");
            result.First().LearnerReferenceNumber.Should().Be("0AchDt03");
            result.First().RuleName.Should().Be("AchDate_03");
            result.First().SWSupAimId.Should().Be("01035bef-5316-4e83-9f5d-53536ac97bb2");
            result.First().Severity.Should().Be("E");
            result.First().ProviderSpecDelOccurA.Should().Be("1");
            result.First().ProviderSpecDelOccurB.Should().Be("2");
            result.First().ProviderSpecDelOccurC.Should().Be("3");
            result.First().ProviderSpecDelOccurD.Should().Be("4");

            result.ElementAt(1).AimSequenceNumber.Should().Be(1);
            result.ElementAt(1).ErrorMessage.Should().Be("The Achievement date must be after the Learning actual end date");
            result.ElementAt(1).FieldValues.Should().Be("LearnEndDate=13/11/2018|AchDate=12/10/2018|");
            result.ElementAt(1).FundModel.Should().Be(82);
            result.ElementAt(1).LearnAimRef.Should().Be("60133533");
            result.ElementAt(1).LearnerReferenceNumber.Should().Be("0AchDt03");
            result.ElementAt(1).RuleName.Should().Be("AchDate_05");
            result.ElementAt(1).SWSupAimId.Should().Be("01035bef-5316-4e83-9f5d-53536ac97bb2");
            result.ElementAt(1).Severity.Should().Be("E");

            result.ElementAt(2).AimSequenceNumber.Should().Be(1);
            result.ElementAt(2).ErrorMessage.Should().Be("The end point assessment organisation is not valid for this standard on this planned end date.");
            result.ElementAt(2).FieldValues.Should().Be("EPAOrgID=EPA0032|StdCode=1|LearnPlanEndDate=31/07/2020|");
            result.ElementAt(2).FundModel.Should().Be(81);
            result.ElementAt(2).LearnAimRef.Should().Be("ZPROG001");
            result.ElementAt(2).LearnerReferenceNumber.Should().Be("0AchDt08");
            result.ElementAt(2).RuleName.Should().Be("EPAOrgID_01");
            result.ElementAt(2).SWSupAimId.Should().Be("6406d4f6-f38a-45a9-9b9a-f956728ce540");
            result.ElementAt(2).Severity.Should().Be("W");
        }

        private IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> BuildIlrValidationErrorsMetadata()
        {
            IReadOnlyCollection<ReferenceDataService.Model.MetaData.ValidationError> validationErrorsMetadata
                = new List<ReferenceDataService.Model.MetaData.ValidationError>()
                {
                    new ReferenceDataService.Model.MetaData.ValidationError()
                    {
                        Severity = ReferenceDataService.Model.MetaData.ValidationError.SeverityLevel.Error,
                        RuleName = "AchDate_03",
                        Message = "The Achievement date must not be before the Learning start date"
                    },
                    new ReferenceDataService.Model.MetaData.ValidationError()
                    {
                        Severity = ReferenceDataService.Model.MetaData.ValidationError.SeverityLevel.Error,
                        RuleName = "AchDate_05",
                        Message = "The Achievement date must be after the Learning actual end date"
                    },
                    new ReferenceDataService.Model.MetaData.ValidationError()
                    {
                        Severity = ReferenceDataService.Model.MetaData.ValidationError.SeverityLevel.Warning,
                        RuleName = "EPAOrgID_01",
                        Message = "The end point assessment organisation is not valid for this standard on this planned end date."
                    }
                };
            return validationErrorsMetadata;
        }

        private IMessage BuildIlrMessage()
        {
            IMessage message = new TestMessage()
            {
                
                Learners = new List<ILearner>()
                {
                    new TestLearner()
                    {
                        LearnRefNumber = "0AchDt03",
                       LearningDeliveries = new List<ILearningDelivery>()
                       {
                           new TestLearningDelivery()
                           {
                               AimSeqNumber = 1,
                               LearnAimRef = "60133533",
                               SWSupAimId = "01035bef-5316-4e83-9f5d-53536ac97bb2",
                               FundModel = 82,
                               ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                               {
                                   new TestProviderSpecDeliveryMonitoring()
                                   {
                                       ProvSpecDelMon = "1",
                                       ProvSpecDelMonOccur ="A"
                                   },
                                   new TestProviderSpecDeliveryMonitoring()
                                   {
                                       ProvSpecDelMon = "2",
                                       ProvSpecDelMonOccur ="B"
                                   },
                                   new TestProviderSpecDeliveryMonitoring()
                                   {
                                       ProvSpecDelMon = "3",
                                       ProvSpecDelMonOccur ="C"
                                   },
                                   new TestProviderSpecDeliveryMonitoring()
                                   {
                                       ProvSpecDelMon = "4",
                                       ProvSpecDelMonOccur ="D"
                                   }

                               }
                           }
                       }
                    },
                    new TestLearner()
                    {
                        LearnRefNumber = "0AchDt08",
                        LearningDeliveries = new List<ILearningDelivery>()
                        {
                            new TestLearningDelivery()
                            {
                                AimSeqNumber = 1,
                                LearnAimRef = "ZPROG001",
                                SWSupAimId = "6406d4f6-f38a-45a9-9b9a-f956728ce540",
                                FundModel = 81
                            }
                        }
                    }
                }
            };
           
         
            return message;
        }

        private List<ValidationError> BuildIlrValidationErrors()
        {
            List<ValidationError> ilrValidationErrors = new List<ValidationError>()
            {
                new ValidationError()
                {
                    RuleName = "AchDate_03",
                    Severity = "E",
                    LearnerReferenceNumber = "0AchDt03",
                    AimSequenceNumber = 1,
                    ValidationErrorParameters = new List<ValidationErrorParameter>()
                    {
                        new ValidationErrorParameter()
                        {
                            PropertyName = "LearnStartDate",
                            Value = "14/10/2018"
                        },
                        new ValidationErrorParameter()
                        {
                            PropertyName = "AchDate",
                            Value = "12/10/2018"
                        }
                    }
                },
                new ValidationError()
                {
                    RuleName = "AchDate_05",
                    Severity = "E",
                    LearnerReferenceNumber = "0AchDt03",
                    AimSequenceNumber = 1,
                    ValidationErrorParameters = new List<ValidationErrorParameter>()
                    {
                        new ValidationErrorParameter()
                        {
                            PropertyName = "LearnEndDate",
                            Value = "13/11/2018"
                        },
                        new ValidationErrorParameter()
                        {
                            PropertyName = "AchDate",
                            Value = "12/10/2018"
                        }
                    }
                },
                new ValidationError()
                {
                    RuleName = "EPAOrgID_01",
                    Severity = "W",
                    LearnerReferenceNumber = "0AchDt08",
                    AimSequenceNumber = 1,
                    ValidationErrorParameters = new List<ValidationErrorParameter>()
                    {
                        new ValidationErrorParameter()
                        {
                            PropertyName = "EPAOrgID",
                            Value = "EPA0032"
                        },
                        new ValidationErrorParameter()
                        {
                            PropertyName = "StdCode",
                            Value = "1"
                        },
                        new ValidationErrorParameter()
                        {
                            PropertyName = "LearnPlanEndDate",
                            Value = "31/07/2020"
                        }
                    }
                }
            };

            return ilrValidationErrors;
        }
    }
}
