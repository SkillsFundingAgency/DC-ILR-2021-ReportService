﻿using System;

namespace ESFA.DC.ILR1819.ReportService.Model.PeriodEnd.AppsCoInvestment
{
    public class AppFinRecordInfo
    {
        public string LearnRefNumber { get; set; }

        public int AimSeqNumber { get; set; }

        public string AFinType { get; set; }

        public int AFinCode { get; set; }

        public DateTime AFinDate { get; set; }

        public int AFinAmount { get; set; }
        
    }
}