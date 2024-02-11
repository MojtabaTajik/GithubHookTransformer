using System.Threading.Tasks;

namespace GithubHookTransformer.Services.HttpCallerService;

public interface IHttpCallerService
{
    public Task PostPayloadAsync(string url, string jsonPayload);
}