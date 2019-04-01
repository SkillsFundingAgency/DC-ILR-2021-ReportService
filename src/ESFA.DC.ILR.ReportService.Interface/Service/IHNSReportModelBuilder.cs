using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Model.ReportModels;

namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    public interface IHNSReportModelBuilder
    {
        HNSModel BuildModel(
            ILearner learner,
            ILearningDelivery learningDelivery,
            FM25Learner fm25Data);
    }
}
