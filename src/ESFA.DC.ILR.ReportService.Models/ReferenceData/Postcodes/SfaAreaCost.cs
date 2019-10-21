using System;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Postcodes
{
    public class SfaAreaCost : AbstractTimeBoundedEntity
    {
        public Decimal AreaCostFactor { get; set; }
    }
}
