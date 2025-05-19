FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# api
COPY ["LiftNet.Api/LiftNet.Api.csproj", "LiftNet.Api/"]

# application
COPY ["LiftNet.Handler/LiftNet.Handler.csproj", "LiftNet.Handler/"]
COPY ["LiftNet.Service/LiftNet.Service.csproj", "LiftNet.Service/"]
COPY ["LiftNet.SharedKenel/LiftNet.SharedKenel.csproj", "LiftNet.SharedKenel/"]
COPY ["LiftNet.Engine/LiftNet.Engine.csproj", "LiftNet.Engine/"]
COPY ["LiftNet.Hub/LiftNet.Hub.csproj", "LiftNet.Hub/"]

# common
COPY ["LiftNet.Contract/LiftNet.Contract.csproj", "LiftNet.Contract/"]
COPY ["LiftNet.Utility/LiftNet.Utility.csproj", "LiftNet.Utility/"]

# domain
COPY ["LiftNet.Domain/LiftNet.Domain.csproj", "LiftNet.Domain/"]
COPY ["LiftNet.Logger/LiftNet.Logger.csproj", "LiftNet.Logger/"]
COPY ["LiftNet.Persistence/LiftNet.Persistence.csproj", "LiftNet.Persistence/"]
COPY ["LiftNet.Ioc/LiftNet.Ioc.csproj", "LiftNet.Ioc/"]

# infra
COPY ["LiftNet.ProvinceSDK/LiftNet.ProvinceSDK.csproj", "LiftNet.ProvinceSDK/"]
COPY ["LiftNet.MapSDK/LiftNet.MapSDK.csproj", "LiftNet.MapSDK/"]
COPY ["LiftNet.Cloudinary/LiftNet.Cloudinary.csproj", "LiftNet.Cloudinary/"]
COPY ["LiftNet.RedisCache/LiftNet.RedisCache.csproj", "LiftNet.RedisCache/"]
COPY ["LiftNet.MailService/LiftNet.MailService.csproj", "LiftNet.MailService/"]

# data
COPY ["LiftNet.AzureBlob/LiftNet.AzureBlob.csproj", "LiftNet.AzureBlob/"]
COPY ["LiftNet.CosmosDb/LiftNet.CosmosDb.csproj", "LiftNet.CosmosDb/"]
COPY ["LiftNet.Repositories/LiftNet.Repositories.csproj", "LiftNet.Repositories/"]

# event
COPY ["LiftNet.ServiceBus/LiftNet.ServiceBus.csproj", "LiftNet.ServiceBus/"]

# job
COPY ["LiftNet.Job/LiftNet.Job.csproj", "LiftNet.Job/"]
COPY ["LiftNet.JobService/LiftNet.JobService.csproj", "LiftNet.JobService/"]

# worker
COPY ["LiftNet.Timer/LiftNet.Timer.csproj", "LiftNet.Timer/"]
COPY ["LiftNet.WorkerService/LiftNet.WorkerService.csproj", "LiftNet.WorkerService/"]

# Restore dependencies
RUN dotnet restore "./LiftNet.Api/./LiftNet.Api.csproj"

# Copy all files and build
COPY . .
WORKDIR "/src/LiftNet.Api"
RUN dotnet build "./LiftNet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LiftNet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY LiftNet.Api/.env /app/

# Copy the published app
COPY --from=publish /app/publish /app

ENTRYPOINT ["dotnet", "LiftNet.Api.dll"]