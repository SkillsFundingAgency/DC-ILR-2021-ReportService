using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.FRM;
using ESFA.DC.ILR.ReportService.Reports.Constants;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public abstract class FrmBaseModelBuilder
    {
        protected const string ADLLearnDelFamType = "ADL";
        protected const string SOFLearnDelFamType = "SOF";
        protected const string RESLearnDelFamType = "RES";

        protected readonly HashSet<string> DevolvedCodes = new HashSet<string> { "110", "111", "112", "113", "114", "115", "116", "117" };
        private readonly HashSet<int> _apprenticeshipHashSet = new HashSet<int> { 2, 3, 20, 21, 22, 23, 25 };

        protected string RetrieveFamCodeForType(IEnumerable<LearningDeliveryFAM> deliveryFams, string learnDelFamType)
        {
            return deliveryFams.FirstOrDefault(f => f.LearnDelFAMType == learnDelFamType)?.LearnDelFAMCode ?? string.Empty;
        }

        protected string RetrieveFamCodeForType(IEnumerable<ILR.Model.Interface.ILearningDeliveryFAM> deliveryFams, string learnDelFamType)
        {
            return deliveryFams.FirstOrDefault(f => f.LearnDelFAMType == learnDelFamType)?.LearnDelFAMCode ?? string.Empty;
        }

        protected string ProviderSpecDeliveryMonitorings(IReadOnlyCollection<IProviderSpecDeliveryMonitoring> providerSpecDeliveryMonitorings)
        {
            if (providerSpecDeliveryMonitorings == null || !providerSpecDeliveryMonitorings.Any())
            {
                return null;
            }

            return string.Join(";", providerSpecDeliveryMonitorings?.Select(x => x.ProvSpecDelMon));
        }

        protected string ProviderSpecLearningMonitorings(IReadOnlyCollection<IProviderSpecLearnerMonitoring> providerSpecLearnerMonitorings)
        {
            if (providerSpecLearnerMonitorings == null || !providerSpecLearnerMonitorings.Any())
            {
                return null;
            }

            return string.Join(";", providerSpecLearnerMonitorings?.Select(x => x.ProvSpecLearnMon));
        }
    }
}
