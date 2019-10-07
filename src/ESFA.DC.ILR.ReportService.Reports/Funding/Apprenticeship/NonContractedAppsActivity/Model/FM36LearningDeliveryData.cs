using System;
using System.Collections;
using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReportService.Reports.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Apprenticeship.NonContractedAppsActivity.Model
{
    public class FM36LearningDeliveryData
    {
        public ILearningDelivery LearningDelivery { get; set; }

        public ProviderSpecDeliveryMonitoringModel ProviderSpecDeliveryMonitoringModels { get; set; }

        public LearningDeliveryFAMsModel LearningDeliveryFAMsModels { get; set; }

        public int AimSeqNumber => LearningDelivery.AimSeqNumber;

        public string LearnAimRef => LearningDelivery.LearnAimRef;

        public string SWSupAimId => LearningDelivery.SWSupAimId;

        public int? ProgType => LearningDelivery.ProgTypeNullable;

        public int? StdCode => LearningDelivery.StdCodeNullable;

        public int? FworkCode => LearningDelivery.FworkCodeNullable;

        public int? PwayCode => LearningDelivery.PwayCodeNullable;

        public int AimType => LearningDelivery.AimType;

        public string FundingStreamPeriodCode { get; set; }

        public DateTime? OrigLearnStartDate => LearningDelivery.OrigLearnStartDateNullable;

        public DateTime LearnStartDate => LearningDelivery.LearnStartDate;

        public DateTime LearnPlanEndDate => LearningDelivery.LearnPlanEndDate;

        public DateTime? LearnActEndDate => LearningDelivery.LearnActEndDateNullable;

        public DateTime? AchDate => LearningDelivery.AchDateNullable;
      
        public string LearnDelFAM_EEF => LearningDeliveryFAMsModels.EEF;

        public bool LearnDelMathEng => FM36LearningDelivery.LearningDeliveryValues.LearnDelMathEng == true ? true : false;

        public ICollection<ILearningDeliveryFAM> LearningDeliveryFAMs_ACT { get; set; }

        public ICollection<FM36PriceEpisodeValue> FM36PriceEpisodes { get; set; }

        public FM36LearningDeliveryValue FM36LearningDelivery { get; set; }
    }
}
