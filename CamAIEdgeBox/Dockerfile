FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app
EXPOSE 5061
EXPOSE 7248

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CamAI.EdgeBox.Controllers/CamAI.EdgeBox.Controllers.csproj", "CamAI.EdgeBox.Controllers/"]
COPY ["CamAI.EdgeBox.Models/CamAI.EdgeBox.Models.csproj", "CamAI.EdgeBox.Models/"]
COPY ["CamAI.EdgeBox.Repositories/CamAI.EdgeBox.Repositories.csproj", "CamAI.EdgeBox.Repositories/"]
COPY ["CamAI.EdgeBox.Services/CamAI.EdgeBox.Services.csproj", "CamAI.EdgeBox.Services/"]
RUN dotnet restore ./CamAI.EdgeBox.Controllers/CamAI.EdgeBox.Controllers.csproj
COPY . .
WORKDIR /src/CamAI.EdgeBox.Controllers
RUN dotnet build ./CamAI.EdgeBox.Controllers.csproj -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish ./CamAI.EdgeBox.Controllers.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet CamAI.EdgeBox.Controllers.dll