using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace VSTS.PullRequest.ReminderBot
{
    public static class PullRequestStatusWebhook
    {
        [FunctionName(nameof(PullRequestCreated))]
        public static async Task<HttpResponseMessage> PullRequestCreated(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "pullRequestCreated")]HttpRequestMessage req,
            [Table("Projects")] CloudTable projects,
            TraceWriter log)
        {
            var pullRequestInfo = JsonConvert.DeserializeObject<SubscriptionEvent<PullRequestEvent>>(await req.Content.ReadAsStringAsync().ConfigureAwait(false));
            if (pullRequestInfo.Resource == null || pullRequestInfo.Resource.Status != "active")
            {
                return req.CreateResponse(HttpStatusCode.OK);
            }
            var instance = new Uri(pullRequestInfo.ResourceContainers.Account.BaseUrl).Authority;
            var project = projects
                .CreateQuery<ProjectEntity>()
                .Where(p => p.PartitionKey == instance && p.RowKey == pullRequestInfo.ResourceContainers.Project.Id)
                .ToList()
                .FirstOrDefault();
            if (project != null)
            {
                await Helpers.UpdateAccessToken(projects, project.PartitionKey, project.RowKey).ConfigureAwait(false);
                await UpdateStatusAsync(log, pullRequestInfo, project.AccessToken).ConfigureAwait(false);
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }

        [FunctionName(nameof(PullRequestUpdated))]
        public static async Task<HttpResponseMessage> PullRequestUpdated(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "pullRequestUpdated")]HttpRequestMessage req,
            [Table("Projects")] CloudTable projects,
            TraceWriter log)
        {
            var pullRequestInfo = JsonConvert.DeserializeObject<SubscriptionEvent<PullRequestEvent>>(await req.Content.ReadAsStringAsync().ConfigureAwait(false));
            if (pullRequestInfo.Resource == null || pullRequestInfo.Resource.Status != "active")
            {
                return req.CreateResponse(HttpStatusCode.OK);
            }
            var instance = new Uri(pullRequestInfo.ResourceContainers.Account.BaseUrl).Authority;
            var project = projects
                .CreateQuery<ProjectEntity>()
                .Where(p => p.PartitionKey == instance && p.RowKey == pullRequestInfo.ResourceContainers.Project.Id)
                .ToList()
                .FirstOrDefault();
            if (project != null)
            {
                await Helpers.UpdateAccessToken(projects, project.PartitionKey, project.RowKey).ConfigureAwait(false);
                await UpdateStatusAsync(log, pullRequestInfo, project.AccessToken).ConfigureAwait(false);
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }

        private static async Task UpdateStatusAsync(TraceWriter log, SubscriptionEvent<PullRequestEvent> pullRequestInfo, string accessToken)
        {
            var pr = pullRequestInfo.Resource;
            if (pr.Reviewers.Count == 0)
            {
                // don't set a status if there are no reviewers
                return;
            }
            var outstandingReviewers = pr.Reviewers
                .Where(r => r.UniqueName.Contains("@") && r.Vote == 0)
                .Select(r => r.UniqueName);
            var percentReviewed = (pr.Reviewers.Count(r => r.Vote != 0) / (float)pr.Reviewers.Count) * 100.0f;
            var vote = pr.Reviewers.Sum(r => r.Vote);
            var reviewed = percentReviewed >= 100;
            var statusUrl = pullRequestInfo.ResourceContainers.Account.BaseUrl +
                $"{pr.Repository.Project.Id}/_apis/git/repositories/{pr.Repository.Id}" +
                $"/pullRequests/{pr.PullRequestId}/statuses?api-version=4.0-preview";
            var description = $"Waiting for {pr.Reviewers.Count(r => r.Vote == 0)} reviewers";
            var state = PullRequestState.Pending;
            if (reviewed)
            {
                if (vote > 0)
                {
                    state = PullRequestState.Succeeded;
                    description = "All reviews approved";
                }
                else
                {
                    state = PullRequestState.Error;
                    description = "Reviews completed with rejections ";
                }
            }
            var update = new PullRequestStatusUpdate
            {
                State = state,
                Context = new Context
                {
                    Name = "review-check",
                    Genre = "vsts-pullrequest-bot"
                },
                TargetUrl = pr.Links.Web.Href,
                Description = description
            };
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var resp = await httpClient.PostAsJsonAsync(statusUrl, update).ConfigureAwait(false);
                var details = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                resp.EnsureSuccessStatusCode();
            }
            log.Info($"{string.Join("\n", outstandingReviewers)}\n");
        }
    }
}