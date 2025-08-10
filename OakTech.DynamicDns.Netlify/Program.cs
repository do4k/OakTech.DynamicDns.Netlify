// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OakTech.DynamicDns.Netlify.Clients;
using OakTech.DynamicDns.Netlify.Options;
using OakTech.DynamicDns.Netlify.Services;
using Refit;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Debug);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<IpifyOptions>(context.Configuration.GetSection("Ipify"));
        services.AddOptions<IpifyOptions>()
            .Bind(context.Configuration.GetSection("Ipify"));
        
        services.Configure<NetlifyOptions>(context.Configuration.GetSection("Netlify"));
        services.AddOptions<NetlifyOptions>()
            .Bind(context.Configuration.GetSection("Netlify"));

        var ipifyOptions = context.Configuration.GetSection("Ipify").Get<IpifyOptions>();
        services.AddRefitClient<IIpifyApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(ipifyOptions?.Endpoint ?? throw new InvalidOperationException("Ipify:Endpoint is not configured")));
        
        var netlifyOptions = context.Configuration.GetSection("Netlify").Get<NetlifyOptions>();
        services.AddRefitClient<INetlifyApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(netlifyOptions.Endpoint))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler())
            .AddTypedClient((client, sp) =>
            {
                var refitSettings = new RefitSettings
                {
                    ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    })
                };
                return RestService.For<INetlifyApi>(client, refitSettings);
            });
        
        services.AddHostedService<DomainManagementService>();
    })
    .Build();
    
await host.RunAsync();