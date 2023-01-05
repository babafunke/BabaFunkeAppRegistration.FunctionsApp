using Azure;
using Azure.Data.Tables;
using System;

namespace Shared
{
    public class AppUserEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = StringConstants.AppUserPartitionKey;
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.Now;
        public ETag ETag { get; set; } = ETag.All;
        public string Username { get; set; }
        public bool EmailConfirmed { get; set; }
        public string OrchestrationId { get; set; }
        public string ReferenceCode { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}