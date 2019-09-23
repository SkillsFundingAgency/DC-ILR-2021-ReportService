using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.AdultFundingClaim.Model
{
    public class ActualEarnings
    {
        public decimal MidYearClaims { get; set; }

        public decimal YearEndClaims { get; set; }

        public decimal FinalClaims { get; set; }
    }
}
