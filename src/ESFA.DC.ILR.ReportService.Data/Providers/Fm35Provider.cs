using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm35Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public Fm35Provider(
            IFileService fileService, 
            IJsonSerializationService serializationService) 
            : base(fileService, serializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var fm35Global = await ProvideAsync<FundingService.FM35.FundingOutput.Model.Output.FM35Global>(reportServiceContext.FundingFM35OutputKey, reportServiceContext.Container, cancellationToken) as FundingService.FM35.FundingOutput.Model.Output.FM35Global;

            return MapData(fm35Global);
        }

        private FM35Global MapData(FundingService.FM35.FundingOutput.Model.Output.FM35Global fm35Global)
        {
            return MapFm35Global(fm35Global);
        }

        private FM35Global MapFm35Global(FundingService.FM35.FundingOutput.Model.Output.FM35Global fm35Global)
        {
            return new FM35Global()
            {
                Learners = fm35Global.Learners?.Select(MapFm35Learner).ToList()
            };
        }

        private FM35Learner MapFm35Learner(FundingService.FM35.FundingOutput.Model.Output.FM35Learner fm35Learner)
        {
            return new FM35Learner()
            {
                LearnRefNumber = fm35Learner.LearnRefNumber,
                LearningDeliveries = fm35Learner.LearningDeliveries?.Select(MapFm35LearningDelivery).ToList()
            };
        }

        private LearningDelivery MapFm35LearningDelivery(FundingService.FM35.FundingOutput.Model.Output.LearningDelivery fm35LearningDelivery)
        {
            return new LearningDelivery()
            {
                AimSeqNumber = fm35LearningDelivery.AimSeqNumber,
                LearningDeliveryValue = MapFm35LearningDeliveryValue(fm35LearningDelivery.LearningDeliveryValue),
                LearningDeliveryPeriodisedValues = fm35LearningDelivery.LearningDeliveryPeriodisedValues?.Select(MapFm35LearningDeliveryPeriodisedValue).ToList()
            };
        }

        private LearningDeliveryValue MapFm35LearningDeliveryValue(FundingService.FM35.FundingOutput.Model.Output.LearningDeliveryValue fm35LearningDeliveryValue)
        {
            return new LearningDeliveryValue()
            {
                AchieveElement = fm35LearningDeliveryValue.AchieveElement,
                AimValue = fm35LearningDeliveryValue.AimValue,
                ApplicEmpFactDate = fm35LearningDeliveryValue.ApplicEmpFactDate,
                ApplicFactDate = fm35LearningDeliveryValue.ApplicFactDate,
                ApplicProgWeightFact = fm35LearningDeliveryValue.ApplicProgWeightFact,
                ApplicWeightFundRate = fm35LearningDeliveryValue.ApplicWeightFundRate,
                AreaCostFactAdj = fm35LearningDeliveryValue.AreaCostFactAdj,
                CapFactor = fm35LearningDeliveryValue.CapFactor,
                DisUpFactAdj = fm35LearningDeliveryValue.DisUpFactAdj,
                FundLine = fm35LearningDeliveryValue.FundLine,
                LargeEmployerID = fm35LearningDeliveryValue.LargeEmployerID,
                LargeEmployerFM35Fctr = fm35LearningDeliveryValue.LargeEmployerFM35Fctr,
                NonGovCont = fm35LearningDeliveryValue.NonGovCont,
                PlannedNumOnProgInstalm = fm35LearningDeliveryValue.PlannedNumOnProgInstalm,
                PlannedNumOnProgInstalmTrans = fm35LearningDeliveryValue.PlannedNumOnProgInstalmTrans,
                PrscHEAim = fm35LearningDeliveryValue.PrscHEAim,
                StartPropTrans = fm35LearningDeliveryValue.StartPropTrans,
                TrnWorkPlaceAim = fm35LearningDeliveryValue.TrnWorkPlaceAim,
                TrnWorkPrepAim = fm35LearningDeliveryValue.TrnWorkPrepAim,
                WeightedRateFromESOL = fm35LearningDeliveryValue.WeightedRateFromESOL,
            };
        }

        private LearningDeliveryPeriodisedValue MapFm35LearningDeliveryPeriodisedValue(FundingService.FM35.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue fm35LearningDeliveryPeriodisedValue)
        {
            return new LearningDeliveryPeriodisedValue()
            {
                AttributeName = fm35LearningDeliveryPeriodisedValue.AttributeName,
                Period1 = fm35LearningDeliveryPeriodisedValue.Period1,
                Period2 = fm35LearningDeliveryPeriodisedValue.Period2,
                Period3 = fm35LearningDeliveryPeriodisedValue.Period3,
                Period4 = fm35LearningDeliveryPeriodisedValue.Period4,
                Period5 = fm35LearningDeliveryPeriodisedValue.Period5,
                Period6 = fm35LearningDeliveryPeriodisedValue.Period6,
                Period7 = fm35LearningDeliveryPeriodisedValue.Period7,
                Period8 = fm35LearningDeliveryPeriodisedValue.Period8,
                Period9 = fm35LearningDeliveryPeriodisedValue.Period9,
                Period10 = fm35LearningDeliveryPeriodisedValue.Period10,
                Period11 = fm35LearningDeliveryPeriodisedValue.Period11,
                Period12 = fm35LearningDeliveryPeriodisedValue.Period12
            };
        }
    }
}
