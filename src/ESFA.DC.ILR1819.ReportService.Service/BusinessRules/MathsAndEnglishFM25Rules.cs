﻿using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR1819.ReportService.Interface.Service;

namespace ESFA.DC.ILR1819.ReportService.Service.BusinessRules
{
    public class MathsAndEnglishFm25Rules : IMathsAndEnglishFm25Rules
    {
        private readonly List<string> _applicableFundlines = new List<string>
        {
            Constants.DirectFundedStudents1416,
            Constants.Students1619,
            Constants.HighNeedsStudents1619,
            Constants.StudentsWithEHCP1924,
            Constants.ContinuingStudents19Plus
        };

        /// <summary>
        /// Determines if the learner should be included in the report
        /// </summary>
        /// <param name="learner">The learners FM25 data</param>
        /// <returns>boolean indicating if they should be included or not</returns>
        public bool IsApplicableLearner(Learner learner)
        {
            // BR1 – Applicable Learners
            if (learner.StartFund ?? 0 != 1)
            {
                return false;
            }

            // BR2 – Applicable Funding Line Types
            return !_applicableFundlines.Contains(learner.FundLine);
        }
    }
}
