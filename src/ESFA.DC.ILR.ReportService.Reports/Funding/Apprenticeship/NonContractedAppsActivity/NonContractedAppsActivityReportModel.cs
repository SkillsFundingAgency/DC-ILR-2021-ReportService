using ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.Abstract.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity
{
    public class NonContractedAppsActivityReportModel : AbstractAppsReportModel
    {
        public decimal? AugustTotal { get; set; }

        public decimal? SeptemberTotal { get; set; }

        public decimal? OctoberTotal { get; set; }

        public decimal? NovemberTotal { get; set; }

        public decimal? DecemberTotal { get; set; }

        public decimal? JanuaryTotal { get; set; }

        public decimal? FebruaryTotal { get; set; }

        public decimal? MarchTotal { get; set; }

        public decimal? AprilTotal { get; set; }

        public decimal? MayTotal { get; set; }

        public decimal? JuneTotal { get; set; }

        public decimal? JulyTotal { get; set; }

        public decimal? Total => SumMonths();

        private decimal? SumMonths()
        {
            return
                AugustTotal +
                SeptemberTotal +
                OctoberTotal +
                NovemberTotal +
                DecemberTotal +
                JanuaryTotal +
                FebruaryTotal +
                MarchTotal +
                AprilTotal +
                MayTotal +
                JuneTotal +
                JulyTotal ?? 0m;
        }
    }
}