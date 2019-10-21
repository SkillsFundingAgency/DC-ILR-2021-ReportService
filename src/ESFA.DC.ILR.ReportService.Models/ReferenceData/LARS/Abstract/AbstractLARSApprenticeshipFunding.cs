using System;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS.Abstract
{
    public class AbstractLARSApprenticeshipFunding : AbstractTimeBoundedEntity
    {
        public string FundingCategory { get; set; }

        public int? BandNumber { get; set; }

        public Decimal? CoreGovContributionCap { get; set; }

        public Decimal? SixteenToEighteenIncentive { get; set; }

        public Decimal? SixteenToEighteenProviderAdditionalPayment { get; set; }

        public Decimal? SixteenToEighteenEmployerAdditionalPayment { get; set; }

        public Decimal? SixteenToEighteenFrameworkUplift { get; set; }

        public Decimal? CareLeaverAdditionalPayment { get; set; }

        public Decimal? Duration { get; set; }

        public Decimal? ReservedValue2 { get; set; }

        public Decimal? ReservedValue3 { get; set; }

        public Decimal? MaxEmployerLevyCap { get; set; }

        public string FundableWithoutEmployer { get; set; }
    }
}
