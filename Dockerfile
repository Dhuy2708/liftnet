FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Api/LiftNet.Api/LiftNet.Api.csproj Api/LiftNet.Api/
COPY Application/Common/LiftNet.Contract/LiftNet.Contract.csproj Application/Common/LiftNet.Contract/
COPY Application/Domain/LiftNet.Domain/LiftNet.Domain.csproj Application/Domain/LiftNet.Domain/
COPY Application/LiftNet.Service/LiftNet.Service.csproj Application/LiftNet.Service/
COPY Application/Domain/LiftNet.Persistence/LiftNet.Persistence.csproj Application/Domain/LiftNet.Persistence/
COPY Application/Common/LiftNet.Utility/LiftNet.Utility.csproj Application/Common/LiftNet.Utility/
COPY Infrastructure/Data/LiftNet.Repositories/LiftNet.Repositories.csproj Infrastructure/Data/LiftNet.Repositories/
COPY Application/LiftNet.SharedKenel/LiftNet.SharedKenel.csproj Application/LiftNet.SharedKenel/
COPY Infrastructure/Data/LiftNet.AzureBlob/LiftNet.AzureBlob.csproj Infrastructure/Data/LiftNet.AzureBlob/
COPY Application/Domain/LiftNet.Logger/LiftNet.Logger.csproj Application/Domain/LiftNet.Logger/
COPY Application/LiftNet.Handler/LiftNet.Handler.csproj Application/LiftNet.Handler/
COPY Application/Domain/LiftNet.Ioc/LiftNet.Ioc.csproj Application/Domain/LiftNet.Ioc/
COPY Infrastructure/Data/LiftNet.CosmosDb/LiftNet.CosmosDb.csproj Infrastructure/Data/LiftNet.CosmosDb/
COPY Infrastructure/LiftNet.ProvinceSDK/LiftNet.ProvinceSDK.csproj Infrastructure/LiftNet.ProvinceSDK/
COPY Infrastructure/Event/LiftNet.ServiceBus/LiftNet.ServiceBus.csproj Infrastructure/Event/LiftNet.ServiceBus/
COPY Infrastructure/LiftNet.Cloudinary/LiftNet.Cloudinary.csproj Infrastructure/LiftNet.Cloudinary/
COPY Infrastructure/LiftNet.MailService/LiftNet.MailService.csproj Infrastructure/LiftNet.MailService/
COPY Infrastructure/LiftNet.MapSDK/LiftNet.MapSDK.csproj Infrastructure/LiftNet.MapSDK/
COPY Infrastructure/LiftNet.Redis/LiftNet.Redis.csproj Infrastructure/LiftNet.Redis/
COPY Application/LiftNet.Hub/LiftNet.Hub.csproj Application/LiftNet.Hub/
COPY Application/LiftNet.Engine/LiftNet.Engine.csproj Application/LiftNet.Engine/
COPY Infrastructure/LiftNet.VNPay/LiftNet.VNPay.csproj Infrastructure/LiftNet.VNPay/
COPY Infrastructure/LiftNet.ExerciseSDK/LiftNet.ExerciseSDK.csproj Infrastructure/LiftNet.ExerciseSDK/
COPY Infrastructure/Job/LiftNet.Job/LiftNet.Job.csproj Infrastructure/Job/LiftNet.Job/
COPY Infrastructure/Job/LiftNet.JobService/LiftNet.JobService.csproj Infrastructure/Job/LiftNet.JobService/
COPY Api/LiftNet.ToolApi/LiftNet.ToolApi.csproj Api/LiftNet.ToolApi/
COPY Worker/LiftNet.Timer/LiftNet.Timer.csproj Worker/LiftNet.Timer/
COPY Worker/LiftNet.WorkerService/LiftNet.WorkerService.csproj Worker/LiftNet.WorkerService/

# Restore dependencies
RUN dotnet restore "./Api/LiftNet.Api/LiftNet.Api.csproj"

# Copy full source
COPY . .

# Build (optional, không bắt buộc nếu chỉ publish)
WORKDIR /src/Api/LiftNet.Api
RUN dotnet build "LiftNet.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
WORKDIR /src/Api/LiftNet.Api
RUN dotnet publish "LiftNet.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy env file nếu có
COPY Api/LiftNet.Api/.env /app/

# Copy published app từ publish stage
COPY --from=publish /app/publish .

# Run
ENTRYPOINT ["dotnet", "LiftNet.Api.dll"]