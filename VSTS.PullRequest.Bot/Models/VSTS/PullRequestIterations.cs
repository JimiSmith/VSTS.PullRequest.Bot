namespace VSTS.PullRequest.ReminderBot
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class PullRequestIterations
    {
        [JsonProperty("value")]
        public List<Value> Value { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public class Value
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("updatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty("sourceRefCommit")]
        public RefCommit SourceRefCommit { get; set; }

        [JsonProperty("targetRefCommit")]
        public RefCommit TargetRefCommit { get; set; }

        [JsonProperty("commonRefCommit")]
        public RefCommit CommonRefCommit { get; set; }

        [JsonProperty("hasMoreCommits")]
        public bool HasMoreCommits { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }

    public class PullRequestIterationAuthor
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }
    }

    public class RefCommit
    {
        [JsonProperty("commitId")]
        public string CommitId { get; set; }
    }
}
