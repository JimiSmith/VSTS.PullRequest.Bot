namespace VSTS.PullRequest.Bot.Models.VSTS.Request
{
    using Newtonsoft.Json;

    public enum PullRequestState
    {
        NotSet,
        Pending,
        Succeeded,
        Failed,
        Error
    }

    public class PullRequestStatusUpdate
    {
        [JsonProperty("state")]
        public PullRequestState State { get; set; }

        [JsonProperty("iterationId")]
        public long IterationId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("context")]
        public Context Context { get; set; }

        [JsonProperty("targetUrl")]
        public string TargetUrl { get; set; }
    }

    public class Context
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }
    }
}
