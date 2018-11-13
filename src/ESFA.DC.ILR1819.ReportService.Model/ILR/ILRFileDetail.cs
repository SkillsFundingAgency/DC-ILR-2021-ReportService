using System;

namespace ESFA.DC.ILR1819.ReportService.Model.ILR
{
    public class ILRFileDetail
    {

        public long ID { get; set; }

        public int UKPRN { get; set; }

        public string Filename { get; set; }

        public DateTime? SubmittedTime { get; set; }

    }
}
