﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReportService.Interface.Configuration;

namespace ESFA.DC.ILR.ReportService.Stateless.Configuration
{
    public class ReportServiceConfiguration : IReportServiceConfiguration
    {
        public string DasCommitmentsConnectionString { get; set; }

        public string DASPaymentsConnectionString { get; set; }

        public string ILRDataStoreConnectionString { get; set; }

        public string ILRDataStoreValidConnectionString { get; set; }

        public string EasConnectionString { get; set; }

        public string FCSConnectionString { get; set; }

        public string IlrValidationErrorsConnectionString { get; set; }

        public string LargeEmployerConnectionString { get; set; }

        public string LarsConnectionString { get; set; }

        public string OrgConnectionString { get; set; }

        public string PostcodeConnectionString { get; set; }
    }
}
