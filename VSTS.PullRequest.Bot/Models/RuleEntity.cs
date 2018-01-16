using Microsoft.WindowsAzure.Storage.Table;

namespace VSTS.PullRequest.Bot.Models
{
    public enum RuleType
    {
        WorkItemTaskUpdate
    }

    public class RuleEntity : TableEntity
    {
        public RuleType RuleType { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
