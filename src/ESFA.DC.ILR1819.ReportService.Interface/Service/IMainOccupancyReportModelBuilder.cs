namespace ESFA.DC.ILR1819.ReportService.Interface.Service
{
    using ESFA.DC.ILR.FundingService.FM25.Model.Output;
    using ESFA.DC.ILR.Model.Interface;
    using Model.Lars;
    using Model.ReportModels;

    public interface IMainOccupancyReportModelBuilder
    {
        MainOccupancyModel BuildFm35Model(
            ILearner learner,
            ILearningDelivery learningDelivery,
            LarsLearningDelivery larsModel,
            LearningDelivery frameworkAim,
            ILR.FundingService.FM35.FundingOutput.Model.Output.LearningDelivery fm35Data,
            IStringUtilitiesService stringUtilitiesService);

        MainOccupancyModel BuildFm25Model(
            ILearner learner,
            ILearningDelivery learningDelivery,
            FM25Learner fm25Data);
    }
}