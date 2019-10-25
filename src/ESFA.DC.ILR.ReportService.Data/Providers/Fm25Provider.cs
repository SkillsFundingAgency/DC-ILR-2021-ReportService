using System.Linq;
using ESFA.DC.ILR.ReportService.Data.Providers.Abstract;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ReportService.Data.Providers
{
    public class Fm25Provider : AbstractFileServiceProvider, IExternalDataProvider
    {
        public Fm25Provider(
            IFileService fileService, 
            IJsonSerializationService serializationService) 
            : base(fileService, serializationService)
        {
        }

        public async Task<object> ProvideAsync(IReportServiceContext reportServiceContext, CancellationToken cancellationToken)
        {
            var fm25Global = await ProvideAsync<FundingService.FM25.Model.Output.FM25Global>(reportServiceContext.FundingFM25OutputKey, reportServiceContext.Container, cancellationToken) as FundingService.FM25.Model.Output.FM25Global;

            return MapData(fm25Global);
        }

        private FM25Global MapData(FundingService.FM25.Model.Output.FM25Global fm25Global)
        {
            return MapFm25Global(fm25Global);
        }

        private FM25Global MapFm25Global(FundingService.FM25.Model.Output.FM25Global fm25Global)
        {
            return new FM25Global()
            {
                Learners = fm25Global.Learners?.Select(MapFm25Learner).ToList()
            };
        }

        private FM25Learner MapFm25Learner(FundingService.FM25.Model.Output.FM25Learner learner)
        {
            return new FM25Learner()
            {
                LearnRefNumber = learner.LearnRefNumber,
                OnProgPayment = learner.OnProgPayment,
                AreaCostFact1618Hist = learner.AreaCostFact1618Hist,
                ConditionOfFundingEnglish = learner.ConditionOfFundingEnglish,
                ConditionOfFundingMaths = learner.ConditionOfFundingMaths,
                FundLine = learner.FundLine,
                LearnerActEndDate = learner.LearnerActEndDate,
                LearnerPlanEndDate = learner.LearnerPlanEndDate,
                LearnerStartDate = learner.LearnerStartDate,
                NatRate = learner.NatRate,
                ProgWeightHist = learner.ProgWeightHist,
                PrvDisadvPropnHist = learner.PrvDisadvPropnHist,
                PrvHistLrgProgPropn = learner.PrvHistLrgProgPropn,
                PrvRetentFactHist = learner.PrvRetentFactHist,
                RateBand = learner.RateBand,
                StartFund = learner.StartFund,
                LearnerPeriodisedValues = learner.LearnerPeriodisedValues?.Select(MapLearnerPeriodisedValues).ToList()
            };
        }

        private LearnerPeriodisedValues MapLearnerPeriodisedValues(FundingService.FM25.Model.Output.LearnerPeriodisedValues learnerPeriodisedValues)
        {
            return new LearnerPeriodisedValues()
            {
                LearnRefNumber = learnerPeriodisedValues.LearnRefNumber,
                AttributeName = learnerPeriodisedValues.AttributeName,
                Period1 = learnerPeriodisedValues.Period1,
                Period2 = learnerPeriodisedValues.Period2,
                Period3 = learnerPeriodisedValues.Period3,
                Period4 = learnerPeriodisedValues.Period4,
                Period5 = learnerPeriodisedValues.Period5,
                Period6 = learnerPeriodisedValues.Period6,
                Period7 = learnerPeriodisedValues.Period7,
                Period8 = learnerPeriodisedValues.Period8,
                Period9 = learnerPeriodisedValues.Period9,
                Period10 = learnerPeriodisedValues.Period10,
                Period11 = learnerPeriodisedValues.Period11,
                Period12 = learnerPeriodisedValues.Period12
            };
        }

    }
}
