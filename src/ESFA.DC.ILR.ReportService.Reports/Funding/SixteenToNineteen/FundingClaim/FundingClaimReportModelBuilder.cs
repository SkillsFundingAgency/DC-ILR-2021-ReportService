﻿using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.FundingService.FM25.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim.Model;
using ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentSummary.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.FundingClaim
{
    public class FundingClaimReportModelBuilder : IModelBuilder<FundingClaimReportModel>
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private const string ReportGeneratedTimeStringFormat = "HH:mm:ss on dd/MM/yyyy";

        private readonly string[] _applicableFundingLineTypes = new string[]
        {
            "14-16 Direct Funded Students",
            "16-19 Students (excluding High Needs Students)",
            "16-19 High Needs Students",
            "19-24 Students with an EHCP",
            "19+ Continuing Students (excluding EHCP)"
        };

        public FundingClaimReportModelBuilder(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }
        public FundingClaimReportModel Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm25Data = reportServiceDependentData.Get<FM25Global>();
            var referenceDataRoot = reportServiceDependentData.Get<ReferenceDataRoot>();

            var organisationName = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.Name ?? string.Empty;
            var ilrFileName = reportServiceContext.OriginalFilename ?? reportServiceContext.Filename;
            var cofRemoval = referenceDataRoot.Organisations.FirstOrDefault(o => o.UKPRN == reportServiceContext.Ukprn)?.OrganisationCoFRemovals?.OrderByDescending(x => x.EffectiveFrom).FirstOrDefault()?.CoFRemoval;
            var learners = message?.Learners ?? Enumerable.Empty<ILearner>();

            var model = new FundingClaimReportModel();

            DateTime dateTimeNowUtc = _dateTimeProvider.GetNowUtc();
            DateTime dateTimeNowUk = _dateTimeProvider.ConvertUtcToUk(dateTimeNowUtc);
            var reportGeneratedAt = "Report generated at: " + dateTimeNowUk.ToString(ReportGeneratedTimeStringFormat);

            // Header
            model.ProviderName = organisationName;
            model.Ukprn = reportServiceContext.Ukprn;
            model.IlrFile = ilrFileName;
            model.Year = ReportingConstants.Year;
            model.CofRemoval = -cofRemoval.GetValueOrDefault();

            // Body
            var applicableLearners = FilterLearners(learners);
            var learnersArray = applicableLearners.Select(x => x.LearnRefNumber).ToArray();

            var applicableFm25Learners = FilterFm25Learners(fm25Data, learnersArray);

            if (applicableFm25Learners != null)
            {
                var fm25Learner = applicableFm25Learners.First();

                model.FundingFactor = new FundingFactorModel
                {
                    AreaCostFact1618Hist = fm25Learner.AreaCostFact1618Hist.GetValueOrDefault(0).ToString("N5"),
                    ProgWeightHist = fm25Learner.ProgWeightHist.GetValueOrDefault(0).ToString("N5"),
                    PrvDisadvPropnHist = fm25Learner.PrvDisadvPropnHist.GetValueOrDefault(0).ToString("N5"),
                    PrvHistLrgProgPropn = fm25Learner.PrvHistLrgProgPropn.GetValueOrDefault(0).ToString("N5"),
                    PrvRetentFactHist = fm25Learner.PrvRetentFactHist.GetValueOrDefault(0).ToString("N5")
                };

                var validLearnersForFundlineA = applicableFm25Learners?.Where(x => x.FundLine == FundLineConstants.DirectFundedStudents1416).ToList();
                model.DirectFundingStudents = BuildFundlineReprtingBandModel(validLearnersForFundlineA);

                var validLearnersForFundlineB = applicableFm25Learners?.Where(x => x.FundLine == FundLineConstants.StudentsIncludingHighNeeds1619).ToList();
                model.StudentsIncludingHNS = BuildFundlineReprtingBandModel(validLearnersForFundlineB);

                var validLearnersForFundlineC = applicableFm25Learners?.Where(x => x.FundLine == FundLineConstants.StudentsWithEHCP1924).ToList();
                model.StudentsWithEHCPlan = BuildFundlineReprtingBandModel(validLearnersForFundlineC);

                var validLearnersForFundlineD = applicableFm25Learners?.Where(x => x.FundLine == FundLineConstants.ContinuingStudents19Plus).ToList();
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
            model.FilePreparationDate = message?.HeaderEntity.CollectionDetailsEntity.FilePreparationDate.ToString("dd/MM/yyyy");

            return model;
        }

        private List<FM25Learner> FilterFm25Learners(FM25Global fm25Data, string[] learnersArray)
        {
            return fm25Data.Learners?.Where(x => x.StartFund.GetValueOrDefault()
                                                 && _applicableFundingLineTypes.Contains(x.FundLine)
                                                 && learnersArray.Contains(x.LearnRefNumber))?.ToList();
        }

        private IEnumerable<ILearner> FilterLearners(IEnumerable<ILearner> learners)
        {
            return learners.Where(x => x.LearningDeliveries.Any(ld =>
                ld.FundModel == 25 &&
                ld.LearningDeliveryFAMs.Any(fam => fam.LearnDelFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.SOF)) &&
                ld.LearningDeliveryFAMs.Any(fam => fam.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMCodeConstants.SOF_ESFA_1619)))).ToList();
        }

        private FundingLineReportingBandModel BuildFundlineReprtingBandModel(List<FM25Learner> fm25Learners)
        {
            var model = new FundingLineReportingBandModel()
            {
                Band5StudentNumbers = fm25Learners.Count(Band5),
                Band5TotalFunding = fm25Learners.Where(Band5).Sum(x => x.OnProgPayment.GetValueOrDefault()),
                Band4aStudentNumbers = fm25Learners.Count(Band4a),
                Band4aTotalFunding = fm25Learners.Where(Band4a).Sum(x => x.OnProgPayment.GetValueOrDefault()),
                Band4bStudentNumbers = fm25Learners.Count(Band4a),
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

        private bool Band5(FM25Learner fm25Learner)
        {
            return fm25Learner.RateBand.CaseInsensitiveEquals("540+ hours (Band 5)");
        }

        private bool Band4a(FM25Learner fm25Learner)
        {
            return fm25Learner.RateBand.CaseInsensitiveEquals("450+ hours (Band 4a)");
        }

        private bool Band4b(FM25Learner fm25Learner)
        {
            return fm25Learner.RateBand.CaseInsensitiveEquals("450 to 539 hours (Band 4b)");
        }

        private bool Band3(FM25Learner fm25Learner)
        {
            return fm25Learner.RateBand.CaseInsensitiveEquals("360 to 449 hours (Band 3)");
        }

        private bool Band2(FM25Learner fm25Learner)
        {
            return fm25Learner.RateBand.CaseInsensitiveEquals("280 to 359 hours (Band 2)");
        }

        private bool Band1(FM25Learner fm25Learner)
        {
            return fm25Learner.RateBand.CaseInsensitiveEquals("Up to 279 hours (Band 1)");
        }
    }
}
