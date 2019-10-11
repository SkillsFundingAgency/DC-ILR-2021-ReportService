using System;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Abstract;

namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.LARS
{
    public class LARSStandardFunding : AbstractTimeBoundedEntity
    {
        public string FundingCategory { get; set; }

        public int? BandNumber { get; set; }

        public Decimal? CoreGovContributionCap { get; set; }

        public Decimal? SixteenToEighteenIncentive { get; set; }

        public Decimal? SmallBusinessIncentive { get; set; }

        public Decimal? AchievementIncentive { get; set; }

        public string FundableWithoutEmployer { get; set; }
    }
}
