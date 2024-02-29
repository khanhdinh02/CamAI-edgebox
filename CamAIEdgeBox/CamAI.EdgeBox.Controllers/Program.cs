using CamAI.EdgeBox.Controllers;
using CamAI.EdgeBox.Controllers.BackgroundServices;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services;
using CamAI.EdgeBox.Services.AI;
using CamAI.EdgeBox.Services.Streaming;
using FFMpegCore;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog(
    (context, logConfig) =>
        logConfig.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext()
);

builder.Services.AddDbContext<CamAiEdgeBoxContext>();
builder.Services.AddScoped<UnitOfWork>();
builder
    .Services.AddScoped<CameraService>()
    .AddScoped<BrandService>()
    .AddScoped<AIService>()
    .AddScoped<ShopService>()
    .AddScoped<EdgeBoxService>()
    .AddScoped<EmployeeService>()
    .AddScoped<GlobalDataSync>();

builder.Services.AddMemoryCache();
builder.Services.AddHostedService<CpuTrackingService>();
builder.Services.Configure<AiConfiguration>(
    builder.Configuration.GetSection(AiConfiguration.Section)
);

var streamConfigurationSection = builder.Configuration.GetSection(StreamingConfiguration.Section);
builder.Services.Configure<StreamingConfiguration>(streamConfigurationSection);

var streamConf = streamConfigurationSection.Get<StreamingConfiguration>()!;
GlobalFFOptions.Configure(x =>
{
    x.BinaryFolder = streamConf.FFMpegPath;
});
StreamingEncoderProcessManager.Option.TimerInterval = streamConf.Interval;

builder.Services.AddCors(
    opts =>
        opts.AddPolicy(
            name: "AllowAll",
            policy =>
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("auto")
        )
);

#pragma warning disable ASP0000
var provider = builder.Services.BuildServiceProvider();
#pragma warning restore ASP0000
using (var scope = provider.CreateScope())
{
    while (true)
    {
        try
        {
            var globalDataSync = scope.ServiceProvider.GetRequiredService<GlobalDataSync>();
            globalDataSync.SyncData();
            break;
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            await scope
                .ServiceProvider.GetRequiredService<CamAiEdgeBoxContext>()
                .Database.MigrateAsync();
            scope.ServiceProvider.GetRequiredService<GlobalDataSync>().SyncData();
        }
    }
}

// TODO [Duy]: how to run masstransit configuration after sync data
builder.ConfigureMassTransit();

builder.Services.Configure<RouteOptions>(opts =>
{
    opts.LowercaseUrls = true;
    opts.LowercaseQueryStrings = true;
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var globalDataSync = scope.ServiceProvider.GetRequiredService<GlobalDataSync>();
    globalDataSync.SyncData();
    // var aiService = scope.ServiceProvider.GetRequiredService<AIService>();
    // aiService.RunAI();
}

app.MapGet("/", () => Results.Ok("Hello word"));

app.Run();

// TODO: Get all camera and run AI
