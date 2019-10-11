using System;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Postcodes
{
    public class ONSData : AbstractTimeBoundedEntity
    {
        public DateTime? Termination { get; set; }

        public string LocalAuthority { get; set; }

        public string Lep1 { get; set; }

        public string Lep2 { get; set; }

        public string Nuts { get; set; }
    }
}
