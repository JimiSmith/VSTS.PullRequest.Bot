using System;
using System.Runtime.Serialization;

namespace VSTS.PullRequest.ReminderBot
{
    [Serializable]
    internal class TokenUpdateException : ApplicationException
    {
        public TokenUpdateException()
        {
        }

        public TokenUpdateException(string message) : base(message)
        {
        }

        public TokenUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TokenUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}