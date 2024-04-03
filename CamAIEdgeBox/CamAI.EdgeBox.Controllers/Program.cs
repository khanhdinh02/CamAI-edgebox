using CamAI.EdgeBox.Consumers;
using CamAI.EdgeBox.Controllers;
using CamAI.EdgeBox.Controllers.BackgroundServices;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Middlewares;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using CamAI.EdgeBox.Services.AI;
using CamAI.EdgeBox.Services.Streaming;
using FFMpegCore;
using Serilog;

async Task FetchServerData(WebApplicationBuilder builder1)
{
    var edgeBoxId = builder1.Configuration.GetRequiredSection("EdgeBoxId").Get<Guid>();
    GlobalData.EdgeBox ??= new DbEdgeBox { Id = edgeBoxId };

    Log.Information("Configuring bus for global data");
    builder1.Services.AddScoped<UpdateDataConsumer>();
#pragma warning disable ASP0000
    var provider = builder1.Services.BuildServiceProvider();
#pragma warning restore ASP0000
    using var scope = provider.CreateScope();
    var consumer = scope.ServiceProvider.GetRequiredService<UpdateDataConsumer>();

    // sync data from server
    var busControl = builder1.CreateSyncBusControl(consumer);
    await busControl.StartAsync();
    Log.Information("Bus for global data started");

    var syncRequest = new SyncDataRequest { EdgeBoxId = edgeBoxId };
    await busControl.Publish(syncRequest);
    while (GlobalData.Shop == null || GlobalData.Brand == null)
        Thread.Sleep(1000);
    await busControl.StopAsync();
    Log.Information("Bus for global data stopped");
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog(
    (context, logConfig) =>
        logConfig.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext()
);

builder.Services.AddScoped<CameraService>().AddScoped<AiService>().AddScoped<EdgeBoxService>();

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

// get data
GlobalDataHelper.GetData();
if (GlobalData.EdgeBox == null || GlobalData.Shop == null || GlobalData.Brand == null)
    await FetchServerData(builder);

builder.ConfigureMassTransit();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var cameraService = scope.ServiceProvider.GetRequiredService<CameraService>();
    foreach (var camera in cameraService.GetCamera())
        cameraService.CheckCameraConnection(camera);

    var aiService = scope.ServiceProvider.GetRequiredService<AiService>();
    aiService.RunAi();
}

app.MapGet("/", () => Results.Ok("Hello word"));

app.Run();
