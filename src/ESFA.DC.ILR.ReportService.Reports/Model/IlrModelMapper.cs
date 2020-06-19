using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Model
{
    public class IlrModelMapper : IIlrModelMapper
    {
        public ProviderSpecLearnerMonitoringModel MapProviderSpecLearnerMonitorings(IEnumerable<IProviderSpecLearnerMonitoring> monitorings)
        {
            return new ProviderSpecLearnerMonitoringModel()
            {
                A = monitorings?.FirstOrDefault(m => m.ProvSpecLearnMonOccur.CaseInsensitiveEquals("A"))?.ProvSpecLearnMon,
                B = monitorings?.FirstOrDefault(m => m.ProvSpecLearnMonOccur.CaseInsensitiveEquals("B"))?.ProvSpecLearnMon,
            };
        }

        public ProviderSpecDeliveryMonitoringModel MapProviderSpecDeliveryMonitorings(IEnumerable<IProviderSpecDeliveryMonitoring> monitorings)
        {
            return new ProviderSpecDeliveryMonitoringModel()
            {
                A = monitorings?.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("A"))?.ProvSpecDelMon,
                B = monitorings?.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("B"))?.ProvSpecDelMon,
                C = monitorings?.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("C"))?.ProvSpecDelMon,
                D = monitorings?.FirstOrDefault(m => m.ProvSpecDelMonOccur.CaseInsensitiveEquals("D"))?.ProvSpecDelMon,
            };
        }

        public LearningDeliveryFAMsModel MapLearningDeliveryFAMs(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            var famDictionary = learningDeliveryFams.GroupBy(fam => fam.LearnDelFAMType).ToDictionary(g => g.Key, g => g.ToArray(), StringComparer.OrdinalIgnoreCase);

            var ldmsArray = famDictionary.GetValueOrDefault(LearningDeliveryFAMTypeConstants.LDM).ToFixedLengthArray(6);
            var damsArray = famDictionary.GetValueOrDefault(LearningDeliveryFAMTypeConstants.DAM).ToFixedLengthArray(6);

            var lsf = famDictionary.GetValueOrDefault(LearningDeliveryFAMTypeConstants.LSF);
            var alb = famDictionary.GetValueOrDefault(LearningDeliveryFAMTypeConstants.ALB);

            return new LearningDeliveryFAMsModel()
            {
                ADL = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.ADL, famDictionary),
                ALB_Highest = alb?.MaxOrDefault(f => f.LearnDelFAMCode),
                ALB_EarliestDateFrom = alb?.MinOrDefault(f => f.LearnDelFAMDateFromNullable),
                ALB_LatestDateTo = alb?.MaxOrDefault(f => f.LearnDelFAMDateToNullable),
                SOF = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.SOF, famDictionary),
                FFI = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.FFI, famDictionary),
                LSF_Highest = lsf?.MaxOrDefault(f => f.LearnDelFAMCode),
                LSF_EarliestDateFrom = lsf?.MinOrDefault(f => f.LearnDelFAMDateFromNullable),
                LSF_LatestDateTo = lsf?.MaxOrDefault(f => f.LearnDelFAMDateToNullable),
                LDM1 = ldmsArray[0]?.LearnDelFAMCode,
                LDM2 = ldmsArray[1]?.LearnDelFAMCode,
                LDM3 = ldmsArray[2]?.LearnDelFAMCode,
                LDM4 = ldmsArray[3]?.LearnDelFAMCode,
                LDM5 = ldmsArray[4]?.LearnDelFAMCode,
                LDM6 = ldmsArray[5]?.LearnDelFAMCode,
                DAM1 = damsArray[0]?.LearnDelFAMCode,
                DAM2 = damsArray[1]?.LearnDelFAMCode,
                DAM3 = damsArray[2]?.LearnDelFAMCode,
                DAM4 = damsArray[3]?.LearnDelFAMCode,
                DAM5 = damsArray[4]?.LearnDelFAMCode,
                DAM6 = damsArray[5]?.LearnDelFAMCode,
                RES = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.RES, famDictionary),
                EEF = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.EEF, famDictionary),
                ASL = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.ASL, famDictionary),
                EII = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.EII, famDictionary),
                HHS1 = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.HHS1, famDictionary),
                HHS2 = GetLearningDeliveryFAMCode(LearningDeliveryFAMTypeConstants.HHS2, famDictionary),
            };
        }

        private string GetLearningDeliveryFAMCode(string famType, IDictionary<string, ILearningDeliveryFAM[]> learningDeliveryFamDictionary)
        {
            return learningDeliveryFamDictionary.GetValueOrDefault(famType)?.FirstOrDefault()?.LearnDelFAMCode;
        }

    }
}
