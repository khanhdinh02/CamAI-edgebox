using CamAI.EdgeBox.Controllers;
using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services;
using CamAI.EdgeBox.Services.AI;
using CamAI.EdgeBox.Services.Streaming;
using FFMpegCore;
using SQLitePCL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.Configure<AiConfiguration>(
    builder.Configuration.GetSection(AiConfiguration.Section)
);

var streamConf = builder.Configuration.GetSection(StreamingConfiguration.Section);
builder.Services.Configure<StreamingConfiguration>(streamConf);

var ffmpegPath = streamConf.GetRequiredSection("FFMpegPath").Get<string>();
GlobalFFOptions.Configure(x =>
{
    x.BinaryFolder = ffmpegPath!;
});

builder.Services.AddCors(
    opts =>
        opts.AddPolicy(
            name: "AllowAll",
            policy =>
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("auto")
        )
);

builder.ConfigureMassTransit();

builder.Services.Configure<RouteOptions>(opts =>
{
    opts.LowercaseUrls = true;
    opts.LowercaseQueryStrings = true;
});

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();

// }

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var globalDataSync = scope.ServiceProvider.GetRequiredService<GlobalDataSync>();
    globalDataSync.SyncData();
}

app.Run();

// TODO: Get all camera and run AI
