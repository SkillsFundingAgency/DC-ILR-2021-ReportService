using ESFA.DC.ILR.FundingService.FM25.Model.Output;

namespace ESFA.DC.ILR1819.ReportService.Tests.Helpers
{
    public static class TestFM25Builder
    {
        public static Learner Build()
        {
            return new Learner
            {
                AcadMonthPayment = 0,
                AcadProg = false,
                ActualDaysILCurrYear = 4,
                AreaCostFact1618Hist = 12M,
                Block1DisadvUpliftNew = 0,
                Block2DisadvElementsNew = 0,
                ConditionOfFundingEnglish = string.Empty,
                ConditionOfFundingMaths = string.Empty,
                CoreAimSeqNumber = 0,
                FullTimeEquiv = 0
            };
        }
    }
}
