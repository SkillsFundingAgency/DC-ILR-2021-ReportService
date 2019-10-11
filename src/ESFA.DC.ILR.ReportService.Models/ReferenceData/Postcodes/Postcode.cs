using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.Postcodes
{
    public class Postcode
    {
        public string PostCode { get; set; }

        public List<ONSData> ONSData { get; set; }

        public List<DasDisadvantage> DasDisadvantages { get; set; }

        public List<EfaDisadvantage> EfaDisadvantages { get; set; }

        public List<SfaAreaCost> SfaAreaCosts { get; set; }

        public List<SfaDisadvantage> SfaDisadvantages { get; set; }

        public List<McaglaSOF> McaglaSOFs { get; set; }
    }
}
