using CamAI.EdgeBox.MassTransit;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services;
using CamAI.EdgeBox.Services.Streaming;
using FFMpegCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CamAiEdgeBoxContext>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<CameraService>().AddScoped<BrandService>();

builder.Services.Configure<StreamingConfiguration>(
    builder.Configuration.GetSection(StreamingConfiguration.Section)
);

GlobalFFOptions.Configure(x =>
{
    x.BinaryFolder = "C:\\Users\\ngud\\Downloads\\ffmpeg-6.1.1-essentials_build\\bin";
});

builder.Services.AddCors(opts =>
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
