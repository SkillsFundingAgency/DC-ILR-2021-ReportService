using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ReportService.Reports.Funding.CommunityLearning.Model.Interface
{
    public interface ICommunityLearningReport
    {
        List<ICategory> Categories { get; }
    }
}
