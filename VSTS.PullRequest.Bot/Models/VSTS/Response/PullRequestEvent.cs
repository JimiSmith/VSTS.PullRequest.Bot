namespace VSTS.PullRequest.Bot.Models.VSTS.Response
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class SubscriptionEvent<T>
    {
        [JsonProperty("subscriptionId")]
        public string SubscriptionId { get; set; }

        [JsonProperty("notificationId")]
        public long NotificationId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("detailedMessage")]
        public Message DetailedMessage { get; set; }

        [JsonProperty("resource")]
        public T Resource { get; set; }

        [JsonProperty("resourceVersion")]
        public string ResourceVersion { get; set; }

        [JsonProperty("resourceContainers")]
        public ResourceContainers ResourceContainers { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }

    public class Message
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("markdown")]
        public string Markdown { get; set; }
    }

    public class PullRequestEvent
    {
        [JsonProperty("repository")]
        public PullRequestRepository Repository { get; set; }

        [JsonProperty("pullRequestId")]
        public long PullRequestId { get; set; }

        [JsonProperty("codeReviewId")]
        public long CodeReviewId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdBy")]
        public PullRequestCreatedBy CreatedBy { get; set; }

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
        public ResourceLastMergeSourceCommit LastMergeSourceCommit { get; set; }

        [JsonProperty("lastMergeTargetCommit")]
        public ResourceLastMergeSourceCommit LastMergeTargetCommit { get; set; }

        [JsonProperty("lastMergeCommit")]
        public ResourceLastMergeCommit LastMergeCommit { get; set; }

        [JsonProperty("reviewers")]
        public List<PullRequestReviewer> Reviewers { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }

        [JsonProperty("supportsIterations")]
        public bool SupportsIterations { get; set; }

        [JsonProperty("artifactId")]
        public string ArtifactId { get; set; }
    }

    public class PullRequestCreatedBy
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

    public class ResourceLastMergeCommit
    {
        [JsonProperty("commitId")]
        public string CommitId { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("committer")]
        public Author Committer { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Author
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }

    public class ResourceLastMergeSourceCommit
    {
        [JsonProperty("commitId")]
        public string CommitId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Links
    {
        [JsonProperty("web")]
        public Statuses Web { get; set; }

        [JsonProperty("statuses")]
        public Statuses Statuses { get; set; }
    }

    public class Statuses
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public class PullRequestRepository
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("project")]
        public PullRequestProject Project { get; set; }

        [JsonProperty("remoteUrl")]
        public string RemoteUrl { get; set; }

        [JsonProperty("sshUrl")]
        public string SshUrl { get; set; }
    }

    public class PullRequestProject
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("revision")]
        public long Revision { get; set; }

        [JsonProperty("visibility")]
        public string Visibility { get; set; }
    }

    public class PullRequestReviewer
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
    }

    public class ResourceContainers
    {
        [JsonProperty("collection")]
        public Account Collection { get; set; }

        [JsonProperty("account")]
        public Account Account { get; set; }

        [JsonProperty("project")]
        public Account Project { get; set; }
    }

    public class Account
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("baseUrl")]
        public string BaseUrl { get; set; }
    }
}
