namespace VSTS.PullRequest.ReminderBot
{
    using Newtonsoft.Json;

    public class WorkItemUpdate
    {
        [JsonProperty("op")]
        public string Op { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
