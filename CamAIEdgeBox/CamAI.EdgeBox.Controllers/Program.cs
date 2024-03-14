using CamAI.EdgeBox.Consumers;
using CamAI.EdgeBox.Controllers;
using CamAI.EdgeBox.Controllers.BackgroundServices;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Middlewares;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services;
using CamAI.EdgeBox.Services.AI;
using CamAI.EdgeBox.Services.Streaming;
using FFMpegCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;

async Task FetchServerData(WebApplicationBuilder builder1)
{
    var edgeBoxId = builder1.Configuration.GetRequiredSection("EdgeBoxId").Get<Guid>();
    GlobalData.EdgeBox ??= new DbEdgeBox { Id = edgeBoxId };

    builder1.Services.AddScoped<UpdateDataConsumer>();
#pragma warning disable ASP0000
    var provider = builder1.Services.BuildServiceProvider();
#pragma warning restore ASP0000
    using var scope = provider.CreateScope();
    var consumer = scope.ServiceProvider.GetRequiredService<UpdateDataConsumer>();

    // sync data from server
    var busControl = builder1.CreateSyncBusControl(consumer);
    await busControl.StartAsync();

    var syncRequest = new SyncDataRequest { EdgeBoxId = edgeBoxId };
    await busControl.Publish(syncRequest);
    while (GlobalData.Shop == null)
        Thread.Sleep(1000);
    // TODO: wait for response from server
    await busControl.StopAsync();
}

async Task FetchLocalData(WebApplicationBuilder webApplicationBuilder)
{
#pragma warning disable ASP0000
    var provider = webApplicationBuilder.Services.BuildServiceProvider();
#pragma warning restore ASP0000
    using var scope = provider.CreateScope();

    while (true)
    {
        try
        {
            var globalDataHelper = scope.ServiceProvider.GetRequiredService<GlobalDataHelper>();
            globalDataHelper.GetData();
            break;
        }
        catch (SqliteException)
        {
            await scope
                .ServiceProvider.GetRequiredService<CamAiEdgeBoxContext>()
                .Database.MigrateAsync();
            scope.ServiceProvider.GetRequiredService<GlobalDataHelper>().GetData();
        }
    }
}

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
    .AddScoped<GlobalDataHelper>();

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

builder.Services.AddCors(opts =>
    opts.AddPolicy(
        name: "AllowAll",
        policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("auto")
    )
);

builder.Services.Configure<RouteOptions>(opts =>
{
    opts.LowercaseUrls = true;
    opts.LowercaseQueryStrings = true;
});

await FetchLocalData(builder);
if (GlobalData.EdgeBox == null)
    await FetchServerData(builder);

builder.ConfigureMassTransit();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/", () => Results.Ok("Hello word"));

app.Run();

// TODO: Get all camera and run AI
