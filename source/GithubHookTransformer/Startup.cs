using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GithubHookTransformer;
using GithubHookTransformer.Services.HttpCallerService;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace GithubHookTransformer;

public class Startup : FunctionsStartup
{
    private IConfiguration Config { get; set; }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
#if DEBUG
        var assemblyPath = Assembly.GetExecutingAssembly().Location;
        var localConfigPath = Path.Combine(Path.GetDirectoryName(assemblyPath) ?? string.Empty, "local.settings.json");

        Config = builder.ConfigurationBuilder
            .AddJsonFile(localConfigPath, false, true)
            .AddEnvironmentVariables()
            .Build();
#else
        Config = builder.ConfigurationBuilder
            .AddEnvironmentVariables()
            .Build();
#endif
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        Config = builder.GetContext().Configuration;

        builder.Services.AddSingleton(Config);
        builder.Services.AddHttpClient();
        builder.Services.AddTransient<IHttpCallerService, HttpCallerService>();
    }
}