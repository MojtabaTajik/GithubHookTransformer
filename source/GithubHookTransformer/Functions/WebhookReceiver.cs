using System;
using System.IO;
using System.Threading.Tasks;
using GithubHookTransformer.Models;
using GithubHookTransformer.Models.GithubPayloads;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GithubHookTransformer.Functions;

public static class WebhookReceiver
{
    [FunctionName("WebhookReceiver")]
    public static async Task<IActionResult> RunAsync(
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

    private static async Task<IActionResult> ProcessPullRequestEvent(string payloadString, ILogger log)
    {
        // Read the request body

        var githubPayload = JsonConvert.DeserializeObject<GithubWebHookPayload>(payloadString);

        // Access parameters from the JSON payload
        string repoName = githubPayload.repository.full_name;
        var type = githubPayload.hook.type;
        var hookId = githubPayload.hook.id;
        var hookName = githubPayload.hook.name;
        var sender = githubPayload.hook.name;
        var pullRequestUrl = githubPayload.repository.pulls_url;
        var createAt = githubPayload.repository.created_at;

        // Construct your response message
        var message =
            $"=> Repository {repoName} received a {type} event from {sender} at {createAt} - Hook ID: {hookId} - Hook Name: {hookName} - Pull Request URL: {pullRequestUrl}";
        log.LogInformation(message);
        log.LogInformation(payloadString);
        return new OkObjectResult(message);
    }
}