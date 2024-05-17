using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using CamAI.EdgeBox.Consumers;
using CamAI.EdgeBox.Consumers.Messages;
using CamAI.EdgeBox.Controllers;
using CamAI.EdgeBox.Controllers.BackgroundServices;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Middlewares;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using CamAI.EdgeBox.Services.AI;
using CamAI.EdgeBox.Services.Streaming;
using CamAI.EdgeBox.Services.Utils;
using FFMpegCore;
using MassTransit;
using Microsoft.Extensions.Primitives;
using Serilog;

async Task InitData(WebApplicationBuilder builder1)
{
    // init services
    Console.WriteLine("Configuring bus for global data");
    builder1.Services.AddScoped<UpdateDataConsumer>();
#pragma warning disable ASP0000
    var provider = builder1.Services.BuildServiceProvider();
#pragma warning restore ASP0000
    using var scope = provider.CreateScope();
    var consumer = scope.ServiceProvider.GetRequiredService<UpdateDataConsumer>();

    var edgeBoxId = builder1.Configuration.GetRequiredSection("EdgeBoxId").Get<Guid>();
    GlobalData.EdgeBoxId = edgeBoxId;
    var busControl = builder1.CreateSyncBusControl(consumer, edgeBoxId);
    await busControl.StartAsync();
    Console.WriteLine("Bus for global data started");

    // sync data from server
    await InitializeEdgeBoxWithServer(busControl, builder1.Configuration);
    await busControl.StopAsync();
}

// check network connection
if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
    throw new Exception("No internet connection. Please connect to internet before running");

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddControllers()
    .AddJsonOptions(config =>
    {
        config.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<HealthCheckBackgroundService>();

builder.Host.UseSerilog(
    (context, logConfig) =>
        logConfig.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext()
);

builder.Services.AddScoped<CameraService>().AddScoped<AiService>().AddScoped<EdgeBoxService>();

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

// get local data
await InitData(builder);

builder.ConfigureMassTransit();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseFileServer();
app.MapControllers();

app.Use(
    (context, next) =>
    {
        if (context.Request.Path.Value?.ToLower().Contains("login") == true)
        {
            next();
            return Task.CompletedTask;
        }

        if (
            StringValues.IsNullOrEmpty(context.Request.Headers.Authorization)
            || !Hasher.Verify("yes", context.Request.Headers.Authorization!)
        )
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        next();
        return Task.CompletedTask;
    }
);

using (var scope = app.Services.CreateScope())
{
    var cameraService = scope.ServiceProvider.GetRequiredService<CameraService>();
    foreach (var camera in StaticCameraService.GetCamera())
    {
        Console.WriteLine("Check camera connection");
        cameraService.CheckCameraConnection(camera, runAi: false);
        cameraService.PublishCameraChangeMessage(camera);
    }

    var aiService = scope.ServiceProvider.GetRequiredService<AiService>();
    Console.WriteLine("Running Ai Service");
    aiService.RunAi();
}

app.MapGet("/", () => Results.Ok("Hello word"));

app.Run();
return;

async Task InitializeEdgeBoxWithServer(IBusControl busControl, IConfiguration configuration)
{
    var localIpAddress = NetworkUtil.GetLocalIpAddress();
    var macAddr = NetworkUtil.GetMacAddress();
    var osName = NetworkUtil.GetOsName();
    var edgeBoxId = configuration.GetRequiredSection("EdgeBoxId").Get<Guid>();
    var version = configuration.GetRequiredSection("Version").Get<string>()!;
    Console.WriteLine(
        "Current IP address {0}, MAC Address {1}, OS name {2}, Edge box id {3}, Version {4}",
        localIpAddress,
        macAddr,
        osName,
        edgeBoxId,
        version
    );

    var initializeRequest = new InitializeRequest
    {
        EdgeBoxId = edgeBoxId,
        IpAddress = localIpAddress,
        Version = version,
        MacAddress = macAddr,
        OperatingSystem = osName,
        SerialNumber = IOUtil.GetSerialNumber()
    };
    await busControl.Publish(initializeRequest);

    GlobalData.Version = version;
    GlobalData.MacAddress = macAddr;
    GlobalData.OsName = osName;
    while (GlobalData.EdgeBox == null || GlobalData.Shop == null || GlobalData.Brand == null)
        Thread.Sleep(1000);

    if (GlobalData.EdgeBox.EdgeBoxStatus == EdgeBoxStatus.Active)
        await busControl.Publish(
            new ConfirmedEdgeBoxActivationMessage
            {
                EdgeBoxId = GlobalData.EdgeBoxId,
                IsActivatedSuccessfully = true
            }
        );
    Console.WriteLine("Bus for global data stopped");
}
