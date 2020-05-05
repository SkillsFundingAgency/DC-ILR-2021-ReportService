using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.Organisations;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim.Constants;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim
{
    public class FundingClaimReportModelBuilder : AbstractSixteenToNineteenReportModelBuilder , IModelBuilder<FundingClaimReportModel>
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public FundingClaimReportModelBuilder(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public FundingClaimReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm25Data = reportServiceDependentData.Get<FM25Global>();
            var referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();

            var referenceDateFilter = RetrieveReportFilterValueFromContext<DateTime?>(reportServiceContext, ReportNameConstants.SixteenNineteenFundingClaim, ReportingConstants.ReferenceDateFilterPropertyName);

            var organisation = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn);
            var learners = message?.Learners ?? Enumerable.Empty<ILearner>();

            var model = new FundingClaimReportModel();

            var applicableLearners = FilterLearners(learners);

            // Header
            var referenceDate = referenceDateFilter.HasValue ? referenceDateFilter.Value.ShortDateStringFormat() : "(ALL)";
            BuildHeader(reportServiceContext, model, organisation, referenceDate);

            // Body
            BuildBody(model, fm25Data.Learners, applicableLearners, referenceDateFilter);

            // Footer
            BuildFooter(model, referenceDataRoot.MetaDatas.ReferenceDataVersions, reportServiceContext.ServiceReleaseVersion, message?.HeaderEntity.CollectionDetailsEntity.FilePreparationDate);

            return model;
        }

        private void BuildHeader(IReportServiceContext reportServiceContext, FundingClaimReportModel model, Organisation organisation, string referenceDate)
        {
            var cofRemoval = organisation?.OrganisationCoFRemovals?.OrderByDescending(x => x.EffectiveFrom).FirstOrDefault()?.CoFRemoval;
            var organisationName = organisation?.Name ?? string.Empty;

            model.ProviderName = organisationName;
            model.Ukprn = reportServiceContext.Ukprn.ToString();
            model.IlrFile = ExtractFileName(reportServiceContext.IlrReportingFilename);
            model.Year = ReportingConstants.Year;
            model.ReferenceDate = referenceDate;
            model.CofRemoval = -cofRemoval.GetValueOrDefault();
        }

        private void BuildFooter(FundingClaimReportModel model, ReferenceDataVersion referenceDataVersions, string applicationVersion, DateTime? filePrepDate)
        {
            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);

            model.ReportGeneratedAt = "Report generated at: " + FormatReportGeneratedAtDateTime(dateTimeNowUk);
            model.ApplicationVersion = applicationVersion;
            model.ComponentSetVersion = "NA";
            model.OrganisationData = referenceDataVersions.OrganisationsVersion.Version;
            model.LargeEmployerData = referenceDataVersions.Employers.Version;
            model.LarsData = referenceDataVersions.LarsVersion.Version;
            model.PostcodeData = referenceDataVersions.PostcodesVersion.Version;
            model.FilePreparationDate = FormatFilePreparationDate(filePrepDate);
            model.CofRemovalData = referenceDataVersions.CoFVersion.Version;
        }

        private void BuildBody(FundingClaimReportModel model, List<FM25Learner> fm25Learners, IEnumerable<ILearner> applicableLearners, DateTime? referenceDateFilter)
        {
            var learnersArray = applicableLearners.Select(x => x.LearnRefNumber);

            var fm25LearnersQueryable = fm25Learners?.AsQueryable() ?? Enumerable.Empty<FM25Learner>().AsQueryable();
            fm25LearnersQueryable = ApplyFm25LearnersFilter(fm25LearnersQueryable, learnersArray);
            fm25LearnersQueryable = ApplyUserFilters(fm25LearnersQueryable, referenceDateFilter);

            var filteredFm25Learners = fm25LearnersQueryable?.ToList();

            if (filteredFm25Learners != null && filteredFm25Learners.Any())
            {
                var fm25Learner = filteredFm25Learners.First();

                model.FundingFactor = new FundingFactorModel
                {
                    AreaCostFact1618Hist = fm25Learner.AreaCostFact1618Hist.GetValueOrDefault(0).ToString("N5"),
                    ProgWeightHist = fm25Learner.ProgWeightHist.GetValueOrDefault(0).ToString("N5"),
                    PrvDisadvPropnHist = fm25Learner.PrvDisadvPropnHist.GetValueOrDefault(0).ToString("N5"),
                    PrvHistLrgProgPropn = fm25Learner.PrvHistLrgProgPropn.GetValueOrDefault(0).ToString("N5"),
                    PrvRetentFactHist = fm25Learner.PrvRetentFactHist.GetValueOrDefault(0).ToString("N5"),
                    PrvHistL3ProgMathEngProp = fm25Learner.PrvHistL3ProgMathEngProp.GetValueOrDefault(0).ToString("N5")
                };

                var validProgrammeLearnersForFundlineA = filteredFm25Learners?.Where(x => StudyProgrammePredicate(x) && x.FundLine == FundLineConstants.DirectFundedStudents1416).ToList();
                var validTLevelLearnersForFundlineA = filteredFm25Learners?.Where(x => TLevelPredicate(x) && x.FundLine == FundLineConstants.DirectFundedStudents1416).ToList();
                model.DirectFundingStudents = BuildFundlineReprtingBandModelForProgrammes(validProgrammeLearnersForFundlineA, validTLevelLearnersForFundlineA);

                var validProgrammeLearnersForFundlineB = filteredFm25Learners?.Where(x => StudyProgrammePredicate(x) && x.FundLine == FundLineConstants.StudentsExcludingHighNeeds1619 ||
                                                                                   x.FundLine == FundLineConstants.HighNeedsStudents1619).ToList();
                var validTLevelLearnersForFundlineB = filteredFm25Learners?.Where(x => TLevelPredicate(x) && x.FundLine == FundLineConstants.StudentsExcludingHighNeeds1619 ||
                                                                                   x.FundLine == FundLineConstants.HighNeedsStudents1619).ToList();
                model.StudentsIncludingHNS = BuildFundlineReprtingBandModelForProgrammes(validProgrammeLearnersForFundlineB, validTLevelLearnersForFundlineB);

                var validProgrammeLearnersForFundlineC = filteredFm25Learners?.Where(x => StudyProgrammePredicate(x) && x.FundLine == FundLineConstants.StudentsWithEHCP1924).ToList();
                var validTLevelLearnersForFundlineC = filteredFm25Learners?.Where(x => TLevelPredicate(x) && x.FundLine == FundLineConstants.StudentsWithEHCP1924).ToList();
                model.StudentsWithEHCPlan = BuildFundlineReprtingBandModelForProgrammes(validProgrammeLearnersForFundlineC, validTLevelLearnersForFundlineC);

                var validProgrammeLearnersForFundlineD = filteredFm25Learners?.Where(x => StudyProgrammePredicate(x) && x.FundLine == FundLineConstants.ContinuingStudents19Plus).ToList();
                var validTLevelLearnersForFundlineD = filteredFm25Learners?.Where(x => TLevelPredicate(x) && x.FundLine == FundLineConstants.ContinuingStudents19Plus).ToList();
                model.ContinuingStudentsExcludingEHCPlan = BuildFundlineReprtingBandModelForProgrammes(validProgrammeLearnersForFundlineD, validTLevelLearnersForFundlineD);
            }
        }

        public IQueryable<FM25Learner> ApplyFm25LearnersFilter(IQueryable<FM25Learner> learners, IEnumerable<string> learnRefNumbers)
        {
            var learnRefNumbersArray = learnRefNumbers.ToArray();

            return learners?
                .Where(x =>  
                    FilterStartFund(x.StartFund)
                     && FilterFundLine(x.FundLine)
                     && learnRefNumbersArray.Contains(x.LearnRefNumber));
        }

        public IQueryable<FM25Learner> ApplyUserFilters(IQueryable<FM25Learner> learners, DateTime? referenceDate)
        {
            return referenceDate.HasValue
                ? learners?.Where(l => l.LearnerStartDate <= referenceDate)
                : learners;
        }

        public IEnumerable<ILearner> FilterLearners(IEnumerable<ILearner> learners)
        {
            return learners?
                .Where(x => 
                    x.LearningDeliveries != null &&
                    x.LearningDeliveries.Any(ld =>
                        ld.FundModel == FundModelConstants.FM25 &&
                        ld.LearningDeliveryFAMs != null &&
                        ld.LearningDeliveryFAMs
                            .Any(
                                fam => 
                                    fam.LearnDelFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.SOF)
                                    && fam.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMCodeConstants.SOF_ESFA_1619))));
        }

        public bool ValidOnProgPayment(FM25Learner fm25Learner) => fm25Learner.OnProgPayment != null;

        public bool Band9(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals(SixteenToNineteenFundingClaimConstants.Band9);

        public bool Band8(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals(SixteenToNineteenFundingClaimConstants.Band8);

        public bool Band7(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals(SixteenToNineteenFundingClaimConstants.Band7);

        public bool Band6(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals(SixteenToNineteenFundingClaimConstants.Band6);

        public bool Band5(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals(SixteenToNineteenFundingClaimConstants.Band5);

        public bool Band4a(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals(SixteenToNineteenFundingClaimConstants.Band4a);

        public bool Band4b(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals(SixteenToNineteenFundingClaimConstants.Band4b);

        public bool Band3(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals(SixteenToNineteenFundingClaimConstants.Band3);

        public bool Band2(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals(SixteenToNineteenFundingClaimConstants.Band2);

        public bool Band1(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals(SixteenToNineteenFundingClaimConstants.Band1);

        public string IlrFilename(string originalFilename) => ExtractFileName(originalFilename);

        private FundingLineReportingBandModel BuildFundlineReprtingBandModelForProgrammes(List<FM25Learner> fm25ProgrammeLearners, List<FM25Learner> fm25TLevelLearners)
        {
            var programmeOnProgPaymentsLearners = fm25ProgrammeLearners.Where(ValidOnProgPayment).ToList();
            var tLevelOnProgPaymentsLearners = fm25TLevelLearners.Where(ValidOnProgPayment).ToList();

            var model = new FundingLineReportingBandModel()
            {
                Band9StudentNumbers = fm25TLevelLearners.Count(Band9),
                Band9TotalFunding = SumOnProgPaymentsForRateBand(tLevelOnProgPaymentsLearners, Band9),
                Band8StudentNumbers = fm25TLevelLearners.Count(Band8),
                Band8TotalFunding = SumOnProgPaymentsForRateBand(tLevelOnProgPaymentsLearners, Band8),
                Band7StudentNumbers = fm25TLevelLearners.Count(Band7),
                Band7TotalFunding = SumOnProgPaymentsForRateBand(tLevelOnProgPaymentsLearners, Band7),
                Band6StudentNumbers = fm25TLevelLearners.Count(Band6),
                Band6TotalFunding = SumOnProgPaymentsForRateBand(tLevelOnProgPaymentsLearners, Band6),
                Band5StudentNumbers = fm25ProgrammeLearners.Count(Band5),
                Band5TotalFunding = SumOnProgPaymentsForRateBand(programmeOnProgPaymentsLearners, Band5),
                Band4aStudentNumbers = fm25ProgrammeLearners.Count(Band4a),
                Band4aTotalFunding = SumOnProgPaymentsForRateBand(programmeOnProgPaymentsLearners, Band4a),
                Band4bStudentNumbers = fm25ProgrammeLearners.Count(Band4b),
                Band4bTotalFunding = SumOnProgPaymentsForRateBand(programmeOnProgPaymentsLearners, Band4b),
                Band3StudentNumbers = fm25ProgrammeLearners.Count(Band3),
                Band3TotalFunding = SumOnProgPaymentsForRateBand(programmeOnProgPaymentsLearners, Band3),
                Band2StudentNumbers = fm25ProgrammeLearners.Count(Band2),
                Band2TotalFunding = SumOnProgPaymentsForRateBand(programmeOnProgPaymentsLearners, Band2),
                Band1StudentNumbers = fm25ProgrammeLearners.Count(Band1),
                Band1TotalFunding = SumOnProgPaymentsForRateBand(programmeOnProgPaymentsLearners, Band1),
            };

            return model;
        }

        private decimal SumOnProgPaymentsForRateBand(List<FM25Learner> learners, Func<FM25Learner, bool> rateBand)
        {
            return learners.Where(rateBand).Sum(x => x.OnProgPayment.Value);
        }
    }
}
