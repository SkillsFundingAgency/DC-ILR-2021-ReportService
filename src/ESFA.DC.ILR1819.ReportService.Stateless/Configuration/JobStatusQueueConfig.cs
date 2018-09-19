﻿using ESFA.DC.Queueing;

namespace ESFA.DC.ILR1819.ReportService.Stateless.Configuration
{
    public sealed class JobStatusQueueConfig : QueueConfiguration
    {
        public JobStatusQueueConfig(string connectionString, string queueName, int maxConcurrentCalls, string topicName = null, string subscriptionName = null, int minimumBackoffSeconds = 5, int maximumBackoffSeconds = 50, int maximumRetryCount = 10)
            : base(connectionString, queueName, maxConcurrentCalls, topicName, subscriptionName, minimumBackoffSeconds, maximumBackoffSeconds, maximumRetryCount)
        {
        }
    }
}
