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

        var notification = new MicrosoftTeamsPayload
        {
            Title = pullRequestPayload.pull_request.title,
            RepoName = pullRequestPayload.repository.name,
            Author = pullRequestPayload.sender.login,
            Date = pullRequestPayload.pull_request.created_at.ToShortDateString(),
            Description = pullRequestPayload.pull_request.body.ToString(),
            GithubUri = pullRequestPayload.pull_request.html_url,
        };

        string payload = notification.GeneratePayload();
    
        await httpCallerService.PostPayloadAsync("https://wiggertroamler.webhook.office.com/webhookb2/d36dca86-925f-4161-b997-add42c05dd69@26723dba-9aaf-4167-bb0b-b37edc22e442/IncomingWebhook/be72571b0b04484d83ae455ec96b08be/a213c4ac-7ba7-47f4-9b9b-f1cfeec7d357", payload);
        
        
        return new OkObjectResult(pullRequestPayload);
    }
}