using System;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Postcodes
{
    public class SfaDisadvantage : AbstractTimeBoundedEntity
    {
        public Decimal? Uplift { get; set; }
    }
}
