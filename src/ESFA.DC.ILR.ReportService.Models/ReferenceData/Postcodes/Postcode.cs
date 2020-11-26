using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Postcodes
{
    public class Postcode
    {
        public string PostCode { get; set; }

        public List<ONSData> ONSData { get; set; }
    }
}
