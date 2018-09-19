﻿using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Attribute;

namespace ESFA.DC.ILR1819.ReportService.Tests.Helpers
{
    public static class TestFM35Builder
    {
        public static FM35FundingOutputs Build()
        {
            return new FM35FundingOutputs
            {
                Global = new GlobalAttribute(),
                Learners = new[]
                {
                    new LearnerAttribute
                    {
                        LearnRefNumber = "3fm9901",
                        LearningDeliveryAttributes = new[]
                        {
                            new LearningDeliveryAttribute
                            {
                                AimSeqNumber = 1,
                                LearningDeliveryAttributeDatas = new LearningDeliveryAttributeData
                                {
                                    FundLine = "12345"
                                },
                                LearningDeliveryPeriodisedAttributes = new[]
                                {
                                    new LearningDeliveryPeriodisedAttribute
                                    {
                                        AttributeName = "OnProgPayment",
                                        Period1 = 102.45M
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}