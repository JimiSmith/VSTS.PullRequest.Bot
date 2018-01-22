using Microsoft.WindowsAzure.Storage.Table;

namespace VSTS.PullRequest.Bot.Models
{
    public class RuleEntity : TableEntity
    {
        public string RuleJson { get; set; }
    }
}
