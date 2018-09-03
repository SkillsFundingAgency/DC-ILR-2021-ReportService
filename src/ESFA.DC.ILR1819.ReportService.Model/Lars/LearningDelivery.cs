using System;

namespace ESFA.DC.ILR1819.ReportService.Model.Lars
{
    public sealed class LearningDelivery
    {
        public string LearningDeliveryLearnAimRef { get; }

        public int LearningDeliveryAimSeqNumber { get; }

        public DateTime LearningDeliveryLearnStartDate { get; }

        public int? FrameworkComponentType { get; set; }

        public LearningDelivery(
            string learningDeliveryLearnAimRef,
            int learningDeliveryAimSeqNumber,
            DateTime learningDeliveryLearnStartDate)
        {
            LearningDeliveryLearnAimRef = learningDeliveryLearnAimRef;
            LearningDeliveryAimSeqNumber = learningDeliveryAimSeqNumber;
            LearningDeliveryLearnStartDate = learningDeliveryLearnStartDate;
        }
    }
}
