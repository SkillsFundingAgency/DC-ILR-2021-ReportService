using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm81;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm81Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public Fm81Provider(
            IFileService fileService, 
            IJsonSerializationService serializationService) 
            : base(fileService, serializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var fm81Global = await ProvideAsync<FundingService.FM81.FundingOutput.Model.Output.FM81Global>(reportServiceContext.FundingFM81OutputKey, reportServiceContext.Container, cancellationToken) as FundingService.FM81.FundingOutput.Model.Output.FM81Global;

            return MapData(fm81Global);
        }

        private FM81Global MapData(FundingService.FM81.FundingOutput.Model.Output.FM81Global fm81Global)
        {
            return MapFm81Global(fm81Global);
        }

        private FM81Global MapFm81Global(FundingService.FM81.FundingOutput.Model.Output.FM81Global fm81Global)
        {
            return new FM81Global()
            {
                Learners = fm81Global.Learners?.Select(MapFm81Learner).ToList()
            };
        }

        private FM81Learner MapFm81Learner(FundingService.FM81.FundingOutput.Model.Output.FM81Learner fm81Learner)
        {
            return new FM81Learner()
            {
                LearnRefNumber = fm81Learner.LearnRefNumber,
                LearningDeliveries = fm81Learner.LearningDeliveries?.Select(MapFm81LearningDelivery).ToList()
            };
        }

        private LearningDelivery MapFm81LearningDelivery(FundingService.FM81.FundingOutput.Model.Output.LearningDelivery fm81LearningDelivery)
        {
            return new LearningDelivery()
            {
                AimSeqNumber = fm81LearningDelivery.AimSeqNumber,
                LearningDeliveryValues = MapFm81LearningDeliveryValue(fm81LearningDelivery.LearningDeliveryValues),
                LearningDeliveryPeriodisedValues = fm81LearningDelivery.LearningDeliveryPeriodisedValues?.Select(Mapfm81LearningDeliveryPeriodisedValue).ToList()
            };
        }

        private LearningDeliveryValue MapFm81LearningDeliveryValue(FundingService.FM81.FundingOutput.Model.Output.LearningDeliveryValue fm81LearningDeliveryValue)
        {
            return new LearningDeliveryValue()
            {
                AchApplicDate = fm81LearningDeliveryValue.AchApplicDate,
                AchievementApplicVal = fm81LearningDeliveryValue.AchievementApplicVal,
                AgeStandardStart = fm81LearningDeliveryValue.AgeStandardStart,
                ApplicFundValDate = fm81LearningDeliveryValue.ApplicFundValDate,
                CoreGovContCapApplicVal = fm81LearningDeliveryValue.CoreGovContCapApplicVal,
                EmpIdAchDate = fm81LearningDeliveryValue.EmpIdAchDate,
                EmpIdFirstDayStandard = fm81LearningDeliveryValue.EmpIdFirstDayStandard,
                EmpIdFirstYoungAppDate = fm81LearningDeliveryValue.EmpIdFirstYoungAppDate,
                EmpIdSecondYoungAppDate = fm81LearningDeliveryValue.EmpIdSecondYoungAppDate,
                EmpIdSmallBusDate = fm81LearningDeliveryValue.EmpIdSmallBusDate,
                FundLine = fm81LearningDeliveryValue.FundLine,
                MathEngLSFFundStart = fm81LearningDeliveryValue.MathEngLSFFundStart,
                SmallBusApplicVal = fm81LearningDeliveryValue.SmallBusApplicVal,
                SmallBusEligible = fm81LearningDeliveryValue.SmallBusEligible,
                YoungAppApplicVal = fm81LearningDeliveryValue.YoungAppApplicVal,
                YoungAppEligible = fm81LearningDeliveryValue.YoungAppEligible,
            };
        }

        private LearningDeliveryPeriodisedValue Mapfm81LearningDeliveryPeriodisedValue(FundingService.FM81.FundingOutput.Model.Output.LearningDeliveryPeriodisedValue fm81LearningDeliveryPeriodisedValue)
        {
            return new LearningDeliveryPeriodisedValue()
            {
                AttributeName = fm81LearningDeliveryPeriodisedValue.AttributeName,
                Period1 = fm81LearningDeliveryPeriodisedValue.Period1,
                Period2 = fm81LearningDeliveryPeriodisedValue.Period2,
                Period3 = fm81LearningDeliveryPeriodisedValue.Period3,
                Period4 = fm81LearningDeliveryPeriodisedValue.Period4,
                Period5 = fm81LearningDeliveryPeriodisedValue.Period5,
                Period6 = fm81LearningDeliveryPeriodisedValue.Period6,
                Period7 = fm81LearningDeliveryPeriodisedValue.Period7,
                Period8 = fm81LearningDeliveryPeriodisedValue.Period8,
                Period9 = fm81LearningDeliveryPeriodisedValue.Period9,
                Period10 = fm81LearningDeliveryPeriodisedValue.Period10,
                Period11 = fm81LearningDeliveryPeriodisedValue.Period11,
                Period12 = fm81LearningDeliveryPeriodisedValue.Period12
            };
        }
    }
}
