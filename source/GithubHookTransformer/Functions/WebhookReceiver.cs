using System;
using System.IO;
using System.Threading.Tasks;
using GithubHookTransformer.Models;
using GithubHookTransformer.Models.GithubPayloads;
using GithubHookTransformer.Models.Transforms;
using GithubHookTransformer.Services.HttpCallerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GithubHookTransformer.Functions;

public class WebhookReceiver(IHttpCallerService httpCallerService)
{
    [FunctionName("WebhookReceiver")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequest req, ILogger log)
    {
        var eventTypeString = req.Headers[Consts.Github_Event_Header_Name];
        log.LogInformation("Github Webhook received a request with event type: {eventType}", eventTypeString);

        var parseResult = Enum.TryParse(typeof(GithubEventTypes), eventTypeString, true, out var eventType);
        if (!parseResult)
        {
            log.LogError($"Undefined Github event type: {eventTypeString}");
            return new OkResult();
        }

        var payloadString = await new StreamReader(req.Body).ReadToEndAsync();

        switch (eventType)
        {
            case GithubEventTypes.pull_Request:
                return await ProcessPullRequestEvent(payloadString, log);
            default:
                return new OkObjectResult("Unhandled event type.");
        }
    }

    private async Task<IActionResult> ProcessPullRequestEvent(string payloadString, ILogger log)
    {
        var pullRequestPayload = JsonConvert.DeserializeObject<GithubPullRequestPayload>(payloadString);
        if (pullRequestPayload.action != "opened")
        {
            log.LogInformation("Pull request action is not 'opened', ignoring.");
            return new OkObjectResult("Pull request action is not 'opened', ignoring.");
        }

        var notificationPayload = new MicrosoftTeamsPayload().GeneratePayload(
            pullRequestPayload.pull_request.title,
            pullRequestPayload.repository.name,
            pullRequestPayload.sender.login,
            pullRequestPayload.sender.avatar_url,
            pullRequestPayload.pull_request.created_at.ToShortDateString(),
            pullRequestPayload.pull_request.body?.ToString(),
            pullRequestPayload.pull_request.html_url);

        var webhookUrl = GetProperWebhookUrl(pullRequestPayload.repository.name);
        await httpCallerService.PostPayloadAsync(webhookUrl, notificationPayload);

        return new OkObjectResult(pullRequestPayload);
    }

    private string GetProperWebhookUrl(string repoName)
    {
        var repoNameLower = repoName.ToLower();
        var repoNameEnvValue = Environment.GetEnvironmentVariable(repoNameLower);
        return string.IsNullOrEmpty(repoNameEnvValue)
            ? Environment.GetEnvironmentVariable("DefaultWebhookUrl")
            : repoNameEnvValue;
    }
}