using System.Net;
using System.Text.Json.Serialization;
using CamAI.EdgeBox.Consumers;
using CamAI.EdgeBox.Controllers;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Middlewares;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services;
using CamAI.EdgeBox.Services.AI;
using CamAI.EdgeBox.Services.Streaming;
using CamAI.EdgeBox.Services.Utils;
using FFMpegCore;
using MassTransit;
using Serilog;

async Task InitData(WebApplicationBuilder builder1)
{
    // init edge box
    var edgeBoxId = builder1.Configuration.GetRequiredSection("EdgeBoxId").Get<Guid>();
    GlobalData.EdgeBox ??= new DbEdgeBox { Id = edgeBoxId };

    // init services
    Console.WriteLine("Configuring bus for global data");
    builder1.Services.AddScoped<UpdateDataConsumer>();
#pragma warning disable ASP0000
    var provider = builder1.Services.BuildServiceProvider();
#pragma warning restore ASP0000
    using var scope = provider.CreateScope();
    var consumer = scope.ServiceProvider.GetRequiredService<UpdateDataConsumer>();

    var busControl = builder1.CreateSyncBusControl(consumer);
    await busControl.StartAsync();
    Console.WriteLine("Bus for global data started");

    var localIpAddress = NetworkUtil.GetLocalIpAddress();
    Console.WriteLine("Ip address sent to server {0}", localIpAddress);
    // publish health message
    await busControl.Publish(
        new HealthCheckResponseMessage
        {
            EdgeBoxId = GlobalData.EdgeBox.Id,
            Status = EdgeBoxInstallStatus.Working,
            IpAddress = localIpAddress
        }
    );

    // sync data from server
    if (GlobalData.Shop == null || GlobalData.Brand == null)
        await FetchServerData(busControl);

    await busControl.StopAsync();
}

// check network connection
if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
    throw new Exception("No internet connection. Please connect to internet before running");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(opts =>
    opts.AddPolicy(
        "AllowAll",
        cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("Auto")
    )
);

builder
    .Services.AddControllers()
    .AddJsonOptions(config =>
    {
        config.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
GlobalDataHelper.GetData();
await InitData(builder);

builder.ConfigureMassTransit();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseSession();
app.Use(
    (context, next) =>
    {
        if (context.Request.Path.Value?.ToLower().Contains("login") == true)
        {
            next();
            return Task.CompletedTask;
        }

        if (!context.Session.TryGetValue("ID", out _))
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
    foreach (var camera in cameraService.GetCamera())
        cameraService.CheckCameraConnection(camera, runAi: false);

    var aiService = scope.ServiceProvider.GetRequiredService<AiService>();
    aiService.RunAi();
}

app.MapGet("/", () => Results.Ok("Hello word"));

app.Run();
return;

async Task FetchServerData(IBusControl busControl)
{
    var syncRequest = new SyncDataRequest { EdgeBoxId = GlobalData.EdgeBox!.Id };
    await busControl.Publish(syncRequest);

    while (GlobalData.Shop == null || GlobalData.Brand == null)
        Thread.Sleep(1000);
    Console.WriteLine("Bus for global data stopped");
}
