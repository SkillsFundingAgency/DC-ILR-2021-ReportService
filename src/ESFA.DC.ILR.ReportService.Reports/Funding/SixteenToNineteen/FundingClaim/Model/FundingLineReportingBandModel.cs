namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model
{
    public class FundingLineReportingBandModel
    {
        public int Band9StudentNumbers { get; set; }
        public int Band8StudentNumbers { get; set; }
        public int Band7StudentNumbers { get; set; }
        public int Band6StudentNumbers { get; set; }
        public int Band5StudentNumbers { get; set; }
        public int Band4aStudentNumbers { get; set; }
        public int Band4bStudentNumbers { get; set; }
        public int Band3StudentNumbers { get; set; }
        public int Band2StudentNumbers { get; set; }
        public int Band1StudentNumbers { get; set; }
        public int Band5StudentNumbersTLevel { get; set; }
        public int Band4aStudentNumbersTLevel { get; set; }
        public int Band4bStudentNumbersTLevel { get; set; }
        public int Band3StudentNumbersTLevel { get; set; }
        public int Band2StudentNumbersTLevel { get; set; }
        public int Band1StudentNumbersTLevel { get; set; }

        public decimal Band9TotalFunding { get; set; }
        public decimal Band8TotalFunding { get; set; }
        public decimal Band7TotalFunding { get; set; }
        public decimal Band6TotalFunding { get; set; }
        public decimal Band5TotalFunding { get; set; }
        public decimal Band4aTotalFunding { get; set; }
        public decimal Band4bTotalFunding { get; set; }
        public decimal Band3TotalFunding { get; set; }
        public decimal Band2TotalFunding { get; set; }
        public decimal Band1TotalFunding { get; set; }
        public decimal Band5TotalFundingTLevel { get; set; }
        public decimal Band4aTotalFundingTLevel { get; set; }
        public decimal Band4bTotalFundingTLevel { get; set; }
        public decimal Band3TotalFundingTLevel { get; set; }
        public decimal Band2TotalFundingTLevel { get; set; }
        public decimal Band1TotalFundingTLevel { get; set; }

        public decimal Band1To5TotalFundingTLevel => Band1TotalFundingTLevel + Band2TotalFundingTLevel + Band3TotalFundingTLevel +
                                               Band4aTotalFundingTLevel + Band4bTotalFundingTLevel + Band5TotalFundingTLevel;

        public int Band1To5StudentNumbersTLevel => Band1StudentNumbersTLevel + Band2StudentNumbersTLevel + Band3StudentNumbersTLevel +
                                             Band4aStudentNumbersTLevel + Band4bStudentNumbersTLevel + Band5StudentNumbersTLevel;
    }
}
