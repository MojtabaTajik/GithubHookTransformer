using Newtonsoft.Json;

namespace GithubHookTransformer.Models.Transforms;

public class MicrosoftTeamsPayload
{
    public string Title { get; set; }
    public string RepoName { get; set; }
    public string Author { get; set; }
    public string Date { get; set; }
    public string Description { get; set; }
    public string GithubUri { get; set; }

    public string GeneratePayload()
    {
        var notification = new
        {
            @type = "MessageCard",
            @context = "http://schema.org/extensions",
            themeColor = "0076D7",
            summary = "New Pull Request Notification",
            sections = new[]
            {
                new
                {
                    activityTitle = $"New Pull Request: {Title}",
                    activitySubtitle = $"On {RepoName}, by {Author} on {Date}",
                    activityImage = "https://adaptivecards.io/content/cats/2.png",
                    facts = new[]
                    {
                        new { name = "Description", value = Description }
                    },
                    markdown = true
                }
            },
            potentialAction = new[]
            {
                new
                {
                    @type = "OpenUri",
                    name = "View on GitHub",
                    targets = new[]
                    {
                        new { os = "default", uri = GithubUri }
                    }
                }
            }
        };

        return JsonConvert.SerializeObject(notification, Formatting.Indented);
    }
}

