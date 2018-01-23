namespace VSTS.PullRequest.Bot.Models.VSTS.Response
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class Response<T>
    {
        [JsonProperty("value")]
        public List<T> Value { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public class PullRequest
    {
        [JsonProperty("repository")]
        public Repository Repository { get; set; }

        [JsonProperty("pullRequestId")]
        public long PullRequestId { get; set; }

        [JsonProperty("codeReviewId")]
        public long CodeReviewId { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("createdBy")]
        public CreatedBy CreatedBy { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("sourceRefName")]
        public string SourceRefName { get; set; }

        [JsonProperty("targetRefName")]
        public string TargetRefName { get; set; }

        [JsonProperty("mergeStatus")]
        public string MergeStatus { get; set; }

        [JsonProperty("mergeId")]
        public string MergeId { get; set; }

        [JsonProperty("lastMergeSourceCommit")]
        public LastMergeCommit LastMergeSourceCommit { get; set; }

        [JsonProperty("lastMergeTargetCommit")]
        public LastMergeCommit LastMergeTargetCommit { get; set; }

        [JsonProperty("lastMergeCommit")]
        public LastMergeCommit LastMergeCommit { get; set; }

        [JsonProperty("reviewers")]
        public List<Reviewer> Reviewers { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("supportsIterations")]
        public bool SupportsIterations { get; set; }
    }

    public class CreatedBy
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

    public class LastMergeCommit
    {
        [JsonProperty("commitId")]
        public string CommitId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Repository
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("project")]
        public Project Project { get; set; }
    }

    public class Project
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public class Reviewer
    {
        [JsonProperty("reviewerUrl")]
        public string ReviewerUrl { get; set; }

        [JsonProperty("vote")]
        public long Vote { get; set; }

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

        [JsonProperty("isContainer")]
        public bool? IsContainer { get; set; }
    }

    public enum Status
    {
        Active,
        Abandoned,
        Completed
    }
}
