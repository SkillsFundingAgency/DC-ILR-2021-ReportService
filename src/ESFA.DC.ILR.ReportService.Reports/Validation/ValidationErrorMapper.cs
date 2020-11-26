using CsvHelper.Configuration;
using ESFA.DC.ILR.ReportService.Reports.Validation.Model;

namespace ESFA.DC.ILR.ReportService.Reports.Validation
{
    public sealed class ValidationErrorMapper : ClassMap<ValidationErrorRow>
    {
        public ValidationErrorMapper()
        {
            Map(m => m.Severity).Index(0).Name(@"Error\Warning");
            Map(m => m.LearnerReferenceNumber).Index(1).Name(@"Learner Ref");
            Map(m => m.ULN).Index(2).Name(@"Unique Learner Number");
            Map(m => m.FamilyName).Index(3).Name(@"Family Name");
            Map(m => m.GivenNames).Index(4).Name(@"Given Names");
            Map(m => m.RuleName).Index(5).Name(@"Rule Name");
            Map(m => m.FieldValues).Index(6).Name(@"Field Values");
            Map(m => m.ErrorMessage).Index(7).Name(@"Error Message");
            Map(m => m.AimSequenceNumber).Index(8).Name(@"Aim Sequence Number");
            Map(m => m.LearnAimRef).Index(9).Name(@"Aim Reference Number");
            Map(m => m.SWSupAimId).Index(10).Name(@"Software Supplier Aim ID");
            Map(m => m.FundModel).Index(11).Name(@"Funding Model");
            Map(m => m.PartnerUKPRN).Index(12).Name(@"Subcontracted UKPRN");
            Map(m => m.ProviderSpecLearnOccurA).Index(13).Name(@"Provider Specified Learner Monitoring A");
            Map(m => m.ProviderSpecLearnOccurB).Index(14).Name(@"Provider Specified Learner Monitoring B");
            Map(m => m.ProviderSpecDelOccurA).Index(15).Name(@"Provider Specified Learning Delivery Monitoring A");
            Map(m => m.ProviderSpecDelOccurB).Index(16).Name(@"Provider Specified Learning Delivery Monitoring B");
            Map(m => m.ProviderSpecDelOccurC).Index(17).Name(@"Provider Specified Learning Delivery Monitoring C");
            Map(m => m.ProviderSpecDelOccurD).Index(18).Name(@"Provider Specified Learning Delivery Monitoring D");
            Map(m => m.SecurityClassification).Index(19).Name(@"OFFICIAL - SENSITIVE");
        }
    }
}
