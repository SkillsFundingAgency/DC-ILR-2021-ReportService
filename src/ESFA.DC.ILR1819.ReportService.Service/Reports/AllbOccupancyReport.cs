using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using CsvHelper;
using ESFA.DC.ILR.FundingService.ALB.FundingOutput.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR1819.ReportService.Interface;
using ESFA.DC.ILR1819.ReportService.Interface.Reports;
using ESFA.DC.ILR1819.ReportService.Interface.Service;
using ESFA.DC.ILR1819.ReportService.Service.Model;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR1819.ReportService.Service.Reports
{
    public sealed class AllbOccupancyReport : IAllbOccupancyReport
    {
        private const string AlbCode = "ALBCode";

        private const string AlbSupportPayment = "ALBSupportPayment";

        private const string AlbAreaUpliftBalPayment = "AreaUpliftBalPayment";

        private const string AlbAreaUpliftOnProgPayment = "AreaUpliftOnProgPayment";

        private readonly ILogger _logger;
        private readonly IKeyValuePersistenceService _storage;
        private readonly IKeyValuePersistenceService _redis;
        private readonly IXmlSerializationService _xmlSerializationService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IIlrProviderService _ilrProviderService;

        public AllbOccupancyReport(
            ILogger logger,
            [KeyFilter(PersistenceStorageKeys.Blob)] IKeyValuePersistenceService storage,
            [KeyFilter(PersistenceStorageKeys.Redis)] IKeyValuePersistenceService redis,
            IXmlSerializationService xmlSerializationService,
            IJsonSerializationService jsonSerializationService,
            IIlrProviderService ilrProviderService)
        {
            _logger = logger;
            _storage = storage;
            _redis = redis;
            _xmlSerializationService = xmlSerializationService;
            _jsonSerializationService = jsonSerializationService;
            _ilrProviderService = ilrProviderService;
        }

        public async Task GenerateReport(IJobContextMessage jobContextMessage)
        {
            await _storage.SaveAsync("ALLB_Occupancy_Report.csv", await GetCsv(jobContextMessage));
        }

        private async Task<string> GetCsv(IJobContextMessage jobContextMessage)
        {
            var ilrFileTask = _ilrProviderService.GetIlrFile(jobContextMessage);
            var albDataTask = ReadAndDeserialiseAlbAsync(jobContextMessage);
            var validLearnersTask = ReadAndDeserialiseValidLearnersAsync(jobContextMessage);
            await Task.WhenAll(ilrFileTask, albDataTask, validLearnersTask);
            bool wasError = false;

            List<AllbOccupancyModel> models = new List<AllbOccupancyModel>(validLearnersTask.Result.Count);
            foreach (string validLearnerRefNum in validLearnersTask.Result)
            {
                var learner = ilrFileTask.Result.Learners?.SingleOrDefault(x => x.LearnRefNumber == validLearnerRefNum);
                if (learner == null)
                {
                    wasError = true;
                    continue;
                }

                foreach (ILearningDelivery learningDelivery in learner.LearningDeliveries)
                {
                    var albLearner =
                        albDataTask.Result.Learners.SingleOrDefault(x => x.LearnRefNumber == validLearnerRefNum);
                    var albAttribs = albLearner?.LearningDeliveryAttributes
                        .SingleOrDefault(x => x.AimSeqNumber == learningDelivery.AimSeqNumber)
                        ?.LearningDeliveryAttributeDatas;
                    var albSupportPaymentObj =
                        albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbSupportPayment);
                    var albAreaUpliftOnProgPaymentObj =
                        albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbAreaUpliftOnProgPayment);
                    var albAreaUpliftBalPaymentObj =
                        albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbAreaUpliftBalPayment);

                    var alb = learningDelivery.LearningDeliveryFAMs.SingleOrDefault(x => x.LearnDelFAMType == "ALB");

                    models.Add(new AllbOccupancyModel()
                    {
                        LearnRefNumber = learner.LearnRefNumber,
                        Uln = learner.ULN,
                        DateOfBirth = learner.DateOfBirthNullable,
                        PreMergerUkprn = learner.PrevUKPRNNullable,
                        CampId = learner.CampId,
                        ProvSpecLearnMonA = learner.ProviderSpecLearnerMonitorings
                            ?.FirstOrDefault(x => x.ProvSpecLearnMonOccur == "A")?.ProvSpecLearnMon,
                        ProvSpecLearnMonB = learner.ProviderSpecLearnerMonitorings
                            ?.FirstOrDefault(x => x.ProvSpecLearnMonOccur == "B")?.ProvSpecLearnMon,
                        AimSeqNumber = learningDelivery.AimSeqNumber,
                        LearnAimRef = learningDelivery.LearnAimRef,
                        LearnAimRefTitle = "Todo",
                        SwSupAimId = learningDelivery.SWSupAimId,
                        WeightedRate = albAttribs?.WeightedRate,
                        ApplicProgWeightFact = albAttribs?.ApplicProgWeightFact,
                        NotionalNvqLevelV2 = 1, // Lars
                        SectorSubjectAreaTier2 = 1, // Lars
                        AimType = learningDelivery.AimType,
                        FundingModel = learningDelivery.FundModel,
                        PriorLearnFundAdj = learningDelivery.PriorLearnFundAdjNullable,
                        OtherFundAdj = learningDelivery.OtherFundAdjNullable,
                        OrigLearnStartDate = learningDelivery.OrigLearnStartDateNullable,
                        LearnStartDate = learningDelivery.LearnStartDate,
                        LearnPlanEndDate = learningDelivery.LearnPlanEndDate,
                        CompStatus = learningDelivery.CompStatus,
                        LearnActEndDate = learningDelivery.LearnActEndDateNullable,
                        Outcome = learningDelivery.OutcomeNullable,
                        LearnDelFamCodeAdl = learningDelivery.LearningDeliveryFAMs.SingleOrDefault(x => x.LearnDelFAMType == "ADL")?.LearnDelFAMCode,
                        AlbBursaryFunding = alb?.LearnDelFAMCode,
                        AlbDateFrom = alb?.LearnDelFAMDateFromNullable,
                        AlbDateTo = alb?.LearnDelFAMDateToNullable,
                        LearnDelMonA = learningDelivery.LearningDeliveryFAMs.SingleOrDefault(x => x.LearnDelFAMType == "LDM1")?.LearnDelFAMCode,
                        LearnDelMonB = learningDelivery.LearningDeliveryFAMs.SingleOrDefault(x => x.LearnDelFAMType == "LDM2")?.LearnDelFAMCode,
                        LearnDelMonC = learningDelivery.LearningDeliveryFAMs.SingleOrDefault(x => x.LearnDelFAMType == "LDM3")?.LearnDelFAMCode,
                        LearnDelMonD = learningDelivery.LearningDeliveryFAMs.SingleOrDefault(x => x.LearnDelFAMType == "LDM4")?.LearnDelFAMCode,
                        ProvSpecDelMonA = learningDelivery.ProviderSpecDeliveryMonitorings.SingleOrDefault(x => x.ProvSpecDelMonOccur == "A")?.ProvSpecDelMon,
                        ProvSpecDelMonB = learningDelivery.ProviderSpecDeliveryMonitorings.SingleOrDefault(x => x.ProvSpecDelMonOccur == "B")?.ProvSpecDelMon,
                        ProvSpecDelMonC = learningDelivery.ProviderSpecDeliveryMonitorings.SingleOrDefault(x => x.ProvSpecDelMonOccur == "C")?.ProvSpecDelMon,
                        ProvSpecDelMonD = learningDelivery.ProviderSpecDeliveryMonitorings.SingleOrDefault(x => x.ProvSpecDelMonOccur == "D")?.ProvSpecDelMon,
                        PartnerUkprn = learningDelivery.PartnerUKPRNNullable,
                        DelLocPostCode = learningDelivery.DelLocPostCode,
                        AreaCodeFactAdj = albAttribs?.AreaCostFactAdj,
                        FundLine = albAttribs?.FundLine,
                        LiabilityDate = albAttribs?.LiabilityDate,
                        PlannedNumOnProgInstalm = albAttribs?.PlannedNumOnProgInstalm,
                        ApplicFactDate = albAttribs?.ApplicFactDate,
                        Period1AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period1,
                        Period1AlbPayment = albSupportPaymentObj?.Period1,
                        Period1AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period1,
                        Period1AlbAreaUplift = albAreaUpliftBalPaymentObj?.Period1,
                        Period1AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period1),
                        Period2AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period2,
                        Period2AlbPayment = albSupportPaymentObj?.Period2,
                        Period2AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period2,
                        Period2AlbBalPayment = albAreaUpliftBalPaymentObj?.Period2,
                        Period2AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period2),
                        Period3AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period3,
                        Period3AlbPayment = albSupportPaymentObj?.Period3,
                        Period3AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period3,
                        Period3AlbBalPayment = albAreaUpliftBalPaymentObj?.Period3,
                        Period3AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period3),
                        Period4AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period4,
                        Period4AlbPayment = albSupportPaymentObj?.Period4,
                        Period4AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period4,
                        Period4AlbBalPayment = albAreaUpliftBalPaymentObj?.Period4,
                        Period4AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period4),
                        Period5AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period5,
                        Period5AlbPayment = albSupportPaymentObj?.Period5,
                        Period5AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period5,
                        Period5AlbBalPayment = albAreaUpliftBalPaymentObj?.Period5,
                        Period5AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period5),
                        Period6AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period6,
                        Period6AlbPayment = albSupportPaymentObj?.Period6,
                        Period6AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period6,
                        Period6AlbBalPayment = albAreaUpliftBalPaymentObj?.Period6,
                        Period6AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period6),
                        Period7AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period7,
                        Period7AlbPayment = albSupportPaymentObj?.Period7,
                        Period7AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period7,
                        Period7AlbBalPayment = albAreaUpliftBalPaymentObj?.Period7,
                        Period7AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period7),
                        Period8AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period8,
                        Period8AlbPayment = albSupportPaymentObj?.Period8,
                        Period8AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period8,
                        Period8AlbBalPayment = albAreaUpliftBalPaymentObj?.Period8,
                        Period8AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period8),
                        Period9AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period9,
                        Period9AlbPayment = albSupportPaymentObj?.Period9,
                        Period9AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period9,
                        Period9AlbBalPayment = albAreaUpliftBalPaymentObj?.Period9,
                        Period9AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period9),
                        Period10AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period10,
                        Period10AlbPayment = albSupportPaymentObj?.Period10,
                        Period10AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period10,
                        Period10AlbBalPayment = albAreaUpliftBalPaymentObj?.Period10,
                        Period10AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period10),
                        Period11AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period11,
                        Period11AlbPayment = albSupportPaymentObj?.Period11,
                        Period11AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period11,
                        Period11AlbBalPayment = albAreaUpliftBalPaymentObj?.Period11,
                        Period11AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period11),
                        Period12AlbCode = albLearner?.LearnerPeriodisedAttributes.SingleOrDefault(x => x.AttributeName == AlbCode)?.Period12,
                        Period12AlbPayment = albSupportPaymentObj?.Period12,
                        Period12AlbOnProgPayment = albAreaUpliftOnProgPaymentObj?.Period12,
                        Period12AlbBalPayment = albAreaUpliftBalPaymentObj?.Period12,
                        Period12AlbTotal = albLearner?.LearnerPeriodisedAttributes.Where(x => x.AttributeName == AlbSupportPayment || x.AttributeName == AlbAreaUpliftOnProgPayment || x.AttributeName == AlbAreaUpliftBalPayment).Sum(x => x.Period12),
                        TotalAlbSupportPayment = (albSupportPaymentObj?.Period1 ?? 0) + (albSupportPaymentObj?.Period2 ?? 0) + (albSupportPaymentObj?.Period3 ?? 0) + (albSupportPaymentObj?.Period4 ?? 0) + (albSupportPaymentObj?.Period5 ?? 0) + (albSupportPaymentObj?.Period6 ?? 0) + (albSupportPaymentObj?.Period7 ?? 0)
                                                + (albSupportPaymentObj?.Period8 ?? 0) + (albSupportPaymentObj?.Period9 ?? 0) + (albSupportPaymentObj?.Period10 ?? 0) + (albSupportPaymentObj?.Period11 ?? 0) + (albSupportPaymentObj?.Period12 ?? 0),
                        TotalAlbAreaUplift = (albAreaUpliftOnProgPaymentObj?.Period1 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period2 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period3 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period4 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period5 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period6 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period7 ?? 0)
                                            + (albAreaUpliftOnProgPaymentObj?.Period8 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period9 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period10 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period11 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period12 ?? 0),
                        TotalAlbBalPayment = (albAreaUpliftBalPaymentObj?.Period1 ?? 0) + (albAreaUpliftBalPaymentObj?.Period2 ?? 0) + (albAreaUpliftBalPaymentObj?.Period3 ?? 0) + (albAreaUpliftBalPaymentObj?.Period4 ?? 0) + (albAreaUpliftBalPaymentObj?.Period5 ?? 0) + (albAreaUpliftBalPaymentObj?.Period6 ?? 0) + (albAreaUpliftBalPaymentObj?.Period7 ?? 0)
                                            + (albAreaUpliftBalPaymentObj?.Period8 ?? 0) + (albAreaUpliftBalPaymentObj?.Period9 ?? 0) + (albAreaUpliftBalPaymentObj?.Period10 ?? 0) + (albAreaUpliftBalPaymentObj?.Period11 ?? 0) + (albAreaUpliftBalPaymentObj?.Period12 ?? 0),
                        TotalEarnedCash = (albSupportPaymentObj?.Period1 ?? 0) + (albSupportPaymentObj?.Period2 ?? 0) + (albSupportPaymentObj?.Period3 ?? 0) + (albSupportPaymentObj?.Period4 ?? 0) + (albSupportPaymentObj?.Period5 ?? 0) + (albSupportPaymentObj?.Period6 ?? 0) + (albSupportPaymentObj?.Period7 ?? 0)
                                          + (albSupportPaymentObj?.Period8 ?? 0) + (albSupportPaymentObj?.Period9 ?? 0) + (albSupportPaymentObj?.Period10 ?? 0) + (albSupportPaymentObj?.Period11 ?? 0) + (albSupportPaymentObj?.Period12 ?? 0)
                                          + (albAreaUpliftOnProgPaymentObj?.Period1 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period2 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period3 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period4 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period5 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period6 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period7 ?? 0)
                                          + (albAreaUpliftOnProgPaymentObj?.Period8 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period9 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period10 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period11 ?? 0) + (albAreaUpliftOnProgPaymentObj?.Period12 ?? 0)
                                          + (albAreaUpliftBalPaymentObj?.Period1 ?? 0) + (albAreaUpliftBalPaymentObj?.Period2 ?? 0) + (albAreaUpliftBalPaymentObj?.Period3 ?? 0) + (albAreaUpliftBalPaymentObj?.Period4 ?? 0) + (albAreaUpliftBalPaymentObj?.Period5 ?? 0) + (albAreaUpliftBalPaymentObj?.Period6 ?? 0) + (albAreaUpliftBalPaymentObj?.Period7 ?? 0)
                                          + (albAreaUpliftBalPaymentObj?.Period8 ?? 0) + (albAreaUpliftBalPaymentObj?.Period9 ?? 0) + (albAreaUpliftBalPaymentObj?.Period10 ?? 0) + (albAreaUpliftBalPaymentObj?.Period11 ?? 0) + (albAreaUpliftBalPaymentObj?.Period12 ?? 0)
                    });
                }
            }

            if (wasError)
            {
                _logger.LogWarning("Failed to get one or more learners from ILR while generating Allb Occupancy Report");
            }

            StringBuilder sb = new StringBuilder();

            using (TextWriter textWriter = new StringWriter(sb))
            {
                using (CsvWriter csvWriter = new CsvWriter(textWriter))
                {
                    // csvWriter.Configuration.RegisterClassMap<ValidationErrorMapper>();
                    csvWriter.WriteHeader<AllbOccupancyModel>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecords(models);
                }
            }

            return sb.ToString();
        }

        private async Task<FundingOutputs> ReadAndDeserialiseAlbAsync(IJobContextMessage jobContextMessage)
        {
            FundingOutputs fundingOutputs = null;

            try
            {
                string albFilename = jobContextMessage.KeyValuePairs[JobContextMessageKey.FundingAlbOutput].ToString();
                string alb = await _redis.GetAsync(albFilename);

                fundingOutputs = _jsonSerializationService.Deserialize<FundingOutputs>(alb);
            }
            catch (Exception ex)
            {
                // Todo: Check behaviour
                _logger.LogError("Failed to get & deserialise ALB funding data", ex);
            }

            return fundingOutputs;
        }

        private async Task<List<string>> ReadAndDeserialiseValidLearnersAsync(IJobContextMessage jobContextMessage)
        {
            string learnersValidStr = await _redis.GetAsync(jobContextMessage.KeyValuePairs[JobContextMessageKey.ValidLearnRefNumbers].ToString());
            List<string> validLearners = _jsonSerializationService.Deserialize<List<string>>(learnersValidStr);
            return validLearners;
        }
    }
}
