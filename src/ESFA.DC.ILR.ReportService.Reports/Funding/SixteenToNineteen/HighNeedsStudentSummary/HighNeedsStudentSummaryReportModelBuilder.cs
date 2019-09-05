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
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;

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
                    applicableLearners.Where(x => validLearnersForFundlineA.Contains(x.LearnRefNumber) && x.LearnerFAMs != null).ToList();

                model.DirectFunded1416StudentsTotal = new FundingLineReportingBandStudentNumbers()
                {
                    WithEHCP = validApplicableLearners1416FundLine.Count(WithEHCP),
                    WithoutEHCP = validApplicableLearners1416FundLine.Count(WithoutEhcp),
                    HNSWithoutEHCP = validApplicableLearners1416FundLine.Count(HNSWithoutEHCP),
                    EHCPWithHNS = validApplicableLearners1416FundLine.Count(HNSWithEHCP),
                    EHCPWithoutHNS = validApplicableLearners1416FundLine.Count(EHCPWithoutHNS)
                };
            }
        }
        
        private void PopulateFundlineB(string[] validLearnersForFundlineB, List<ILearner> applicableLearners, HighNeedsStudentSummaryReportModel model)
        {
            if (validLearnersForFundlineB != null)
            {
                var validApplicableLearners1619FundLine =
                    applicableLearners.Where(x => validLearnersForFundlineB.Contains(x.LearnRefNumber) && x.LearnerFAMs != null)
                        .ToList();

                model.IncludingHNS1619StudentsTotal = new FundingLineReportingBandStudentNumbers()
                {
                    WithEHCP = validApplicableLearners1619FundLine.Count(WithEHCP),
                    WithoutEHCP = validApplicableLearners1619FundLine.Count(WithoutEhcp),
                    HNSWithoutEHCP = validApplicableLearners1619FundLine.Count(HNSWithoutEHCP),
                    EHCPWithHNS = validApplicableLearners1619FundLine.Count(HNSWithEHCP),
                    EHCPWithoutHNS = validApplicableLearners1619FundLine.Count(EHCPWithoutHNS)
                };
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
                model.EHCP1924StudentsTotal = new FundingLineReportingBandStudentNumbers()
                {
                    WithEHCP = validApplicableLearners1924Fundline.Count(WithEHCP),
                    WithoutEHCP = validApplicableLearners1924Fundline.Count(WithoutEhcp),
                    HNSWithoutEHCP = validApplicableLearners1924Fundline.Count(HNSWithoutEHCP),
                    EHCPWithHNS = validApplicableLearners1924Fundline.Count(HNSWithEHCP),
                    EHCPWithoutHNS = validApplicableLearners1924Fundline.Count(EHCPWithoutHNS)
                };
            }
        }

        private void PopulatefundlineD(string[] validLearnersForFundlineD, List<ILearner> applicableLearners,HighNeedsStudentSummaryReportModel model)
        {
            if (validLearnersForFundlineD != null)
            {
                var validApplicableLearners19PlusFundline = applicableLearners.Where(x => validLearnersForFundlineD.Contains(x.LearnRefNumber) && x.LearnerFAMs != null).ToList();

                model.Continuing19PlusExcludingEHCPStudentsTotal = new FundingLineReportingBandStudentNumbers()
                {
                    WithEHCP = validApplicableLearners19PlusFundline.Count(WithEHCP),
                    WithoutEHCP = validApplicableLearners19PlusFundline.Count(WithoutEhcp),
                    HNSWithoutEHCP = validApplicableLearners19PlusFundline.Count(HNSWithoutEHCP),
                    EHCPWithHNS = validApplicableLearners19PlusFundline.Count(HNSWithEHCP),
                    EHCPWithoutHNS = validApplicableLearners19PlusFundline.Count(EHCPWithoutHNS)
                };
            }
        }

        public bool WithEHCP(ILearner learner)
        {
            return learner.LearnerFAMs.Any(y =>
                y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) && y.LearnFAMCode == 1);
        }

        public bool  WithoutEhcp(ILearner learner)
        {
            return !learner.LearnerFAMs.Any(y => y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC));
        }

        public bool HNSWithoutEHCP(ILearner learner)
        {
            return (learner.LearnerFAMs.Any(y => y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) && y.LearnFAMCode == 1)) &&
                !(learner.LearnerFAMs.Any(y => y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC)));
        }

        public bool HNSWithEHCP(ILearner learner)
        {
            return (learner.LearnerFAMs.Any(y => y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS) && y.LearnFAMCode == 1)) &&
                (learner.LearnerFAMs.Any(y => y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) && y.LearnFAMCode == 1));
        }
        public bool EHCPWithoutHNS(ILearner learner)
        {
            return !(learner.LearnerFAMs.Any(y => y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS))) &&
                (learner.LearnerFAMs.Any(y => y.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC) && y.LearnFAMCode == 1));
        }
      
    }
}
