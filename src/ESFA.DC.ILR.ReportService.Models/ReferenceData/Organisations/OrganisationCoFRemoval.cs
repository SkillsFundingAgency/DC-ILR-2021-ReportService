using System;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations
{
    public class OrganisationCoFRemoval : AbstractTimeBoundedEntity
    {
        public Decimal CoFRemoval { get; set; }
    }
}
