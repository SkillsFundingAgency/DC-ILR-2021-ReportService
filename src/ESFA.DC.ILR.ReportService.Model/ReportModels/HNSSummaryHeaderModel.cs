using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Model.ReportModels
{
    public sealed class HNSSummaryHeaderModel
    {
        public string ProviderName { get; set; }

        public int Ukprn { get; set; }

        public string IlrFile { get; set; }

        public string Year { get; set; }
    }
}
