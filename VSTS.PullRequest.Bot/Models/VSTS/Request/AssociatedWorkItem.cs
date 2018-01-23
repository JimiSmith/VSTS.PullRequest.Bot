using Newtonsoft.Json;
using System.Collections.Generic;

namespace VSTS.PullRequest.Bot.Models.VSTS.Request
{
    public class AssociatedWorkItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("fields")]
        public Dictionary<string, string> Fields { get; set; }
    }
}
