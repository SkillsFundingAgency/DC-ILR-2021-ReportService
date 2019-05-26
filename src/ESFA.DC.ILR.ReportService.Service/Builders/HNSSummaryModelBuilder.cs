using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Interface.Builders;
using ESFA.DC.ILR.ReportService.Model.ReportModels;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ReportService.Service.Builders
{
    public class HNSSummaryModelBuilder : IHNSSummaryModelBuilder
    {
        public HNSSummaryModel BuildHNSSummaryModel(
            ILogger logger,
            Task<IMessage> ilrFileTask,
            List<string> validLearners,
            FM25Global fm25Data,
            CancellationToken cancellationToken)
        {
            var model = new HNSSummaryModel();

            List<ILearner> learners =
                ilrFileTask.Result?.Learners?.Where(x => validLearners.Contains(x.LearnRefNumber)).ToList();

            if (learners == null)
            {
                logger.LogWarning("Failed to get learners for High Needs Students Summary Report");
                return null;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            // test for applicable learning deliveries
            var applicableLearners = learners.Where(x => x.LearningDeliveries.Any(ld =>
                ld.FundModel == 25 &&
                ld.LearningDeliveryFAMs.Any(fam => string.Equals(fam.LearnDelFAMType, Constants.LearningDeliveryFAMCodeSOF, StringComparison.OrdinalIgnoreCase)) &&
                ld.LearningDeliveryFAMs.Any(fam => string.Equals(fam.LearnDelFAMCode, Constants.LearningDeliveryFAMCode107, StringComparison.OrdinalIgnoreCase))))?.ToList();

            var applicablefm25Learners =
                fm25Data.Learners?.Where(x => x.StartFund.HasValue && x.StartFund.Value)?.ToList();

            var validLearnersForFundlineA = applicablefm25Learners?
                .Where(x => x.FundLine == Constants.DirectFundedStudents1416FundLine).Select(x => x.LearnRefNumber)?.ToArray();

            var validApplicableLearners1416FundLine =
                applicableLearners.Where(x => validLearnersForFundlineA.Contains(x.LearnRefNumber) && x.LearnerFAMs != null).ToList();

            model.TotalDirectFunded1416_WithEHCP = validApplicableLearners1416FundLine.Where(x => x.LearnerFAMs.Any(y => string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                               y.LearnFAMCode == 1))?.Count();

            model.TotalDirectFunded1416_WithoutEHCP = validApplicableLearners1416FundLine.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                                                               y.LearnFAMCode == 1))?.Count();

            model.TotalDirectFunded1416_HNSWithoutEHCP = validApplicableLearners1416FundLine.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                                                                               (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) && y.LearnFAMCode == 1)))?.Count();

            model.TotalDirectFunded1416_HNSWithEHCP = validApplicableLearners1416FundLine.Where(x => x.LearnerFAMs.Any(y => (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) ||
                                                                                                string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase)) && y.LearnFAMCode == 1))?.Count();

            model.TotalDirectFunded1416_EHCPWithoutHNS = validApplicableLearners1416FundLine.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) &&
                                                                                               (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) && y.LearnFAMCode == 1)))?.Count();

            var validLearnersForFundlineB = applicablefm25Learners?
            .Where(x => x.FundLine == Constants.Students1619AllFundLine).Select(x => x.LearnRefNumber)?.ToArray();

            var validApplicableLearners1619FundLine =
                applicableLearners.Where(x => validLearnersForFundlineB.Contains(x.LearnRefNumber) && x.LearnerFAMs != null).ToList();

            model.Total1619IncludingHNS_WithEHCP = validApplicableLearners1619FundLine.Where(x => x.LearnerFAMs.Any(y => string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                               y.LearnFAMCode == 1))?.Count();

            model.Total1619IncludingHNS_WithoutEHCP = validApplicableLearners1619FundLine.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                                                               y.LearnFAMCode == 1))?.Count();

            model.Total1619IncludingHNS_HNSWithoutEHCP = validApplicableLearners1619FundLine.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                                                                               (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) && y.LearnFAMCode == 1)))?.Count();

            model.Total1619IncludingHNS_HNSWithEHCP = validApplicableLearners1619FundLine.Where(x => x.LearnerFAMs.Any(y => (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) ||
                                                                                                string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase)) && y.LearnFAMCode == 1))?.Count();

            model.Total1619IncludingHNS_EHCPWithoutHNS = validApplicableLearners1619FundLine.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) &&
                                                                                               (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) && y.LearnFAMCode == 1)))?.Count();

            var validLearnersForFundlineC = applicablefm25Learners?
            .Where(x => x.FundLine == Constants.StudentsWithEHCP1924FundLine).Select(x => x.LearnRefNumber)?.ToArray();

            var validApplicableLearners1924Fundline =
                applicableLearners.Where(x => validLearnersForFundlineC.Contains(x.LearnRefNumber) && x.LearnerFAMs != null).ToList();

            model.Total1924WithEHCP_WithEHCP = validApplicableLearners1924Fundline.Where(x => x.LearnerFAMs.Any(y => string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                                                                                                     y.LearnFAMCode == 1))?.Count();

            model.Total1924WithEHCP_WithoutEHCP = validApplicableLearners1924Fundline.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                                                               y.LearnFAMCode == 1))?.Count();

            model.Total1924WithEHCP_HNSWithoutEHCP = validApplicableLearners1924Fundline.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                                                                               (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) && y.LearnFAMCode == 1)))?.Count();

            model.Total1924WithEHCP_HNSWithEHCP = validApplicableLearners1924Fundline.Where(x => x.LearnerFAMs.Any(y => (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) ||
                                                                                                string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase)) && y.LearnFAMCode == 1))?.Count();

            model.Total1924WithEHCP_EHCPWithoutHNS = validApplicableLearners1924Fundline.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) &&
                                                                                               (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) && y.LearnFAMCode == 1)))?.Count();

            var validLearnersForFundlineD = applicablefm25Learners?
            .Where(x => x.FundLine == Constants.ContinuingStudents19PlusFundLine).Select(x => x.LearnRefNumber)?.ToArray();

            var validApplicableLearners19PlusFundline =
                applicableLearners.Where(x => validLearnersForFundlineD.Contains(x.LearnRefNumber) && x.LearnerFAMs != null).ToList();

            model.Total19PlusWithoutEHCP_WithEHCP = validApplicableLearners19PlusFundline.Where(x => x.LearnerFAMs.Any(y => string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                               y.LearnFAMCode == 1))?.Count();

            model.Total19PlusWithoutEHCP_WithoutEHCP = validApplicableLearners19PlusFundline.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                                                               y.LearnFAMCode == 1))?.Count();

            model.Total19PlusWithoutEHCP_HNSWithoutEHCP = validApplicableLearners19PlusFundline.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) &&
                                                                                               (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) && y.LearnFAMCode == 1)))?.Count();

            model.Total19PlusWithoutEHCP_HNSWithEHCP = validApplicableLearners19PlusFundline.Where(x => x.LearnerFAMs.Any(y => (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) ||
                                                                                                string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase)) && y.LearnFAMCode == 1))?.Count();

            model.Total19PlusWithoutEHCP_EHCPWithoutHNS = validApplicableLearners19PlusFundline.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeHNS, StringComparison.OrdinalIgnoreCase) &&
                                                                                               (string.Equals(y.LearnFAMType, Constants.LearnerFAMCodeEHC, StringComparison.OrdinalIgnoreCase) && y.LearnFAMCode == 1)))?.Count();

            return model;
        }
    }
}
