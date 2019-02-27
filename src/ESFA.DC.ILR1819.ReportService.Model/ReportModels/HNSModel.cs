using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace ESFA.DC.ILR1819.ReportService.Model.ReportModels
{
    public sealed class HNSModel
    {
        public string FundLine { get; set; }

        public string LearnRefNumber { get; set; }

        public string FamilyName { get; set; }

        public string GivenNames { get; set; }

        public string CampId { get; set; }

        [Default("N")]
        public string LearnerFAM_A { get; set; }

        [Default("N")]
        public string LearnerFAM_B { get; set; }

        [Default("N")]
        public string LearnerFAM_C { get; set; }

        [Default("N")]
        public string LearnerFAM_D { get; set; }

        [Default("N")]
        public string LearnerFAM_E { get; set; }

        public string OfficalSensitive { get; }
    }
}
