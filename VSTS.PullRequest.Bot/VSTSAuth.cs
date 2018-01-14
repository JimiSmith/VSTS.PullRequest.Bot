using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VSTS.PullRequest.ReminderBot;

namespace VSTS.PullRequest.Bot
{
    public static class Auth
    {
        [FunctionName(nameof(Start))]
        public static HttpResponseMessage Start(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vsts/auth")]HttpRequestMessage req,
            [Table("Projects")] CloudTable projects)
        {
            var instance = req
                .GetQueryNameValuePairs()
                .FirstOrDefault(qp => string.Equals(qp.Key, "instance", StringComparison.InvariantCultureIgnoreCase))
                .Value;
            var projectId = req
                .GetQueryNameValuePairs()
                .FirstOrDefault(qp => string.Equals(qp.Key, "projectId", StringComparison.InvariantCultureIgnoreCase))
                .Value;
            var callbackUrl = $"{Helpers.GetEnvironmentVariable("Host")}/api/vsts/callback";
            var project = projects.CreateQuery<ProjectEntity>()
                .Where(p => p.PartitionKey == instance && p.RowKey == projectId)
                .ToList()
                .FirstOrDefault();
            if (project == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            var redirectUrl = $"https://app.vssps.visualstudio.com/oauth2/authorize" +
                $"?client_id={Uri.EscapeDataString(Helpers.GetEnvironmentVariable("VSTS.AppId"))}" +
                $"&response_type=Assertion" +
                $"&state={project.AuthState}" +
                $"&scope={Uri.EscapeDataString("vso.code vso.code_status vso.notification_manage vso.work_full vso.workitemsearch")}" +
                $"&redirect_uri={Uri.EscapeDataString(callbackUrl)}";
            var resp = req.CreateResponse(HttpStatusCode.Redirect);
            resp.Headers.Location = new Uri(redirectUrl);
            return resp;
        }

        [FunctionName(nameof(Callback))]
        public static async Task<HttpResponseMessage> Callback(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vsts/callback")]HttpRequestMessage req,
            [Table("Projects")] CloudTable projects,
            [Queue("projects-events-check")] IAsyncCollector<ProjectEntity> projectCollector)
        {
            var code = req
                .GetQueryNameValuePairs()
                .FirstOrDefault(qp => string.Equals(qp.Key, "code", StringComparison.InvariantCultureIgnoreCase))
                .Value;
            var state = req
                .GetQueryNameValuePairs()
                .FirstOrDefault(qp => string.Equals(qp.Key, "state", StringComparison.InvariantCultureIgnoreCase))
                .Value;
            var project = projects.CreateQuery<ProjectEntity>()
                .Where(p => p.AuthState == state)
                .ToList()
                .FirstOrDefault();
            if (project == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                new KeyValuePair<string, string>("client_assertion", Helpers.GetEnvironmentVariable("VSTS.ClientSecret")),
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new KeyValuePair<string, string>("assertion", code),
                new KeyValuePair<string, string>("redirect_uri", $"{Helpers.GetEnvironmentVariable("Host")}/api/vsts/callback")
            });
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync("https://app.vssps.visualstudio.com/oauth2/token", content);
                var tokenJson = await response.Content.ReadAsStringAsync();
                var tokenData = JsonConvert.DeserializeObject<dynamic>(tokenJson);

                project.AccessToken = (string)tokenData.access_token;
                project.RefreshToken = (string)tokenData.refresh_token;
                project.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds((long)tokenData.expires_in);
            }
            await projectCollector.AddAsync(project);
            await projects.ExecuteAsync(TableOperation.InsertOrMerge(project));

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
