using System.Collections.Generic;
using Newtonsoft.Json;

namespace GithubHookTransformer.Models.Transforms;

public class MicrosoftTeams
{
    [JsonProperty("@type")]
    public string type { get; set; }

    [JsonProperty("@context")]
    public string context { get; set; }
    public string themeColor { get; set; }
    public string summary { get; set; }
    public List<Section> sections { get; set; }
    public List<PotentialAction> potentialAction { get; set; }
}

public class Fact
{
    public string name { get; set; }
    public string value { get; set; }
}

public class PotentialAction
{
    [JsonProperty("@type")]
    public string type { get; set; }
    public string name { get; set; }
    public List<Target> targets { get; set; }
}

public class Section
{
    public string activityTitle { get; set; }
    public string activitySubtitle { get; set; }
    public string activityImage { get; set; }
    public List<Fact> facts { get; set; }
    public bool markdown { get; set; }
}

public class Target
{
    public string os { get; set; }
    public string uri { get; set; }
}

