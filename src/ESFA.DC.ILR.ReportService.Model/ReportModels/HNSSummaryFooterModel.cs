using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Model.ReportModels
{
    public sealed class HNSSummaryFooterModel
    {
        public string ComponentSetVersion { get; set; }

        public string ApplicationVersion { get; set; }

        public string FilePreparationDate { get; set; }

        public string LarsData { get; set; }

        public string PostcodeData { get; set; }

        public string OrganisationData { get; set; }

        public string LargeEmployerData { get; set; }

        public string ReportGeneratedAt { get; set; }
    }
}
