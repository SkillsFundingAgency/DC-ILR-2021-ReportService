using System;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData
{
    public struct ReturnPeriod
    {
        public string Name { get; set; }

        public int Period { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
