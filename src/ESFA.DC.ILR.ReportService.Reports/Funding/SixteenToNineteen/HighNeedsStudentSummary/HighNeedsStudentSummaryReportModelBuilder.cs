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
                .Where(x => x.FundLine == FundLineConstants.StudentsExcludingHighNeeds1619 ||
                            x.FundLine == FundLineConstants.HighNeedsStudents1619).Select(x => x.LearnRefNumber)?.ToArray();
            var validLearnersForFundlineC = applicablefm25Learners?
                .Where(x => x.FundLine == FundLineConstants.StudentsWithEHCP1924).Select(x => x.LearnRefNumber)?.ToArray();
            var validLearnersForFundlineD = applicablefm25Learners?
                .Where(x => x.FundLine == FundLineConstants.ContinuingStudents19Plus).Select(x => x.LearnRefNumber)?.ToArray();

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
            model.FilePreparationDate = message.HeaderEntity.CollectionDetailsEntity.FilePreparationDate.ToString("dd/MM/yyyy");

            return model;
        }

        private FundingLineReportingBandStudentNumbers BuildFundlineReportingBandStudentNumberModel(string[] validLearnersForFundline, List<ILearner> applicableLearners)
        {
            var model = new FundingLineReportingBandStudentNumbers();
            if (validLearnersForFundline != null)
            {
                var validApplicableLearners =
                    applicableLearners.Where(x => validLearnersForFundline.Contains(x.LearnRefNumber)).ToList();

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

        public bool WithEHCP(ILearner learner)
        {
            return HasLearnerFAMTypeAndCode(learner, LearnerFAMTypeConstants.EHC, 1);
        }

        public bool  WithoutEhcp(ILearner learner)
        {
            return !HasLearnerFAMType(learner, LearnerFAMTypeConstants.EHC);
        }

        public bool HNSWithoutEHCP(ILearner learner)
        {
            return HasLearnerFAMTypeAndCode(learner, LearnerFAMTypeConstants.HNS, 1) &&
                    !HasLearnerFAMType(learner, LearnerFAMTypeConstants.EHC);
        }

        public bool HNSWithEHCP(ILearner learner)
        {
            return HasLearnerFAMTypeAndCode(learner, LearnerFAMTypeConstants.HNS, 1) &&
                   HasLearnerFAMTypeAndCode(learner, LearnerFAMTypeConstants.EHC, 1);
        }
        public bool EHCPWithoutHNS(ILearner learner)
        {
            return !HasLearnerFAMType(learner, LearnerFAMTypeConstants.HNS) &&
                   HasLearnerFAMTypeAndCode(learner, LearnerFAMTypeConstants.EHC, 1);
        }

        public bool HasLearnerFAMType(ILearner learner, string famType)
        {
            return learner?.LearnerFAMs != null && learner.LearnerFAMs.Any(x => x.LearnFAMType.CaseInsensitiveEquals(famType));
        }

        public bool HasLearnerFAMTypeAndCode(ILearner learner, string famType, int code)
        {
            return learner?.LearnerFAMs != null && learner.LearnerFAMs.Any(x => x.LearnFAMType.CaseInsensitiveEquals(famType) && x.LearnFAMCode == code);
        }
    }
}
