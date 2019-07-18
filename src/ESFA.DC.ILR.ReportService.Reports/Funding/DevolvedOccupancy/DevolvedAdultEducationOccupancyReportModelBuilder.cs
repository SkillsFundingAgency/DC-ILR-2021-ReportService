using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CsvHelper.Configuration.Attributes;
using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy.Model;
using ESFA.DC.ILR.ReportService.Service.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy
{
    public class DevolvedAdultEducationOccupancyReportModelBuilder : IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>>
    {
        private readonly IEnumerable<string> _sofLearnDelFamCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.SOF_GreaterManchesterCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_LiverpoolCityRegionCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_WestMidlandsCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_WestOfEnglandCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_TeesValleyCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_CambridgeshireAndPeterboroughCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_GreaterLondonAuthority,
        };

        public IEnumerable<DevolvedAdultEducationOccupancyReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm35 = reportServiceDependentData.Get<FM35Global>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();

            var larsLearningDeliveries = BuildLarsLearningDeliveryDictionary(referenceData);
            var fm35LearningDeliveries = BuildFm35LearningDeliveryDictionary(fm35);

            var models = new List<DevolvedAdultEducationOccupancyReportModel>();

            foreach (var learner in message.Learners)
            {
                foreach (var learningDelivery in learner.LearningDeliveries.Where(LearningDeliveryReportFilter))
                {
                    var fm35LearningDelivery = fm35LearningDeliveries.GetValueOrDefault(learner.LearnRefNumber).GetValueOrDefault(learningDelivery.AimSeqNumber);
                    var larsLearningDelivery = larsLearningDeliveries.GetValueOrDefault(learningDelivery.LearnAimRef);
                    var providerSpecLearnerMonitoring = BuildProviderSpecLearnerMonitoring(learner.ProviderSpecLearnerMonitorings);
                    var providerSpecDeliveryMonitoring = BuildProviderSpecDeliveryMonitoring(learningDelivery.ProviderSpecDeliveryMonitorings);

                    models.Add(new DevolvedAdultEducationOccupancyReportModel()
                    {
                        Learner = learner,
                        ProviderSpecLearnerMonitoring = providerSpecLearnerMonitoring,
                        LearningDelivery = learningDelivery,
                        ProviderSpecDeliveryMonitoring = providerSpecDeliveryMonitoring,
                        Fm35LearningDelivery = fm35LearningDelivery?.LearningDeliveryValue,
                        LarsLearningDelivery = larsLearningDelivery,
                        // devolved
                    });
                }
            }

            return Order(models);
        }

        public bool LearningDeliveryReportFilter(ILearningDelivery learningDelivery)
        {
            return learningDelivery
                .LearningDeliveryFAMs?
                .Any(
                    ldfam => 
                        ldfam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.SOF)
                           && _sofLearnDelFamCodes.Contains(ldfam.LearnDelFAMCode))
                ?? false;
        }

        public IEnumerable<DevolvedAdultEducationOccupancyReportModel> Order(IEnumerable<DevolvedAdultEducationOccupancyReportModel> models)
        {
            return models.OrderBy(m => m.Learner.LearnRefNumber).ThenBy(m => m.LearningDelivery.AimSeqNumber);
        }

        public ProviderSpecLearnerMonitoringModel BuildProviderSpecLearnerMonitoring(IEnumerable<IProviderSpecLearnerMonitoring> monitorings)
        {
            return new ProviderSpecLearnerMonitoringModel()
            {
                A = monitorings.FirstOrDefault(m => m.ProvSpecLearnMonOccur.CaseInsensitiveEquals("A"))?.ProvSpecLearnMon,
                B = monitorings.FirstOrDefault(m => m.ProvSpecLearnMonOccur.CaseInsensitiveEquals("B"))?.ProvSpecLearnMon,
            };
        }

        public ProviderSpecDeliveryMonitoringModel BuildProviderSpecDeliveryMonitoring(IEnumerable<IProviderSpecDeliveryMonitoring> monitorings)
        {
            return new ProviderSpecDeliveryMonitoringModel()
            {
                A = monitorings.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("A"))?.ProvSpecDelMon,
                B = monitorings.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("B"))?.ProvSpecDelMon,
                C = monitorings.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("C"))?.ProvSpecDelMon,
                D = monitorings.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("D"))?.ProvSpecDelMon,
            };
        }

        private IDictionary<string, LARSLearningDelivery> BuildLarsLearningDeliveryDictionary(ReferenceDataRoot referenceDataRoot)
        {
            return referenceDataRoot.LARSLearningDeliveries.ToDictionary(ld => ld.LearnAimRef, ld => ld, StringComparer.OrdinalIgnoreCase);
        }


        private IDictionary<string, Dictionary<int, LearningDelivery>> BuildFm35LearningDeliveryDictionary(FM35Global fm35Global)
        {
            return fm35Global
                .Learners
                .ToDictionary(
                    l => l.LearnRefNumber,
                    l => l.LearningDeliveries
                        .Where(ld => ld.AimSeqNumber.HasValue)
                        .ToDictionary(
                            ld => ld.AimSeqNumber.Value,
                            ld => ld),
                    StringComparer.OrdinalIgnoreCase);
        }
    }
}
