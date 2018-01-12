namespace VSTS.PullRequest.ReminderBot
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public enum SubscriptionStatus
    {
        DisabledBySystem,
        DisabledByUser,
        Enabled,
        OnProbation
    }

    public class CreateSubscription
    {
        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("resourceVersion")]
        public string ResourceVersion { get; set; }

        [JsonProperty("consumerId")]
        public string ConsumerId { get; set; }

        [JsonProperty("consumerActionId")]
        public string ConsumerActionId { get; set; }

        [JsonProperty("publisherInputs")]
        public IDictionary<string, string> PublisherInputs { get; set; }

        [JsonProperty("consumerInputs")]
        public ConsumerInputs ConsumerInputs { get; set; }

        [JsonProperty("scope")]
        public int Scope { get; set; }
    }

    public class ConsumerInputs
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class CreateSubscriptionQuery
    {
        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        [JsonProperty("publisherInputFilters")]
        public List<InputFilter> PublisherInputFilters { get; set; }

        [JsonProperty("consumerInputFilters")]
        public List<InputFilter> ConsumerInputFilters { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("consumerId")]
        public string ConsumerId { get; set; }

        [JsonProperty("consumerActionId")]
        public string ConsumerActionId { get; set; }

        public CreateSubscriptionQuery(CreateSubscription subscription)
        {
            EventType = subscription.EventType;
            ConsumerId = subscription.ConsumerId;
            ConsumerActionId = subscription.ConsumerActionId;
            PublisherId = subscription.PublisherId;
            PublisherInputFilters = subscription.PublisherInputs
                .Select(pi => new InputFilter
                {
                    Conditions = new List<InputFilterCondition>
                    {
                        new InputFilterCondition
                        {
                            InputId = pi.Key,
                            InputValue = pi.Value,
                            Operator = 0
                        }
                    }
                })
                .ToList();
            ConsumerInputFilters = new List<InputFilter>
            {
                new InputFilter
                {
                    Conditions = new List<InputFilterCondition>
                    {
                        new InputFilterCondition
                        {
                            InputId = "url",
                            InputValue = subscription.ConsumerInputs.Url,
                            Operator = 0
                        }
                    }
                }
            };
        }
    }

    public class InputFilter
    {
        [JsonProperty("conditions")]
        public List<InputFilterCondition> Conditions { get; set; }
    }

    public class InputFilterCondition
    {
        [JsonProperty("inputId")]
        public string InputId { get; set; }

        [JsonProperty("inputValue")]
        public string InputValue { get; set; }

        [JsonProperty("operator")]
        public int Operator { get; set; }
    }
}
