using System;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS.Abstract
{
    public class AbstractLARSValidity : AbstractTimeBoundedEntity
    {
        public string ValidityCategory { get; set; }

        public DateTime StartDate
        {
            get
            {
                return this.EffectiveFrom;
            }
        }

        public DateTime? LastNewStartDate { get; set; }

        public DateTime? EndDate
        {
            get
            {
                return this.EffectiveTo;
            }
        }
    }
}
