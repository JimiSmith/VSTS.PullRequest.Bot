using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace VSTS.PullRequest.Bot.Models
{
    // PartitionKey is instance
    // RowKey is project id
    public class ProjectEntity : TableEntity
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTimeOffset? ExpiresAt { get; set; }

        public string AuthState { get; set; }

        public bool SubscribedToEvents { get; set; }
    }
}
