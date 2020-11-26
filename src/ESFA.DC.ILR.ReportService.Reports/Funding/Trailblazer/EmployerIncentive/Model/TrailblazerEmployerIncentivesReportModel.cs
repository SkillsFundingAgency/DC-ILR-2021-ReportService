namespace ESFA.DC.ILR.ReportService.Reports.Funding.Trailblazer.EmployerIncentive.Model
{
    public class TrailblazerEmployerIncentivesReportModel
    {
        public int EmployerIdentifier { get; set; }

        public decimal AugustSmallEmployerIncentive { get; set; }
        public decimal August1618ApprenticeIncentive { get; set; }
        public decimal AugustAchievementPayment { get; set; }
        public decimal AugustTotal => AugustSmallEmployerIncentive + August1618ApprenticeIncentive + AugustAchievementPayment;

        public decimal SeptemberSmallEmployerIncentive { get; set; }
        public decimal September1618ApprenticeIncentive { get; set; }
        public decimal SeptemberAchievementPayment { get; set; }
        public decimal SeptemberTotal => SeptemberSmallEmployerIncentive + September1618ApprenticeIncentive + SeptemberAchievementPayment;

        public decimal OctoberSmallEmployerIncentive { get; set; }
        public decimal October1618ApprenticeIncentive { get; set; }
        public decimal OctoberAchievementPayment { get; set; }
        public decimal OctoberTotal => OctoberSmallEmployerIncentive + October1618ApprenticeIncentive + OctoberAchievementPayment;

        public decimal NovemberSmallEmployerIncentive { get; set; }
        public decimal November1618ApprenticeIncentive { get; set; }
        public decimal NovemberAchievementPayment { get; set; }
        public decimal NovemberTotal => NovemberSmallEmployerIncentive + November1618ApprenticeIncentive + NovemberAchievementPayment;

        public decimal DecemberSmallEmployerIncentive { get; set; }
        public decimal December1618ApprenticeIncentive { get; set; }
        public decimal DecemberAchievementPayment { get; set; }
        public decimal DecemberTotal => DecemberSmallEmployerIncentive + December1618ApprenticeIncentive + DecemberAchievementPayment;

        public decimal JanuarySmallEmployerIncentive { get; set; }
        public decimal January1618ApprenticeIncentive { get; set; }
        public decimal JanuaryAchievementPayment { get; set; }
        public decimal JanuaryTotal => JanuarySmallEmployerIncentive + January1618ApprenticeIncentive + JanuaryAchievementPayment;

        public decimal FebruarySmallEmployerIncentive { get; set; }
        public decimal February1618ApprenticeIncentive { get; set; }
        public decimal FebruaryAchievementPayment { get; set; }
        public decimal FebruaryTotal => FebruarySmallEmployerIncentive + February1618ApprenticeIncentive + FebruaryAchievementPayment;

        public decimal MarchSmallEmployerIncentive { get; set; }
        public decimal March1618ApprenticeIncentive { get; set; }
        public decimal MarchAchievementPayment { get; set; }
        public decimal MarchTotal => MarchSmallEmployerIncentive + March1618ApprenticeIncentive + MarchAchievementPayment;

        public decimal AprilSmallEmployerIncentive { get; set; }
        public decimal April1618ApprenticeIncentive { get; set; }
        public decimal AprilAchievementPayment { get; set; }
        public decimal AprilTotal => AprilSmallEmployerIncentive + April1618ApprenticeIncentive + AprilAchievementPayment;

        public decimal MaySmallEmployerIncentive { get; set; }
        public decimal May1618ApprenticeIncentive { get; set; }
        public decimal MayAchievementPayment { get; set; }
        public decimal MayTotal => MaySmallEmployerIncentive + May1618ApprenticeIncentive + MayAchievementPayment;

        public decimal JuneSmallEmployerIncentive { get; set; }
        public decimal June1618ApprenticeIncentive { get; set; }
        public decimal JuneAchievementPayment { get; set; }
        public decimal JuneTotal => JuneSmallEmployerIncentive + June1618ApprenticeIncentive + JuneAchievementPayment;

        public decimal JulySmallEmployerIncentive { get; set; }
        public decimal July1618ApprenticeIncentive { get; set; }
        public decimal JulyAchievementPayment { get; set; }
        public decimal JulyTotal => JulySmallEmployerIncentive + July1618ApprenticeIncentive + JulyAchievementPayment;

        public decimal TotalSmallEmployerIncentive => AugustSmallEmployerIncentive + SeptemberSmallEmployerIncentive +
                                                      OctoberSmallEmployerIncentive + NovemberSmallEmployerIncentive +
                                                      DecemberSmallEmployerIncentive + JanuarySmallEmployerIncentive +
                                                      FebruarySmallEmployerIncentive + MarchSmallEmployerIncentive +
                                                      AprilSmallEmployerIncentive + MaySmallEmployerIncentive +
                                                      JuneSmallEmployerIncentive + JulySmallEmployerIncentive;

        public decimal Total1618ApprenticeIncentive => August1618ApprenticeIncentive + September1618ApprenticeIncentive +
                                                       October1618ApprenticeIncentive + November1618ApprenticeIncentive +
                                                       December1618ApprenticeIncentive + January1618ApprenticeIncentive +
                                                       February1618ApprenticeIncentive + March1618ApprenticeIncentive +
                                                       April1618ApprenticeIncentive + May1618ApprenticeIncentive +
                                                       June1618ApprenticeIncentive + July1618ApprenticeIncentive;

        public decimal TotalAchievementPayment => AugustAchievementPayment + SeptemberAchievementPayment +
                                                       OctoberAchievementPayment + NovemberAchievementPayment +
                                                       DecemberAchievementPayment + JanuaryAchievementPayment +
                                                       FebruaryAchievementPayment + MarchAchievementPayment +
                                                       AprilAchievementPayment + MayAchievementPayment +
                                                       JuneAchievementPayment + JulyAchievementPayment;

        public decimal GrandTotal => TotalSmallEmployerIncentive + Total1618ApprenticeIncentive + TotalAchievementPayment;

    }
}
