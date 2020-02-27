using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.FRM;
using ESFA.DC.ILR.ReportService.Reports.Constants;

namespace ESFA.DC.ILR.ReportService.Reports.Frm
{
    public abstract class FrmBaseModelBuilder
    {
        protected const string ADLLearnDelFamType = "ADL";
        protected const string SOFLearnDelFamType = "SOF";
        protected const string RESLearnDelFamType = "RES";

        private readonly HashSet<int> _apprenticeshipHashSet = new HashSet<int> { 2, 3, 20, 21, 22, 23, 25 };
        private readonly HashSet<string> _devolvedCodes = new HashSet<string> { "110", "111", "112", "113", "114", "115", "116" };

        protected string RetrieveFamCodeForType(IEnumerable<LearningDeliveryFAM> deliveryFams, string learnDelFamType)
        {
            return deliveryFams.FirstOrDefault(f => f.LearnDelFAMType == learnDelFamType)?.LearnDelFAMCode ?? string.Empty;
        }

        protected string RetrieveFamCodeForType(IEnumerable<ILR.Model.Interface.ILearningDeliveryFAM> deliveryFams, string learnDelFamType)
        {
            return deliveryFams.FirstOrDefault(f => f.LearnDelFAMType == learnDelFamType)?.LearnDelFAMCode ?? string.Empty;
        }

        protected string CalculateFundingStream(int fundModel, int? progTypeNullable, string advancedLearnerLoansIndicator, string devolvedIndicator)
        {
            if (_devolvedCodes.Contains(devolvedIndicator))
            {
                return FundingStreamConstants.Devolved;
            }

            if (fundModel == 35 && !progTypeNullable.HasValue)
            {
                return FundingStreamConstants.AEB;
            }

            if ((fundModel == 35 && progTypeNullable.HasValue && _apprenticeshipHashSet.Contains(progTypeNullable.Value)) || fundModel == 81)
            {
                return FundingStreamConstants.Apprenticeships;
            }

            if (fundModel == 25 && progTypeNullable == 24)
            {
                return FundingStreamConstants.Traineeships1618;
            }

            if (fundModel == 35 && progTypeNullable == 24)
            {
                return FundingStreamConstants.Traineeships1924;
            }

            if (fundModel == 25 && progTypeNullable != 24)
            {
                return FundingStreamConstants.SixteenToNineteen;
            }

            if (fundModel == 36)
            {
                return FundingStreamConstants.ApprenticeshipsFromMay2017;
            }

            if (fundModel == 99 && advancedLearnerLoansIndicator == "1")
            {
                return FundingStreamConstants.AdvancedLearnerLoans;
            }

            if (fundModel == 99 && advancedLearnerLoansIndicator != "1")
            {
                return FundingStreamConstants.NonFunded;
            }

            return FundingStreamConstants.Other;
        }
    }
}
