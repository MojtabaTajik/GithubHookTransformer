using System;
using Newtonsoft.Json;

namespace GithubHookTransformer.Models.Transforms;

public class MicrosoftTeamsPayload
{
    [JsonProperty("@type")] public string Type { get; } = "MessageCard";

    [JsonProperty("@context")] public string Context { get; } = "http://schema.org/extensions";

    public string ThemeColor { get; } = "0076D7";
    public string Summary { get; } = "New Pull Request Notification";
    public SectionObject[] Sections { get; set; }
    public PotentialActionObject[] PotentialAction { get; set; }

    public class SectionObject
    {
        public string ActivityTitle { get; set; }
        public string ActivitySubtitle { get; set; }
        public string ActivityImage { get; set; }
        public FactObject[] Facts { get; set; }
        public bool Markdown { get; } = true;
    }

    public class FactObject
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class PotentialActionObject
    {
        [JsonProperty("@type")] public string Type { get; } = "OpenUri";
        public string Name { get; } = "↗️ View on GitHub";
        public TargetObject[] Targets { get; set; }
    }

    public class TargetObject
    {
        public string Os { get; } = "default";
        public string Uri { get; set; }
    }
    
    public string GeneratePayload(string title, string repoName, string author, string avatar, string date, string description, string githubUri)
    {
        var notification = new MicrosoftTeamsPayload
        {
            Sections = new[]
            {
                new SectionObject
                {
                    ActivityTitle = $"🧲 PR [{title}] was created in [{repoName}]",
                    ActivitySubtitle = $"👨‍💻 {author} - 📅 {date}",
                    ActivityImage = avatar,
                    Facts = new[]
                    {
                        new FactObject {Name = $"📝 {Environment.NewLine}", Value = description}
                    }
                }
            },
            PotentialAction = new[]
            {
                new PotentialActionObject
                {
                    Targets = new[]
                    {
                        new TargetObject {Uri = githubUri}
                    }
                }
            }
        };

        return JsonConvert.SerializeObject(notification);
    }
}