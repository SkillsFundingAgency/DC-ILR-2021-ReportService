namespace ESFA.DC.ILR.ReportService.Reports.Abstract
{
    public class AbstractReportHeaderFooterModel
    {
        //Header
        public string ProviderName { get; set; }

        public int Ukprn { get; set; }

        public string IlrFile { get; set; }

        public string Year { get; set; }

        // footer
        public string ComponentSetVersion { get; set; }

        public string ApplicationVersion { get; set; }

        public string FilePreparationDate { get; set; }

        public string LarsData { get; set; }

        public string PostcodeData { get; set; }

        public string OrganisationData { get; set; }

        public string LargeEmployerData { get; set; }

        public string ReportGeneratedAt { get; set; }

        public string CofRemovalData { get; set; }

        public string CampusIdData { get; set; }

        public string LastILRFileUpdate { get; set; }

        public string LastEASFileUpdate { get; set; }

    }
}
