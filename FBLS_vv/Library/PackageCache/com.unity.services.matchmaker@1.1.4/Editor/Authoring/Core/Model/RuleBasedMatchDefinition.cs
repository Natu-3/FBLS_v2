using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Services.Matchmaker.Authoring.Core.Model
{
    internal class RuleBasedMatchDefinition
    {
        [DataMember(IsRequired = true)] public List<RuleBasedTeamDefinition> teams = new ();
        [DataMember(IsRequired = false)] public List<Rule> matchRules = new ();
    }

    internal class RuleBasedTeamDefinition
    {
        [DataMember(IsRequired = true)] public string name;
        [DataMember(IsRequired = true)] public Range teamCount;
        [DataMember(IsRequired = true)] public Range playerCount;
        [DataMember(IsRequired = false)] public List<Rule> teamRules = new ();
    }

    internal class Range
    {
        [DataMember(IsRequired = true)] public int min;

        [DataMember(IsRequired = true)] public int max;

        [DataMember(IsRequired = false)] public List<RangeRelaxation> relaxations = new ();
    }

    internal class RangeRelaxation
    {
        [DataMember(IsRequired = true)] public RangeRelaxationType type;
        [DataMember(IsRequired = true)] public AgeType ageType;
        [DataMember(IsRequired = true)] public double atSeconds;
        [DataMember(IsRequired = true)] public double value;
    }

    internal enum RangeRelaxationType
    {
        RangeControlReplaceMin
    }

    internal enum AgeType
    {
        Youngest,
        Oldest,
        Average
    }

    internal class Rule
    {
        [DataMember(IsRequired = true)] public string source;
        [DataMember(IsRequired = true)] public string name;
        [DataMember(IsRequired = true)] public RuleType type;
        [DataMember(IsRequired = false)] public JsonObject reference;
        [DataMember(IsRequired = false)] public double overlap;
        [DataMember(IsRequired = false)] public bool enableRule;
        [DataMember(IsRequired = false)] public bool not;
        [DataMember(IsRequired = false)] public List<RuleRelaxation> relaxations = new ();
        [DataMember(IsRequired = false)] public RuleExternalData externalData;
    }

    internal class RuleRelaxation
    {
        [DataMember(IsRequired = true)] public RuleRelaxationType type;
        [DataMember(IsRequired = true)] public AgeType ageType;
        [DataMember(IsRequired = true)] public double atSeconds;
        [DataMember(IsRequired = false)] public JsonObject value;
    }

    internal enum RuleRelaxationType
    {
        RuleControlEnable,
        RuleControlDisable,
        ReferenceControlReplace,
    }

    internal enum RuleType
    {
        LessThan,
        LessThanEqual,
        GreaterThan,
        GreaterThanEqual,
        Difference,
        Equality,
        DoubleDifference,
        InList,
        Intersection
    }

    internal class RuleExternalData
    {
        internal class Leaderboard
        {
            [DataMember(IsRequired = true)] public string id;
        }
        
        internal class CloudSave
        {
            [DataMember(IsRequired = true)] public AccessClass accessClass;
            [DataMember(IsRequired = false, Name = "default")] public JsonObject _default;

            internal enum AccessClass
            {
                Default,
                Public,
                Protected,
                Private
            }
        }

        [DataMember(IsRequired = false)] public Leaderboard leaderboard;
        [DataMember(IsRequired = false)] public CloudSave cloudSave;
    }
}
