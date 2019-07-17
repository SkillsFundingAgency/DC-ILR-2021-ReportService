using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy.Model;
using ESFA.DC.ILR.ReportService.Service.Interface;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.DevolvedOccupancy
{
    class DevolvedAdultEducationOccupancyReportModelBuilder : IModelBuilder<IEnumerable<DevolvedAdultEducationOccupancyReportModel>>
    {
        public IEnumerable<DevolvedAdultEducationOccupancyReportModel> Build(IReportServiceContext reportServiceContext, IReportServiceDependentData reportServiceDependentData)
        {
            return new List<DevolvedAdultEducationOccupancyReportModel>();
        }
    }
}
