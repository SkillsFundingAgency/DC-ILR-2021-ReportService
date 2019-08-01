using ESFA.DC.ILR.FundingService.FM35.FundingOutput.Model.Output;
using ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Abstract.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.Occupancy.Main
{
    public class MainOccupancyReportModel : AbstractOccupancyReportModel
    {
        public LearningDeliveryValue Fm35LearningDelivery { get; set; }

        public FundModelAgnosticModel FundModelAgnosticModel { get; set; }

        public string TraineeshipWorkPlacementOrWorkPreparation { get; set; }

        public string HigherApprenticeshipPrescribedHeAim { get; set; }
    }
}
