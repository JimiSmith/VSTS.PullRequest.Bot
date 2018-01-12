using Microsoft.WindowsAzure.Storage.Table;

namespace VSTS.PullRequest.ReminderBot
{
    // PartitionKey is instance
    // RowKey is project id
    public class ProjectEntity : TableEntity
    {
        public string Pat { get; set; }

        public bool SubscribedToEvents { get; set; }
    }
}
