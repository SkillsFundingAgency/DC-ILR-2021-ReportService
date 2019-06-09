﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Interface.Configuration
{
    public interface IReportServiceConfiguration
    {
        string DasCommitmentsConnectionString { get; set; }

        string DASPaymentsConnectionString { get; set; }

        string ILRDataStoreConnectionString { get; set; }

        string ILRDataStoreValidConnectionString { get; set; }

        string EasConnectionString { get; set; }

        string FCSConnectionString { get; set; }

        string IlrValidationErrorsConnectionString { get; set; }

        string LargeEmployerConnectionString { get; set; }

        string LarsConnectionString { get; set; }

        string OrgConnectionString { get; set; }

        string PostcodeConnectionString { get; set; }
    }
}
