using System;
using System.Collections.Generic;
using ESFA.DC.Data.DAS.Model;
using ESFA.DC.ILR1819.ReportService.Model.DasCommitments;

namespace ESFA.DC.ILR1819.ReportService.Interface.Builders
{
    public interface IDasCommitmentBuilder
    {
        List<DasCommitment> Build(IEnumerable<DasCommitments> commitments);

        IReadOnlyList<DasCommitment> ActiveCommitmentsForDate(IEnumerable<DasCommitment> commitments, DateTime date);

        IReadOnlyList<DasCommitment> NonActiveCommitmentsForDate(IEnumerable<DasCommitment> commitments, DateTime date);
    }
}
