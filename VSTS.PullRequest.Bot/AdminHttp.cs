using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VSTS.PullRequest.ReminderBot
{
    public static class ProjectEntityExtensions
    {
        public static Uri GetCreateSubscriptionUri(this ProjectEntity project)
        {
            return new Uri($"https://{project.PartitionKey}" +
                "/DefaultCollection/_apis/hooks/subscriptions?api-version=1.0");
        }

        public static CreateSubscription GetCreatePullRequestCreatedSubscriptionBody(this ProjectEntity project)
        {
            return new CreateSubscription
            {
                PublisherId = "tfs",
                EventType = "git.pullrequest.created",
                ConsumerActionId = "httpRequest",
                ConsumerId = "webHooks",
                ResourceVersion = "latest",
                ConsumerInputs = new ConsumerInputs
                {
                    Url = $"{Helpers.GetEnvironmentVariable("Host")}/api/pullRequestCreated"
                },
                // all the blank ones are needed otherwise the webhook call doesn't have all the info
                // yep. for real
                PublisherInputs = new Dictionary<string, string>
                {
                    ["projectId"] = project.RowKey,
                    ["repository"] = "",
                    ["branch"] = "",
                    ["pullrequestCreatedBy"] = "",
                    ["pullrequestCreatedBy"] = ""
                },
                Scope = 1
            };
        }

        public static CreateSubscription GetCreatePullRequestUpdatedSubscriptionBody(this ProjectEntity project)
        {
            return new CreateSubscription
            {
                PublisherId = "tfs",
                EventType = "git.pullrequest.updated",
                ConsumerActionId = "httpRequest",
                ConsumerId = "webHooks",
                ResourceVersion = "latest",
                ConsumerInputs = new ConsumerInputs
                {
                    Url = $"{Helpers.GetEnvironmentVariable("Host")}/api/pullRequestUpdated"
                },
                // all the blank ones are needed otherwise the webhook call doesn't have all the info
                // yep. for real
                PublisherInputs = new Dictionary<string, string>
                {
                    ["projectId"] = project.RowKey,
                    ["repository"] = "",
                    ["branch"] = "",
                    ["notificationType"] = "",
                    ["pullrequestCreatedBy"] = "",
                    ["pullrequestReviewersContains"] = ""
                },
                Scope = 1
            };
        }
    }

    public static class AdminHttp
    {
        [FunctionName(nameof(AddProject))]
        public static async Task<HttpResponseMessage> AddProject([HttpTrigger(AuthorizationLevel.Function, "post", Route = "addProject")]HttpRequestMessage req,
            [Table("Projects")] CloudTable projects)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(await req.Content.ReadAsStringAsync());
            if (!Guid.TryParse((string)data.projectId, out Guid projectId))
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid project id");
            }
            var projectIdString = projectId.ToString();
            var instance = (string)data.instance;
            var project = projects.CreateQuery<ProjectEntity>().Where(p => p.PartitionKey == instance && p.RowKey == projectIdString)
                .ToList()
                .FirstOrDefault() ?? new ProjectEntity
                {
                    PartitionKey = instance,
                    RowKey = projectId.ToString(),
                    SubscribedToEvents = false,
                    Pat = (string)data.pat
                };
            if (!project.SubscribedToEvents)
            {
                project.SubscribedToEvents = await SubscribeToProjectEventsAsync(project);
            }
            await projects.ExecuteAsync(TableOperation.InsertOrMerge(project));
            return req.CreateResponse(HttpStatusCode.OK);
        }

        private static async Task<bool> SubscribeToProjectEventsAsync(ProjectEntity projectEntity)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", projectEntity.Pat))));
                var url = projectEntity.GetCreateSubscriptionUri();
                var createBody = projectEntity.GetCreatePullRequestCreatedSubscriptionBody();
                var createPRSuccessful = false;
                if (await IsEventSubscribedToProjectAsync(createBody, projectEntity))
                {
                    createPRSuccessful = true;
                }
                else
                {
                    var createEventResponse = await httpClient.PostAsJsonAsync(
                        url.ToString(),
                        createBody);
                    createPRSuccessful = createEventResponse.IsSuccessStatusCode;
                }
                var updateBody = projectEntity.GetCreatePullRequestUpdatedSubscriptionBody();
                var updatePRSuccessful = false;
                if (await IsEventSubscribedToProjectAsync(updateBody, projectEntity))
                {
                    updatePRSuccessful = true;
                }
                else
                {
                    var updateEventResponse = await httpClient.PostAsJsonAsync(
                        url.ToString(),
                        updateBody);
                    updatePRSuccessful = updateEventResponse.IsSuccessStatusCode;
                }
                return createPRSuccessful && updatePRSuccessful;
            }
        }

        private static async Task<bool> IsEventSubscribedToProjectAsync(CreateSubscription subscription, ProjectEntity projectEntity)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", projectEntity.Pat))));
                var url = $"https://{projectEntity.PartitionKey}/_apis/hooks/subscriptionsquery?api-version=4.1-preview";
                var query = new CreateSubscriptionQuery(subscription);
                var resp = await httpClient.PostAsJsonAsync(url, query);
                if (!resp.IsSuccessStatusCode)
                {
                    return false;
                }
                var subscriptionResultJson = await resp.Content.ReadAsStringAsync();
                var subscriptionResult = JsonConvert.DeserializeObject<dynamic>(subscriptionResultJson);
                return (subscriptionResult?.results.Count ?? 0) > 0;
            }
        }
    }
}
