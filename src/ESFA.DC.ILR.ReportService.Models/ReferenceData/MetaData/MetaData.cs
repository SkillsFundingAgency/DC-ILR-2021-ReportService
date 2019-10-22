using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData
{
    public class MetaData
    {
        public DateTime DateGenerated { get; set; }

    public ReferenceDataVersion ReferenceDataVersions { get; set; }

    public IReadOnlyCollection<ValidationError> ValidationErrors { get; set; }

    public IlrCollectionDates CollectionDates { get; set; }
    }
}
