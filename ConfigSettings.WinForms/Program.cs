using ConfigSettings.Shared;
using ConfigSettings.Shared.Crypto;
using ConfigSettings.Shared.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureServices((ctx, services) =>
            {
                var config = ctx.Configuration;
                bool useLocalDb = config.GetValue<bool>("ClientSettings:UseLocalDatabase");
                var conn = config.GetConnectionString("Postgres") ?? "Host=localhost;Database=server_config;Username=ldx_user_sml;Password=F@uc1.C@rt3r.G@73$!!";
                var apiBase = config["ApiSettings:BaseUrl"] ?? "http://localhost:5000/";
                var masterKey = config["Security:MasterKey"] ?? "DevMasterKeyForDemoOnly123!";

                services.AddSingleton(new CryptoHelper(masterKey));

                if (useLocalDb)
                {
                    services.AddSingleton<ConfigSettings.Shared.Interfaces.IConfigSettingsStrategy>(sp =>
                        new PostgresConfigSettingsStrategy(conn, sp.GetRequiredService<CryptoHelper>(), sp.GetService<Microsoft.Extensions.Logging.ILogger<PostgresConfigSettingsStrategy>>())
                    );
                }
                else
                {
                    services.AddHttpClient<ConfigSettings.Shared.Interfaces.IConfigSettingsStrategy, ApiConfigSettingsStrategy>(client =>
                    {
                        client.BaseAddress = new Uri(apiBase);
                        client.Timeout = TimeSpan.FromSeconds(10);
                    })
                    .AddStandardResilienceHandler();
                }

                services.AddSingleton<ConfigSettingsContext>();
                services.AddTransient<MainForm>();
            })
            .Build();

        ApplicationConfiguration.Initialize();

        using var scope = host.Services.CreateScope();
        var mainForm = scope.ServiceProvider.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }
}
