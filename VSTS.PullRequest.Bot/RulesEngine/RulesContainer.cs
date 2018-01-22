namespace VSTS.PullRequest.Bot.RulesEngine
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class RulesContainer
    {
        [JsonProperty("rules")]
        public List<Rule> Rules { get; set; }
    }

    public partial class Rule
    {
        [JsonProperty("type")]
        public RuleType? Type { get; set; }

        [JsonProperty("conditions")]
        public List<Condition> Conditions { get; set; }

        [JsonProperty("actions")]
        public List<Action> Actions { get; set; }
    }

    public partial class Action
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class Condition
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("operator")]
        public OperatorType Operator { get; set; }
    }

    public enum OperatorType { Eq, Ne };

    public enum RuleType { WorkItemUpdate };

    public partial class RulesContainer
    {
        public static RulesContainer FromJson(string json) => JsonConvert.DeserializeObject<RulesContainer>(json, Converter.Settings);
    }

    static class OperatorTypeExtensions
    {
        public static OperatorType? ValueForString(string str)
        {
            switch (str)
            {
                case "eq": return OperatorType.Eq;
                case "ne": return OperatorType.Ne;
                default: return null;
            }
        }

        public static OperatorType ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            var maybeValue = ValueForString(str);
            if (maybeValue.HasValue) return maybeValue.Value;
            throw new Exception("Unknown enum case " + str);
        }

        public static void WriteJson(this OperatorType value, JsonWriter writer, JsonSerializer serializer)
        {
            switch (value)
            {
                case OperatorType.Eq: serializer.Serialize(writer, "eq"); break;
                case OperatorType.Ne: serializer.Serialize(writer, "ne"); break;
            }
        }
    }

    static class RuleTypeExtensions
    {
        public static RuleType? ValueForString(string str)
        {
            switch (str)
            {
                case "WorkItemUpdate": return RuleType.WorkItemUpdate;
                default: return null;
            }
        }

        public static RuleType ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            var maybeValue = ValueForString(str);
            if (maybeValue.HasValue) return maybeValue.Value;
            throw new Exception("Unknown enum case " + str);
        }

        public static void WriteJson(this RuleType value, JsonWriter writer, JsonSerializer serializer)
        {
            switch (value)
            {
                case RuleType.WorkItemUpdate: serializer.Serialize(writer, "WorkItemUpdate"); break;
            }
        }
    }

    public static class Serialize
    {
        public static string ToJson(this RulesContainer self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(OperatorType) || objectType == typeof(RuleType) || objectType == typeof(OperatorType?) || objectType == typeof(RuleType?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(OperatorType))
                return OperatorTypeExtensions.ReadJson(reader, serializer);
            if (objectType == typeof(RuleType))
                return RuleTypeExtensions.ReadJson(reader, serializer);
            if (objectType == typeof(OperatorType?))
            {
                if (reader.TokenType == JsonToken.Null) return null;
                return OperatorTypeExtensions.ReadJson(reader, serializer);
            }
            if (objectType == typeof(RuleType?))
            {
                if (reader.TokenType == JsonToken.Null) return null;
                return RuleTypeExtensions.ReadJson(reader, serializer);
            }
            throw new Exception("Unknown type");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = value.GetType();
            if (t == typeof(OperatorType))
            {
                ((OperatorType)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(RuleType))
            {
                ((RuleType)value).WriteJson(writer, serializer);
                return;
            }
            throw new Exception("Unknown type");
        }

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = { new Converter() },
        };
    }
}
