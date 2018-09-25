namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    using ESFA.DC.ILR.FundingService.FM25.Model.Output;
    using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Attribute;
    using ESFA.DC.ILR.Model.Interface;
    using Model.Lars;
    using Model.ReportModels;

    public interface IMainOccupancyReportModelBuilder
    {
        MainOccupancyFM35Model BuildFm35Model(
            ILearner learner,
            ILearningDelivery learningDelivery,
            LarsLearningDelivery larsModel,
            LearningDelivery frameworkAim,
            LearningDeliveryAttribute fm35Data);

        MainOccupancyFM25Model BuildFm25Model(
            ILearner learner,
            ILearningDelivery learningDelivery,
            FM25Learner fm25Data);
    }
}