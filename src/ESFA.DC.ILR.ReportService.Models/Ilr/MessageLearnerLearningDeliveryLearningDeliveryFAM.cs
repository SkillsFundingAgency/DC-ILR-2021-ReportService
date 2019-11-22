using System;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Models.Ilr
{
    public class MessageLearnerLearningDeliveryLearningDeliveryFAM : ILearningDeliveryFAM
    {
        public string LearnDelFAMType { get; set; }

        public string LearnDelFAMCode { get; set; }

        public DateTime? LearnDelFAMDateFromNullable => throw new NotImplementedException();

        public DateTime? LearnDelFAMDateToNullable => throw new NotImplementedException();
    }
}
