using ConfigSettings.Shared;
using ConfigSettings.Shared.Crypto;
using ConfigSettings.Shared.Strategies;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? "Host=postgres;Database=settingsdb;Username=postgres;Password=postgrespwd";
var masterKey = builder.Configuration["Security:MasterKey"] ?? "DevMasterKeyForDemoOnly123!";

builder.Services.AddSingleton(new CryptoHelper(masterKey, "abc"));

builder.Services.AddSingleton<ConfigSettings.Shared.Interfaces.IConfigSettingsStrategy>(sp =>
    new PostgresConfigSettingsStrategy(connectionString, sp.GetRequiredService<CryptoHelper>(), sp.GetService<Microsoft.Extensions.Logging.ILogger<PostgresConfigSettingsStrategy>>())
);

builder.Services.AddSingleton<ConfigSettingsContext>();

var app = builder.Build();

app.MapGet("/api/configsettings", async (ConfigSettingsContext ctx) => Results.Ok(await ctx.GetAllAsync()));
app.MapGet("/api/configsettings/{id:int}", async (ConfigSettingsContext ctx, int id) =>
{
    var item = await ctx.GetByIdAsync(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});
app.MapPost("/api/configsettings", async (ConfigSettingsContext ctx, ConfigSettings.Shared.Models.ConfigSetting cs) =>
{
    var id = await ctx.AddAsync(cs);
    return Results.Created($"/api/configsettings/{id}", id);
});
app.MapPut("/api/configsettings/{id:int}", async (ConfigSettingsContext ctx, int id, ConfigSettings.Shared.Models.ConfigSetting cs) =>
{
    cs.ConfigSettingsId = id;
    var success = await ctx.UpdateAsync(cs);
    return success ? Results.NoContent() : Results.NotFound();
});
app.MapDelete("/api/configsettings/{id:int}", async (ConfigSettingsContext ctx, int id) =>
{
    var success = await ctx.DeleteAsync(id);
    return success ? Results.NoContent() : Results.NotFound();
});

app.Run();
