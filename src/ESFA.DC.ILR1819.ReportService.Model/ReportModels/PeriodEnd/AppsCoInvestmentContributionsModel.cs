namespace ESFA.DC.ILR1819.ReportService.Model.ReportModels.PeriodEnd
{
    public class AppsCoInvestmentContributionsModel
    {
        public string LearnRefNumber { get; set; }

        public long UniqueLearnerNumber { get; set; }

        public int? ProgType { get; set; }

        public string OfficialSensitive { get; }

    }
}