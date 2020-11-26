namespace ESFA.DC.ILR.ReportService.Models.ReferenceData.MetaData
{
    public class ReferenceDataVersion
    {
        public CoFVersion CoFVersion { get; set; }

        public CampusIdentifierVersion CampusIdentifierVersion { get; set; }

        public EmployersVersion Employers { get; set; }

        public LarsVersion LarsVersion { get; set; }

        public OrganisationsVersion OrganisationsVersion { get; set; }

        public PostcodesVersion PostcodesVersion { get; set; }

        public DevolvedPostcodesVersion DevolvedPostcodesVersion { get; set; }

        public EasFileDetails EasFileDetails { get; set; }
    }
}
