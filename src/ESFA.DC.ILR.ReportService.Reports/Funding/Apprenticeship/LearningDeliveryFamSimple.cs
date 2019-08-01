using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship
{
    public sealed class LearningDeliveryFamSimple
    {
        public string Code { get; }

        public DateTime From { get; }

        public DateTime To { get; }

        public LearningDeliveryFamSimple(string code, DateTime from, DateTime to)
        {
            Code = code;
            From = from;
            To = to;
        }
    }
}
