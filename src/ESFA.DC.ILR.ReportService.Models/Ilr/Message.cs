using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ReportService.Models.Ilr
{
    public class Message : IMessage
    {
        public IHeader HeaderEntity { get; set; }

        public IReadOnlyCollection<ILearner> Learners => LearnersData;

        public List<MessageLearner> LearnersData { get; set; }

        public IReadOnlyCollection<ISourceFile> SourceFilesCollection => throw new System.NotImplementedException();

        public ILearningProvider LearningProviderEntity => throw new System.NotImplementedException();

        public IReadOnlyCollection<ILearnerDestinationAndProgression> LearnerDestinationAndProgressions => throw new System.NotImplementedException();

    }
}
