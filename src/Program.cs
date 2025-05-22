using cli.slndoc.Commands;
using cli.slndoc.Models.Settings;
using cli.slndoc.Services;
using cli.slndoc.Services.IO;
using cli.slndoc.Services.Mappings;
using cli.slndoc.Services.Roslyn;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

try
{
    await Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(AppContext.BaseDirectory);
        })
        .ConfigureServices((ctx, services) =>
        {
            services.Configure<ServicesDependenciesSettings>(ctx.Configuration.GetSection(nameof(ServicesDependenciesSettings)));
            services.AddSingleton<IRoslynExtractionService, RoslynExtractionService>();
            services.AddSingleton<IIOService, IOService>();
            services.AddSingleton<ISlnExtractionService, SlnExtractionService>();
            services.AddSingleton<IMappingService, MappingService>();
            services.AddSingleton<SettingsService>();
        })
        .RunCommandLineApplicationAsync<MainCommand>(args);
}
catch (Exception mainExc)
{
    Console.WriteLine(mainExc.Message);
}