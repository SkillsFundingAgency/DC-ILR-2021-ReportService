using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm99;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm99Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public Fm99Provider(
            IFileService fileService, 
            IJsonSerializationService serializationService) 
            : base(fileService, serializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext,CancellationToken cancellationToken)
        {
            var albGlobal = await ProvideAsync<FundingService.ALB.FundingOutput.Model.Output.ALBGlobal>(reportServiceContext.FundingALBOutputKey, reportServiceContext.Container, cancellationToken) as FundingService.ALB.FundingOutput.Model.Output.ALBGlobal;

            return MapData(albGlobal);
        }

        private ALBGlobal MapData(FundingService.ALB.FundingOutput.Model.Output.ALBGlobal albGlobal)
        {
            return MapAlbGlobal(albGlobal);
        }

        private ALBGlobal MapAlbGlobal(FundingService.ALB.FundingOutput.Model.Output.ALBGlobal albGlobal)
        {
            return new ALBGlobal()
            {
                Learners = albGlobal.Learners?.Select(MapAlbLearner).ToList()
            };
        }

        private ALBLearner MapAlbLearner(FundingService.ALB.FundingOutput.Model.Output.ALBLearner albLearner)
        {
            return new ALBLearner()
            {
                LearnRefNumber = albLearner.LearnRefNumber,
                LearningDeliveries = albLearner.LearningDeliveries?.Select(MapAlbLearningDelivery).ToList(),
            };
        }

        private LearningDelivery MapAlbLearningDelivery(FundingService.ALB.FundingOutput.Model.Output.LearningDelivery albLearningDelivery)
        {
            return new LearningDelivery()
            {
                AimSeqNumber = albLearningDelivery.AimSeqNumber,
                LearningDeliveryValue = MapAlbLearningDeliveryValue(albLearningDelivery.LearningDeliveryValue),
                LearningDeliveryPeriodisedValues = albLearningDelivery.LearningDeliveryPeriodisedValues.Select(MapAlbLearningDeliveryPeriodisedValue).ToList()
            };
        }

        private LearningDeliveryValue MapAlbLearningDeliveryValue(FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryValue albLearningDeliveryValue)
        {
            return new LearningDeliveryValue()
            {
                ApplicFactDate = albLearningDeliveryValue.ApplicFactDate,
                ApplicProgWeightFact = albLearningDeliveryValue.ApplicProgWeightFact,
                AreaCostFactAdj = albLearningDeliveryValue.AreaCostFactAdj,
                FundLine = albLearningDeliveryValue.FundLine,
                LiabilityDate = albLearningDeliveryValue.LiabilityDate,
                PlannedNumOnProgInstalm = albLearningDeliveryValue.PlannedNumOnProgInstalm,
                WeightedRate = albLearningDeliveryValue.WeightedRate,
            };
        }

        private LearningDeliveryPeriodisedValue MapAlbLearningDeliveryPeriodisedValue(FundingService.ALB.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue albLearningDeliveryPeriodisedValue)
        {
            return new LearningDeliveryPeriodisedValue()
            {
                AttributeName = albLearningDeliveryPeriodisedValue.AttributeName,
                Period1 = albLearningDeliveryPeriodisedValue.Period1,
                Period2 = albLearningDeliveryPeriodisedValue.Period2,
                Period3 = albLearningDeliveryPeriodisedValue.Period3,
                Period4 = albLearningDeliveryPeriodisedValue.Period4,
                Period5 = albLearningDeliveryPeriodisedValue.Period5,
                Period6 = albLearningDeliveryPeriodisedValue.Period6,
                Period7 = albLearningDeliveryPeriodisedValue.Period7,
                Period8 = albLearningDeliveryPeriodisedValue.Period8,
                Period9 = albLearningDeliveryPeriodisedValue.Period9,
                Period10 = albLearningDeliveryPeriodisedValue.Period10,
                Period11 = albLearningDeliveryPeriodisedValue.Period11,
                Period12 = albLearningDeliveryPeriodisedValue.Period12
            };
        }
    }
}
