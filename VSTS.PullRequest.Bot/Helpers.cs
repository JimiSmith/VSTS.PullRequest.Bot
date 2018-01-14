using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace VSTS.PullRequest.ReminderBot
{
    public static class Helpers
    {
        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

        public static async Task UpdateAccessToken(CloudTable projects, string partitionKey, string rowKey)
        {
            var project = projects.CreateQuery<ProjectEntity>()
                .Where(p => p.PartitionKey == partitionKey && p.RowKey == rowKey)
                .ToList()
                .FirstOrDefault();
            if (project == null || project.ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(10))
            {
                return;
            }
            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                new KeyValuePair<string, string>("client_assertion", GetEnvironmentVariable("VSTS.ClientSecret")),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("assertion", project.RefreshToken),
                new KeyValuePair<string, string>("redirect_uri", $"{GetEnvironmentVariable("Host")}/api/vsts/callback")
            });
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync("https://app.vssps.visualstudio.com/oauth2/token", content);
                var tokenJson = await response.Content.ReadAsStringAsync();
                var tokenData = JsonConvert.DeserializeObject<dynamic>(tokenJson);

                project.AccessToken = (string)tokenData.access_token;
                project.RefreshToken = (string)tokenData.refresh_token;
                project.ExpiresAt = DateTimeOffset.UtcNow.AddSeconds((long)tokenData.expires_in);

                await projects.ExecuteAsync(TableOperation.InsertOrMerge(project));
            }
        }
    }
}
