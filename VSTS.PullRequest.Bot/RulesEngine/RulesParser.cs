using System;
using System.Collections.Generic;
using System.Linq;

namespace VSTS.PullRequest.Bot.RulesEngine
{
    public class ParsedRule
    {
        public List<string> AllKeys { get; set; }
        public Func<Dictionary<string, string>, bool> Matches { get; internal set; }
        public Dictionary<string, string> UpdatedFields { get; internal set; }
    }

    public class ParsedRuleGroup
    {
        public RuleType? Type { get; internal set; }
        public List<ParsedRule> Rules { get; internal set; }
    }

    public static class RuleParser
    {
        public static List<ParsedRuleGroup> ParseRules(RulesContainer rulesContainer)
        {
            return rulesContainer
                .Rules
                .GroupBy(r => r.Type)
                .Select(rules => new ParsedRuleGroup
                {
                    Type = rules.Key,
                    Rules = rules.Select(r => new ParsedRule
                    {
                        AllKeys = r.Conditions.Select(c => c.Key)
                            .Union(r.Actions.Select(a => a.Key))
                            .ToList(),
                        Matches = (Dictionary<string, string> fields) => r.Conditions.All(c => Match(c, fields)),
                        UpdatedFields = r.Actions
                            .ToDictionary(a => a.Key, a => a.Value)
                    }).ToList()
                })
                .ToList();
        }

        private static bool Match(Condition c, Dictionary<string, string> fields)
        {
            if (!fields.ContainsKey(c.Key)) return false;
            if (c.Operator == OperatorType.Eq)
            {
                return fields[c.Key].Equals(c.Value, StringComparison.CurrentCultureIgnoreCase);
            }
            else if (c.Operator == OperatorType.Ne)
            {
                return !fields[c.Key].Equals(c.Value, StringComparison.CurrentCultureIgnoreCase);
            }
            return false;
        }
    }
}
