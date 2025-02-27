using Azure.CosmosDb.Configurations;
using Azure.CosmosDb.Services.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Azure.CosmosDb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                // This sets up Azure Functions Worker defaults (triggers, logging, etc.)
                .ConfigureFunctionsWebApplication()
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                {
                    // If you're running locally in "Development" environment,
                    // load local.settings.json (note the correct file name).
                    if (hostBuilderContext.HostingEnvironment.IsDevelopment())
                    {
                        configurationBuilder.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
                    }

                    // Optionally, also load environment variables if needed
                    configurationBuilder.AddEnvironmentVariables();
                })
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    var configuration = hostBuilderContext.Configuration;

                    // Bind your AppSettings
                    var appSetting = new AppSettings();
                    configuration.Bind(appSetting);

                    // Also bind the "TS" section (if that's a separate nested config section)
                    configuration.GetSection("TS").Bind(appSetting);

                    // Register the AppSettings object as a singleton
                    services.AddSingleton(appSetting);

                    // Register strongly-typed CosmosDbSettings from the TS:CosmosDbSettings section
                    services.Configure<CosmosDbSettings>(configuration.GetSection("TS:CosmosDbSettings"));

                    // Register your custom ICosmosService
                    services.AddSingleton<ICosmosService, CosmosServices>();

                    // Build and register the CosmosClient
                    services.AddSingleton(_ =>
                    {
                        var cosmosClientBuilder = new CosmosClientBuilder(
                            appSetting?.CosmosDbSettings?.MobileEldConnectionString
                        );
                        return cosmosClientBuilder.Build();
                    });
                })
                .Build();

            // Run your Azure Functions host
            await host.RunAsync();
        }
    }
}
