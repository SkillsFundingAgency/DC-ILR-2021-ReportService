using System.Collections.Generic;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS
{
    public class LARSStandard : AbstractTimeBoundedEntity
    {
        public int StandardCode { get; set; }

        public string StandardSectorCode { get; set; }

        public string NotionalEndLevel { get; set; }

        public List<LARSStandardApprenticeshipFunding> LARSStandardApprenticeshipFundings { get; set; }

        public List<LARSStandardCommonComponent> LARSStandardCommonComponents { get; set; }

        public List<LARSStandardFunding> LARSStandardFundings { get; set; }

        public List<LARSStandardValidity> LARSStandardValidities { get; set; }
    }
}
