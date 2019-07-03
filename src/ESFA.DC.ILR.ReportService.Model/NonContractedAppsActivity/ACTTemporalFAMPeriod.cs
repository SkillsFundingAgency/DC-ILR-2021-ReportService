namespace ESFA.DC.ILR.ReportService.Model.NonContractedAppsActivity
{
    public class ACTTemporalFAMPeriod
    {
        public ACTTemporalFAMPeriod()
        {
            Periods = new decimal[12];
        }

        public NonContractedAppsActivityLearningDeliveryFAMInfo FamInfo { get; set; }

        public decimal[] Periods { get; set; }

        public string FundingLineType { get; set; }
    }
}
