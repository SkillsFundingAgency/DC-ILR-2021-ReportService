using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Models.Fm35;
using ESFA.DC.ILR.ReportService.Models.Ilr;
using ESFA.DC.ILR.ReportService.Models.ReferenceData;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.DevolvedPostcodes;
using ESFA.DC.ILR.ReportService.Models.ReferenceData.MCAGLA;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Extensions;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.NonContractDevolved
{
    public class NonContractDevolvedAdultEducationOccupancyReportModelBuilder : AbstractOccupancyReportModelBuilder, IModelBuilder<IEnumerable<NonContractDevolvedAdultEducationOccupancyReportModel>>
    {
        private readonly HashSet<int> _categoryRefs = new HashSet<int> { 37, 38 };

        private readonly IEnumerable<string> _sofLearnDelFamCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.SOF_GreaterManchesterCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_LiverpoolCityRegionCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_WestMidlandsCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_WestOfEnglandCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_TeesValleyCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_CambridgeshireAndPeterboroughCombinedAuthority,
            LearningDeliveryFAMCodeConstants.SOF_GreaterLondonAuthority,
            LearningDeliveryFAMCodeConstants.SOF_NorthOfTyneCombinedAuhority,
        };

        private readonly IAcademicYearService _academicYearService;

        public NonContractDevolvedAdultEducationOccupancyReportModelBuilder(IIlrModelMapper ilrModelMapper, IAcademicYearService academicYearService)
            : base(ilrModelMapper)
        {
            _academicYearService = academicYearService;
        }

        public IEnumerable<NonContractDevolvedAdultEducationOccupancyReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            var message = reportServiceDependentData.Get<IMessage>();
            var fm35 = reportServiceDependentData.Get<FM35Global>();
            var referenceData = reportServiceDependentData.Get<ReferenceDataRoot>();
            var sofCodesDictionary = BuildSofDictionary(referenceData.DevolvedPostocdes.McaGlaSofLookups);

            var larsLearningDeliveries = BuildLarsLearningDeliveryDictionary(referenceData);
            var fm35LearningDeliveries = BuildFm35LearningDeliveryDictionary(fm35);
            var organisations = referenceData.Organisations.ToDictionary(x => x.UKPRN, x => x.Name);

            var devolvedContracts = referenceData.McaDevolvedContracts?.Where(mdc => mdc.Ukprn == reportServiceContext.Ukprn) ?? Enumerable.Empty<McaDevolvedContract>();

            var models = new List<NonContractDevolvedAdultEducationOccupancyReportModel>();

            foreach (var learner in message?.Learners?.Where(l => l != null) ?? Enumerable.Empty<ILearner>())
            {
                foreach (var learningDelivery in learner.LearningDeliveries?.Where(LearningDeliveryReportFilter) ?? Enumerable.Empty<ILearningDelivery>())
                {
                    var learningDeliveryFams = _ilrModelMapper.MapLearningDeliveryFAMs(learningDelivery.LearningDeliveryFAMs);
                    var mcaGlaShortCode = sofCodesDictionary.GetValueOrDefault(learningDeliveryFams.SOF);

                    var filteredDevolvedContract = devolvedContracts.Where(c => c.McaGlaShortCode.CaseInsensitiveEquals(mcaGlaShortCode));

                    if (HasValidContract(learningDelivery, filteredDevolvedContract))
                    {
                        continue;
                    }

                    var fm35LearningDelivery = fm35LearningDeliveries.GetValueOrDefault(learner.LearnRefNumber).GetValueOrDefault(learningDelivery.AimSeqNumber);
                    var larsLearningDelivery = larsLearningDeliveries.GetValueOrDefault(learningDelivery.LearnAimRef);
                    var providerSpecLearnerMonitoring = _ilrModelMapper.MapProviderSpecLearnerMonitorings(learner.ProviderSpecLearnerMonitorings);
                    var providerSpecDeliveryMonitoring = _ilrModelMapper.MapProviderSpecDeliveryMonitorings(learningDelivery.ProviderSpecDeliveryMonitorings);                   
                    var periodisedValues = BuildFm35PeriodisedValuesModel(fm35LearningDelivery?.LearningDeliveryPeriodisedValues);
                    var entitlementValue = larsLearningDelivery.LARSLearningDeliveryCategories.Any(c =>
                        (c.EffectiveFrom <= learningDelivery.LearnStartDate && (c.EffectiveTo ?? DateTime.MaxValue) >=
                            learningDelivery.LearnStartDate) && _categoryRefs.Contains(c.CategoryRef))
                        ? ReportingConstants.Yes
                        : ReportingConstants.No;
                    var learnerEmploymentStatus = learner.LearnerEmploymentStatuses?.Where(l => l.DateEmpStatApp <= learningDelivery.LearnStartDate).OrderByDescending(d => d.DateEmpStatApp).FirstOrDefault() ?? new MessageLearnerLearnerEmploymentStatus();
                    var employmentStatusMonitorings = _ilrModelMapper.MapEmploymentStatusMonitorings(learnerEmploymentStatus.EmploymentStatusMonitorings);
                    var partnerProvider = organisations.GetValueOrDefault(learningDelivery.PartnerUKPRNNullable.GetValueOrDefault());

                    var lsdPostcode = referenceData.Postcodes?.FirstOrDefault(p => p.PostCode.CaseInsensitiveEquals(learningDelivery.LSDPostcode));
                    var localAuthorityCode = lsdPostcode?.ONSData
                        .FirstOrDefault(od => learningDelivery.LearnStartDate >= od.EffectiveFrom && learningDelivery.LearnStartDate <= (od.EffectiveTo ?? DateTime.MaxValue))?
                        .LocalAuthority;

                    models.Add(new NonContractDevolvedAdultEducationOccupancyReportModel()
                    {
                        Learner = learner,
                        ProviderSpecLearnerMonitoring = providerSpecLearnerMonitoring,
                        LearningDelivery = learningDelivery,
                        ProviderSpecDeliveryMonitoring = providerSpecDeliveryMonitoring,
                        LearningDeliveryFAMs = learningDeliveryFams,
                        Fm35LearningDelivery = fm35LearningDelivery?.LearningDeliveryValue,
                        LarsLearningDelivery = larsLearningDelivery,
                        PeriodisedValues = periodisedValues,
                        McaGlaShortCode = mcaGlaShortCode,
                        EntitlementCategoryLevel2Or3 = entitlementValue,
                        LearnerEmploymentStatus = learnerEmploymentStatus,
                        EmploymentStatusMonitorings = employmentStatusMonitorings,
                        PartnershipProviderName = partnerProvider,
                        LocalAuthorityCode = localAuthorityCode
                    });
                }
            }

            return Order(models);
        }

        public bool HasValidContract(ILearningDelivery learningDelivery, IEnumerable<McaDevolvedContract> contracts)
        {
            var learnStartDate = learningDelivery.LearnStartDate;
            var learnActEndDate = learningDelivery.LearnActEndDateNullable;
            var learnPlanEndDate = learningDelivery.LearnPlanEndDate;

            foreach (var contract in contracts)
            {
                if (!(learnStartDate >= contract.EffectiveFrom && (!contract.EffectiveTo.HasValue || learnStartDate <= contract.EffectiveTo)))
                {
                    continue;
                }

                return LearnEndDateCondition(learnActEndDate, contract, learnPlanEndDate);
            }

            return false;
        }

        private bool LearnEndDateCondition(DateTime? learnActEndDate, McaDevolvedContract contract, DateTime learnPlanEndDate)
        {
            bool learnEndDateCondition = true;

            if (learnActEndDate.HasValue)
            {
                learnEndDateCondition = (!contract.EffectiveTo.HasValue || learnActEndDate <= contract.EffectiveTo);
            }
            else
            {
                if (!contract.EffectiveTo.HasValue || contract.EffectiveTo < _academicYearService.YearEnd)
                {
                    learnEndDateCondition = (!contract.EffectiveTo.HasValue
                                             || learnPlanEndDate <= contract.EffectiveTo)
                                             || contract.EffectiveTo.Value.Year >= _academicYearService.YearEnd.Year 
                                                     && contract.EffectiveTo.Value.DayOfYear >= _academicYearService.YearEnd.DayOfYear 
                                                     && learnPlanEndDate >= contract.EffectiveTo;
                }
            }

            return learnEndDateCondition;
        }

        public bool LearningDeliveryReportFilter(ILearningDelivery learningDelivery)
        {
            return learningDelivery
                .LearningDeliveryFAMs?
                .Any(ldfam => 
                    ldfam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.SOF)
                    && _sofLearnDelFamCodes.Contains(ldfam.LearnDelFAMCode))
                ?? false;
        }

        public IDictionary<string, string> BuildSofDictionary(IEnumerable<McaGlaSofLookup> mcaGlaSofLookups)
        {
            return mcaGlaSofLookups
               .Where(s => _sofLearnDelFamCodes.Contains(s.SofCode)
                && s.EffectiveFrom <= _academicYearService.YearStart
                && (!s.EffectiveTo.HasValue || _academicYearService.YearEnd.Date <= s.EffectiveTo))
               .ToDictionary(s => s.SofCode, s => s.McaGlaShortCode, StringComparer.OrdinalIgnoreCase);
        }
    }
}
