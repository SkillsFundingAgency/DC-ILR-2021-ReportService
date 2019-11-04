using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReportService.Models.Fm25;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.Abstract;

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

            var referenceDateFilter = RetrieveReportFilterValueFromContext<DateTime?>(reportServiceContext, FundingClaimReport.ReportNameConstant, FundingClaimReport.ReferenceDateFilterPropertyName);

            var organisation = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn);
            var organisationName = organisation?.Name ?? string.Empty;
            var cofRemoval = organisation?.OrganisationCoFRemovals?.OrderByDescending(x => x.EffectiveFrom).FirstOrDefault()?.CoFRemoval;
            var learners = message?.Learners ?? Enumerable.Empty<ILearner>();

            var model = new FundingClaimReportModel();

            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);
            var reportGeneratedAt = "Report generated at: " + FormatReportGeneratedAtDateTime(dateTimeNowUk);

            // Header
            model.ProviderName = organisationName;
            model.Ukprn = reportServiceContext.Ukprn.ToString();
            model.IlrFile = IlrFilename(reportServiceContext.OriginalFilename);
            model.Year = ReportingConstants.Year;
            model.CofRemoval = -cofRemoval.GetValueOrDefault();
            model.ReferenceDate = referenceDateFilter.HasValue ? referenceDateFilter.Value.ShortDateStringFormat() : "(ALL)";

            // Body
            var applicableLearners = FilterLearners(learners);
            var learnersArray = applicableLearners.Select(x => x.LearnRefNumber);


            var fm25LearnersQueryable = fm25Data?.Learners?.AsQueryable() ?? Enumerable.Empty<FM25Learner>().AsQueryable();
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
                    PrvRetentFactHist = fm25Learner.PrvRetentFactHist.GetValueOrDefault(0).ToString("N5")
                };

                var validLearnersForFundlineA = filteredFm25Learners?.Where(x => x.FundLine == FundLineConstants.DirectFundedStudents1416).ToList();
                model.DirectFundingStudents = BuildFundlineReprtingBandModel(validLearnersForFundlineA);

                var validLearnersForFundlineB = filteredFm25Learners?.Where(x => x.FundLine == FundLineConstants.StudentsExcludingHighNeeds1619 ||
                                                                                   x.FundLine == FundLineConstants.HighNeedsStudents1619).ToList();
                model.StudentsIncludingHNS = BuildFundlineReprtingBandModel(validLearnersForFundlineB);

                var validLearnersForFundlineC = filteredFm25Learners?.Where(x => x.FundLine == FundLineConstants.StudentsWithEHCP1924).ToList();
                model.StudentsWithEHCPlan = BuildFundlineReprtingBandModel(validLearnersForFundlineC);

                var validLearnersForFundlineD = filteredFm25Learners?.Where(x => x.FundLine == FundLineConstants.ContinuingStudents19Plus).ToList();
                model.ContinuingStudentsExcludingEHCPlan = BuildFundlineReprtingBandModel(validLearnersForFundlineD);
            }

            // Footer
            model.ReportGeneratedAt = reportGeneratedAt;
            model.ApplicationVersion = reportServiceContext.ServiceReleaseVersion;
            model.ComponentSetVersion = "NA";
            model.OrganisationData = referenceDataRoot.MetaDatas.ReferenceDataVersions.OrganisationsVersion.Version;
            model.LargeEmployerData = referenceDataRoot.MetaDatas.ReferenceDataVersions.Employers.Version;
            model.LarsData = referenceDataRoot.MetaDatas.ReferenceDataVersions.LarsVersion.Version;
            model.PostcodeData = referenceDataRoot.MetaDatas.ReferenceDataVersions.PostcodesVersion.Version;
            model.FilePreparationDate = FormatFilePreparationDate(message.HeaderEntity.CollectionDetailsEntity.FilePreparationDate);
            model.CofRemovalData = referenceDataRoot.MetaDatas.ReferenceDataVersions.CoFVersion.Version;

            return model;
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

        public bool Band5(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals("540+ hours (Band 5)");

        public bool Band4a(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals("450+ hours (Band 4a)");

        public bool Band4b(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals("450 to 539 hours (Band 4b)");

        public bool Band3(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals("360 to 449 hours (Band 3)");

        public bool Band2(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals("280 to 359 hours (Band 2)");

        public bool Band1(FM25Learner fm25Learner) => fm25Learner.RateBand.CaseInsensitiveEquals("Up to 279 hours (Band 1)");

        public string IlrFilename(string originalFilename) => ExtractFileName(originalFilename);

        private FundingLineReportingBandModel BuildFundlineReprtingBandModel(List<FM25Learner> fm25Learners)
        {
            var model = new FundingLineReportingBandModel()
            {
                Band5StudentNumbers = fm25Learners.Count(Band5),
                Band5TotalFunding = fm25Learners.Where(Band5).Sum(x => x.OnProgPayment.GetValueOrDefault()),
                Band4aStudentNumbers = fm25Learners.Count(Band4a),
                Band4aTotalFunding = fm25Learners.Where(Band4a).Sum(x => x.OnProgPayment.GetValueOrDefault()),
                Band4bStudentNumbers = fm25Learners.Count(Band4b),
                Band4bTotalFunding = fm25Learners.Where(Band4b).Sum(x => x.OnProgPayment.GetValueOrDefault()),
                Band3StudentNumbers = fm25Learners.Count(Band3),
                Band3TotalFunding = fm25Learners.Where(Band3).Sum(x => x.OnProgPayment.GetValueOrDefault()),
                Band2StudentNumbers = fm25Learners.Count(Band2),
                Band2TotalFunding = fm25Learners.Where(Band2).Sum(x => x.OnProgPayment.GetValueOrDefault()),
                Band1StudentNumbers = fm25Learners.Count(Band1),
                Band1TotalFunding = fm25Learners.Where(Band1).Sum(x => x.OnProgPayment.GetValueOrDefault()),
            };
            return model;
        }

    }
}
