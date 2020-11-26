using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Postcodes
{
    public class ONSData : AbstractTimeBoundedEntity
    {
        public string LocalAuthority { get; set; }
    }
}
