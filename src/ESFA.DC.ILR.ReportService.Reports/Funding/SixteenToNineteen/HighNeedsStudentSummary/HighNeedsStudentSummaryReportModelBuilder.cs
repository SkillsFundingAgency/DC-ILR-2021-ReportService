using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Reports.Extensions;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary
{
    public class HighNeedsStudentSummaryReportModelBuilder : IModelBuilder<HighNeedsStudentSummaryReportModel>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private const string ReportGeneratedTimeStringFormat = "HH:mm:ss on dd/MM/yyyy";

        public HighNeedsStudentSummaryReportModelBuilder(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }
        public HighNeedsStudentSummaryReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm25Data = reportServiceDependentData.Get<FM25Global>();
            var referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();

            var organisationName = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.Name ?? string.Empty;
            var learners = message?.Learners ?? Enumerable.Empty<ILearner>();
            var model = new HighNeedsStudentSummaryReportModel();

            var ilrFileName = reportServiceContext.OriginalFilename ?? reportServiceContext.Filename;

            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            var reportGeneratedAt = "Report generated at: " + dateTimeNowUk.ToString(ReportGeneratedTimeStringFormat);

            // Header
            model.ProviderName = organisationName;
            model.Ukprn = reportServiceContext.Ukprn;
            model.IlrFile = ilrFileName;
            model.Year = ReportingConstants.Year;

           
            // Body
            // test for applicable learning deliveries
            var applicableLearners = learners.Where(x => x.LearningDeliveries.Any(ld =>
                ld.FundModel == 25 &&
                ld.LearningDeliveryFAMs.Any(fam => fam.LearnDelFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.SOF)) &&
                ld.LearningDeliveryFAMs.Any(fam => fam.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMCodeConstants.SOF_ESFA_1619))))?.ToList();


            var applicablefm25Learners =
                fm25Data.Learners?.Where(x => x.StartFund.HasValue && x.StartFund.Value)?.ToList();

            var validLearnersForFundlineA = applicablefm25Learners?
                .Where(x => x.FundLine == FundLineConstants.DirectFundedStudents1416).Select(x => x.LearnRefNumber)?.ToArray();
            var validLearnersForFundlineB = applicablefm25Learners?
                .Where(x => x.FundLine == FundLineConstants.StudentsIncludingHighNeeds1619).Select(x => x.LearnRefNumber)?.ToArray();
            var validLearnersForFundlineC = applicablefm25Learners?
                .Where(x => x.FundLine == FundLineConstants.StudentsWithEHCP1924).Select(x => x.LearnRefNumber)?.ToArray();
            var validLearnersForFundlineD = applicablefm25Learners?
                .Where(x => x.FundLine == FundLineConstants.ContinuingStudents19Plus).Select(x => x.LearnRefNumber)?.ToArray();

            PopulateFundlineA(validLearnersForFundlineA, applicableLearners, model);
            PopulateFundlineB(validLearnersForFundlineB, applicableLearners, model);
            PopulateFundlineC(validLearnersForFundlineC, applicableLearners, model);
            PopulatefundlineD(validLearnersForFundlineD, applicableLearners, model);

            // Footer
            model.ReportGeneratedAt = reportGeneratedAt;
            model.ApplicationVersion = reportServiceContext.ServiceReleaseVersion;
            model.ComponentSetVersion = "NA";
            model.OrganisationData = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            model.LargeEmployerData = referenceDataRoot.MetaDatas.ReferenceDataVersions.Employers.Version;
            model.LarsData = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            model.PostcodeData = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;
            model.FilePreparationDate = message.HeaderEntity.CollectionDetailsEntity.FilePreparationDate.ToString("dd/MM/yyyy");

            return model;
        }


        private void PopulateFundlineA(string[] validLearnersForFundlineA, List<ILearner> applicableLearners, HighNeedsStudentSummaryReportModel model)
        {
            if (validLearnersForFundlineA != null)
            {
                var validApplicableLearners1416FundLine =
                    applicableLearners.Where(x =>
                            validLearnersForFundlineA.Contains(x.LearnRefNumber) && x.LearnerFAMs != null)
                        .ToList();

                model.TotalDirectFunded1416_WithEHCP =
                    validApplicableLearners1416FundLine.Count(x => x.LearnerFAMs.Any(y =>
                        y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) && y.LearnFAMCode == 1));

                model.TotalDirectFunded1416_WithoutEHCP = validApplicableLearners1416FundLine.Count(x =>
                    x.LearnerFAMs.Any(y =>!y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC)));

                model.TotalDirectFunded1416_HNSWithoutEHCP = validApplicableLearners1416FundLine.Count(x =>
                    x.LearnerFAMs.Any(y =>
                        !y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                        (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) &&
                         y.LearnFAMCode == 1)));

                model.TotalDirectFunded1416_HNSWithEHCP = validApplicableLearners1416FundLine.Count(x =>
                    x.LearnerFAMs.Any(y =>
                        (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) ||
                         y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC)) && y.LearnFAMCode == 1));

                model.TotalDirectFunded1416_EHCPWithoutHNS = validApplicableLearners1416FundLine.Count(x =>
                    x.LearnerFAMs.Any(y =>
                        !y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) &&
                        (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) && y.LearnFAMCode == 1)));



                var TotalDirectFunded1416_WithEHCP = validApplicableLearners1416FundLine.Where(x => x.LearnerFAMs.Any(y => string.Equals(y.LearnFAMType, LearnerFAMTypeConstants.EHC, StringComparison.OrdinalIgnoreCase) &&
                                                                                                                             y.LearnFAMCode == 1))?.Count();

                var TotalDirectFunded1416_WithoutEHCP = validApplicableLearners1416FundLine.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, LearnerFAMTypeConstants.EHC, StringComparison.OrdinalIgnoreCase) &&
                                                                                                                           y.LearnFAMCode == 1))?.Count();
                 
                var TotalDirectFunded1416_HNSWithoutEHCP = validApplicableLearners1416FundLine.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, LearnerFAMTypeConstants.EHC, StringComparison.OrdinalIgnoreCase) &&
                                                                                                                              (string.Equals(y.LearnFAMType, LearnerFAMTypeConstants.HNS, StringComparison.OrdinalIgnoreCase) && y.LearnFAMCode == 1)))?.Count();
                 
                var TotalDirectFunded1416_HNSWithEHCP = validApplicableLearners1416FundLine.Where(x => x.LearnerFAMs.Any(y => (string.Equals(y.LearnFAMType, LearnerFAMTypeConstants.HNS, StringComparison.OrdinalIgnoreCase) ||
                                                                                                                            string.Equals(y.LearnFAMType, LearnerFAMTypeConstants.EHC, StringComparison.OrdinalIgnoreCase)) && y.LearnFAMCode == 1))?.Count();
                 
                var TotalDirectFunded1416_EHCPWithoutHNS = validApplicableLearners1416FundLine.Where(x => x.LearnerFAMs.Any(y => !string.Equals(y.LearnFAMType, LearnerFAMTypeConstants.HNS, StringComparison.OrdinalIgnoreCase) &&
                                                                                                                                   (string.Equals(y.LearnFAMType, LearnerFAMTypeConstants.EHC, StringComparison.OrdinalIgnoreCase) && y.LearnFAMCode == 1)))?.Count();


            }
        }
        
        private void PopulateFundlineB(string[] validLearnersForFundlineB, List<ILearner> applicableLearners,
            HighNeedsStudentSummaryReportModel model)
        {
            if (validLearnersForFundlineB != null)
            {
                var validApplicableLearners1619FundLine =
                    applicableLearners.Where(x => validLearnersForFundlineB.Contains(x.LearnRefNumber) && x.LearnerFAMs != null)
                        .ToList();

                model.Total1619IncludingHNS_WithEHCP = validApplicableLearners1619FundLine.Count(x =>
                    x.LearnerFAMs.Any(y => y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                                           y.LearnFAMCode == 1));

                model.Total1619IncludingHNS_WithoutEHCP = validApplicableLearners1619FundLine.Count(x => x.LearnerFAMs.Any(y =>
                    !y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                    y.LearnFAMCode == 1));

                model.Total1619IncludingHNS_HNSWithoutEHCP = validApplicableLearners1619FundLine.Count(x =>
                    x.LearnerFAMs.Any(y => !y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                                           (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) &&
                                            y.LearnFAMCode == 1)));

                model.Total1619IncludingHNS_HNSWithEHCP = validApplicableLearners1619FundLine.Count(x => x.LearnerFAMs.Any(y =>
                    (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) ||
                     y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC)) && y.LearnFAMCode == 1));

                model.Total1619IncludingHNS_EHCPWithoutHNS = validApplicableLearners1619FundLine.Count(x =>
                    x.LearnerFAMs.Any(y => !y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) &&
                                           (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                                            y.LearnFAMCode == 1)));
            }
        }

        private void PopulateFundlineC(string[] validLearnersForFundlineC, List<ILearner> applicableLearners,
           HighNeedsStudentSummaryReportModel model)
        {
            if (validLearnersForFundlineC != null)
            {
                var validApplicableLearners1924Fundline =
                    applicableLearners.Where(x => validLearnersForFundlineC.Contains(x.LearnRefNumber) && x.LearnerFAMs != null)
                        .ToList();

                model.Total1924WithEHCP_WithEHCP = validApplicableLearners1924Fundline.Count(x => x.LearnerFAMs.Any(y =>
                    y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                    y.LearnFAMCode == 1));

                model.Total1924WithEHCP_WithoutEHCP = validApplicableLearners1924Fundline.Count(x => x.LearnerFAMs.Any(y =>
                    !y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                    y.LearnFAMCode == 1));

                model.Total1924WithEHCP_HNSWithoutEHCP = validApplicableLearners1924Fundline.Count(x => x.LearnerFAMs.Any(y =>
                    !y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                    (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) && y.LearnFAMCode == 1)));

                model.Total1924WithEHCP_HNSWithEHCP = validApplicableLearners1924Fundline.Count(x => x.LearnerFAMs.Any(y =>
                    (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) ||
                     y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC)) && y.LearnFAMCode == 1));

                model.Total1924WithEHCP_EHCPWithoutHNS = validApplicableLearners1924Fundline.Count(x => x.LearnerFAMs.Any(y =>
                    !y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) &&
                    (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) && y.LearnFAMCode == 1)));
            }
        }

        private void PopulatefundlineD(string[] validLearnersForFundlineD, List<ILearner> applicableLearners,
            HighNeedsStudentSummaryReportModel model)
        {
            if (validLearnersForFundlineD != null)
            {
                var validApplicableLearners19PlusFundline =
                    applicableLearners.Where(x => validLearnersForFundlineD.Contains(x.LearnRefNumber) && x.LearnerFAMs != null)
                        .ToList();

                model.Total19PlusWithoutEHCP_WithEHCP = validApplicableLearners19PlusFundline.Count(x => x.LearnerFAMs.Any(y =>
                    y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                    y.LearnFAMCode == 1));

                model.Total19PlusWithoutEHCP_WithoutEHCP = validApplicableLearners19PlusFundline.Count(x =>
                    x.LearnerFAMs.Any(y => !y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                                           y.LearnFAMCode == 1));

                model.Total19PlusWithoutEHCP_HNSWithoutEHCP = validApplicableLearners19PlusFundline.Count(x =>
                    x.LearnerFAMs.Any(y => !y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                                           (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) &&
                                            y.LearnFAMCode == 1)));

                model.Total19PlusWithoutEHCP_HNSWithEHCP = validApplicableLearners19PlusFundline.Count(x =>
                    x.LearnerFAMs.Any(y => (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) ||
                                            y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC)) &&
                                           y.LearnFAMCode == 1));

                model.Total19PlusWithoutEHCP_EHCPWithoutHNS = validApplicableLearners19PlusFundline.Count(x =>
                    x.LearnerFAMs.Any(y => !y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) &&
                                           (y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) &&
                                            y.LearnFAMCode == 1)));
            }
        }

       

    }
}
