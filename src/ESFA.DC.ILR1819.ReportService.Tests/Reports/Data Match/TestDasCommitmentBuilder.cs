using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Data.DAS.Model;
using ESFA.DC.ILR1819.ReportService.Interface.Builders;
using ESFA.DC.ILR1819.ReportService.Model.DasCommitments;
using ESFA.DC.ILR1819.ReportService.Service.Builders;
using Xunit;

namespace ESFA.DC.ILR1819.ReportService.Tests.Reports.Data_Match
{
    public sealed class TestDasCommitmentBuilder
    {
        [Fact]
        public async Task TestDasCommitmentBuilderBuild()
        {
            List<DasCommitments> dasCommitments = new List<DasCommitments>
            {
                new DasCommitments
                {
                    CommitmentId = 1,
                    VersionId = "2"
                },
                new DasCommitments
                {
                    CommitmentId = 1,
                    VersionId = "2"
                }
            };

            IDasCommitmentBuilder dasCommitmentBuilder = new DasCommitmentBuilder();
            List<DasCommitment> commitments = dasCommitmentBuilder.Build(dasCommitments);

            Assert.Equal(2, commitments.Count);
            Assert.True(commitments[0].IsVersioned);
        }
    }
}
