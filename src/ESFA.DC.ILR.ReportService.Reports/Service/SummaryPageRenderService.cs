using Aspose.Cells;
using ESFA.DC.ILR.ReportService.Reports.Constants;
using ESFA.DC.ILR.ReportService.Reports.Model.Interface;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Service
{
    public class SummaryPageRenderService : IRenderService<ISummaryPage>
    {
        private const string ProviderName = "Provider Name:";
        private const string UKPRN = "UKPRN:";
        private const string ILRFile = "ILR File:";
        private const string LastILRFileUpdate = "Last ILR File Update:";
        private const string LastEASUpdate = "Last EAS Update:";
        private const string SecurityClassification = "Security Classification:";

        private const string ApplicationVersion = "Application Version:";
        private const string FilePreparationDate = "File Preparation Date:";
        private const string LARSVersion = "LARS Data:";
        private const string PostcodeVersion = "Postcode Data:";
        private const string OrganisationVersion = "Organisation Data:";
        private const string LargeEmployersVersion = "Large Employers Data:";
        private const string ReportGeneratedAt = "Report Generated at:";

        private readonly Style _defaultStyle;

        public SummaryPageRenderService()
        {
            var cellsFactory = new CellsFactory();
            _defaultStyle = cellsFactory.CreateStyle();
            ConfigureStyles();
        }

        public Worksheet Render(ISummaryPage model, Worksheet worksheet)
        {
            worksheet.Workbook.DefaultStyle = _defaultStyle;
            worksheet.Cells.StandardWidth = 20;
            worksheet.Cells.Columns[0].Width = 65;

            worksheet.Cells.ImportTwoDimensionArray(new object[,]
            {
                { ProviderName, model.ProviderName },
                { UKPRN, model.UKPRN },
                { ILRFile, model.ILRFile },
                { LastILRFileUpdate, model.LastILRFileUpdate },
                { LastEASUpdate, model.LastEASUpdate }, 
                { SecurityClassification, ReportingConstants.OfficialSensitive },
                { ApplicationVersion, model.ApplicationVersion },
                { FilePreparationDate, model.FilePreparationDate },
                { LARSVersion, model.LARSVersion },
                { PostcodeVersion, model.PostcodeVersion },
                { OrganisationVersion, model.OrganisationVersion },
                { LargeEmployersVersion, model.LargeEmployersVersion },
                { ReportGeneratedAt, model.ReportGeneratedAt }
            }, 0, 0);

            return worksheet;
        }

        private void ConfigureStyles()
        {
            _defaultStyle.Font.Size = 10;
            _defaultStyle.Font.Name = "Arial";
            _defaultStyle.Font.IsBold = true;
        }
    }
}
