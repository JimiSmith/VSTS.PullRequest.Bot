using System;

namespace VSTS.PullRequest.ReminderBot
{
    public static class Helpers
    {
        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
