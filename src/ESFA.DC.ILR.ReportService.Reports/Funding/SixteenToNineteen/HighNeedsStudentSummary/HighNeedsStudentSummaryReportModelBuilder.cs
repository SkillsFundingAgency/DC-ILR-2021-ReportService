using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary
{
    public class HighNeedsStudentSummaryReportModelBuilder : AbstractSixteenToNineteenReportModelBuilder, IModelBuilder<HighNeedsStudentSummaryReportModel>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
    
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

            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            var reportGeneratedAt = "Report generated at: " + FormatReportGeneratedAtDateTime(dateTimeNowUk);

            // Header
            model.ProviderName = organisationName;
            model.Ukprn = reportServiceContext.Ukprn.ToString();
            model.IlrFile = ExtractFileName(reportServiceContext.OriginalFilename);
            model.Year = ReportingConstants.Year;

           
            // Body
            // test for applicable learning deliveries
            var applicableLearners = learners.Where(LearnerFilter).ToList();

            var applicablefm25Learners = fm25Data.Learners?.Where(x => x.StartFund == true).ToList();

            var validLearnersForFundlineA = LearnRefNumbersWithFundLine(applicablefm25Learners, FundLineConstants.DirectFundedStudents1416)?.ToArray();
            var validLearnersForFundlineB = 
                LearnRefNumbersWithFundLine(
                    applicablefm25Learners,
                    FundLineConstants.StudentsExcludingHighNeeds1619,
                    FundLineConstants.HighNeedsStudents1619)?.ToArray();
            var validLearnersForFundlineC = LearnRefNumbersWithFundLine(applicablefm25Learners, FundLineConstants.StudentsWithEHCP1924)?.ToArray();
            var validLearnersForFundlineD = LearnRefNumbersWithFundLine(applicablefm25Learners, FundLineConstants.ContinuingStudents19Plus)?.ToArray();

            model.DirectFunded1416StudentsTotal =  BuildFundlineReportingBandStudentNumberModel(validLearnersForFundlineA, applicableLearners);
            model.IncludingHNS1619StudentsTotal =  BuildFundlineReportingBandStudentNumberModel(validLearnersForFundlineB, applicableLearners);
            model.EHCP1924StudentsTotal =  BuildFundlineReportingBandStudentNumberModel(validLearnersForFundlineC, applicableLearners);
            model.Continuing19PlusExcludingEHCPStudentsTotal =  BuildFundlineReportingBandStudentNumberModel(validLearnersForFundlineD, applicableLearners);

            // Footer
            model.ReportGeneratedAt = reportGeneratedAt;
            model.ApplicationVersion = reportServiceContext.ServiceReleaseVersion;
            model.ComponentSetVersion = "NA";
            model.OrganisationData = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            model.LargeEmployerData = referenceDataRoot.MetaDatas.ReferenceDataVersions.Employers.Version;
            model.LarsData = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            model.PostcodeData = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;
            model.FilePreparationDate = FormatFilePreparationDate(message?.HeaderEntity.CollectionDetailsEntity.FilePreparationDate);

            return model;
        }

        private IEnumerable<string> LearnRefNumbersWithFundLine(IEnumerable<FM25Learner> learners, params string[] fundLine)
        {
            var fundLinesHashSet = new HashSet<string>(fundLine, StringComparer.OrdinalIgnoreCase);

            return learners?.Where(x => fundLinesHashSet.Contains(x.FundLine)).Select(x => x.LearnRefNumber);
        }

        private bool LearnerFilter(ILearner learner)
        {
            return learner
                .LearningDeliveries?
                .Any(ld =>
                    ld.FundModel == FundModelConstants.FM25 &&
                    (ld.LearningDeliveryFAMs ?? Enumerable.Empty<ILearningDeliveryFAM>())
                        .Any(fam => 
                            fam.LearnDelFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.SOF) &&
                            fam.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMCodeConstants.SOF_ESFA_1619)))
                   ?? false;

        }

        private FundingLineReportingBandStudentNumbers BuildFundlineReportingBandStudentNumberModel(IEnumerable<string> validLearnersForFundline, IEnumerable<ILearner> applicableLearners)
        {
            var model = new FundingLineReportingBandStudentNumbers();

            if (validLearnersForFundline != null)
            {
                var validApplicableLearners = applicableLearners.Where(x => validLearnersForFundline.Contains(x.LearnRefNumber)).ToList();

                model =  new FundingLineReportingBandStudentNumbers()
                {
                    WithEHCP = validApplicableLearners.Count(WithEHCP),
                    WithoutEHCP = validApplicableLearners.Count(WithoutEhcp),
                    HNSWithoutEHCP = validApplicableLearners.Count(HNSWithoutEHCP),
                    EHCPWithHNS = validApplicableLearners.Count(HNSWithEHCP),
                    EHCPWithoutHNS = validApplicableLearners.Count(EHCPWithoutHNS),
                    TotalFundineStudents = validApplicableLearners.Count
                };
            }

            return model;
        }

        public bool WithEHCP(ILearner learner) => 
            HasLearnerFAMTypeAndCode(learner, LearnerFAMTypeConstants.EHC, 1);

        public bool  WithoutEhcp(ILearner learner) => 
            !HasLearnerFAMType(learner, LearnerFAMTypeConstants.EHC);

        public bool HNSWithoutEHCP(ILearner learner) =>
            HasLearnerFAMTypeAndCode(learner, LearnerFAMTypeConstants.HNS, 1) &&
            !HasLearnerFAMType(learner, LearnerFAMTypeConstants.EHC);

        public bool HNSWithEHCP(ILearner learner) =>
            HasLearnerFAMTypeAndCode(learner, LearnerFAMTypeConstants.HNS, 1) &&
            HasLearnerFAMTypeAndCode(learner, LearnerFAMTypeConstants.EHC, 1);

        public bool EHCPWithoutHNS(ILearner learner) =>
            !HasLearnerFAMType(learner, LearnerFAMTypeConstants.HNS) &&
            HasLearnerFAMTypeAndCode(learner, LearnerFAMTypeConstants.EHC, 1);

        public bool HasLearnerFAMType(ILearner learner, string famType) =>
            learner?.LearnerFAMs?.Any(x => x.LearnFAMType.CaseInsensitiveEquals(famType)) ?? false;

        public bool HasLearnerFAMTypeAndCode(ILearner learner, string famType, int code) => 
            learner?.LearnerFAMs?.Any(x => x.LearnFAMType.CaseInsensitiveEquals(famType) && x.LearnFAMCode == code) ?? false;
    }
}
