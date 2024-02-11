using System.IO;
using System.Threading.Tasks;
using GithubHookTransformer.Models;
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
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("Github Webhook received a request.");
    
        // Read the request body
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        
        var githubPayload = JsonConvert.DeserializeObject<GithubWebHookPayload>(requestBody);
        
        // Access parameters from the JSON payload
        string repoName = githubPayload.repository.full_name;
        var type = githubPayload.hook.type;
        var hookId = githubPayload.hook.id;
        var hookName = githubPayload.hook.name;
        var sender = githubPayload.hook.name;
        var pullRequestUrl = githubPayload.repository.pulls_url;
        var createAt = githubPayload.repository.created_at;
        
        // Construct your response message
        var message = $"=> Repository {repoName} received a {type} event from {sender} at {createAt} - Hook ID: {hookId} - Hook Name: {hookName} - Pull Request URL: {pullRequestUrl}";
        log.LogInformation(message);
        log.LogInformation(requestBody);
        return new OkObjectResult(message);
    }
}