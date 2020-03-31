using CsvHelper.Configuration;
using ESFA.DC.ILR.ReportService.Reports.Service.Converters;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.SixteenToNineteen.HighNeedsStudentDetail
{
    public class HighNeedsStudentDetailReportClassMap : ClassMap<HighNeedsStudentDetailReportModel>
    {
        public HighNeedsStudentDetailReportClassMap()
        {
            var index = 0;
            
            Map(m => m.DerivedFundline).Name(@"Funding line type").Index(++index);
            Map(m => m.Learner.LearnRefNumber).Name(@"Learner reference number").Index(++index);
            Map(m => m.Learner.FamilyName).Name(@"Family name").Index(++index);
            Map(m => m.Learner.GivenNames).Name(@"Given names").Index(++index);
            Map(m => m.Learner.CampId).Name(@"Campus identifier").Index(++index);
            Map(m => m.StudentsWithAnEhcp).Name(@"A - Students with an EHCP").Index(++index).TypeConverter<IlrBooleanConverter>();
            Map(m => m.StudentsWithoutAnEhcp).Name(@"B - Students without an EHCP").Index(++index).TypeConverter<IlrBooleanConverter>();
            Map(m => m.HighNeedsStudentsWithoutAnEhcp).Name(@"C - High Needs Students (HNS) without an EHCP").Index(++index).TypeConverter<IlrBooleanConverter>();
            Map(m => m.StudentsWithAnEhcpAndHns).Name(@"D - Students with an EHCP and HNS").Index(++index).TypeConverter<IlrBooleanConverter>();
            Map(m => m.StudentWithAnEhcpAndNotHns).Name(@"E - Students with an EHCP but without HNS").Index(++index).TypeConverter<IlrBooleanConverter>();
            Map().Name(@"OFFICIAL-SENSITIVE").Constant(string.Empty).Index(++index);
        }
    }
}
