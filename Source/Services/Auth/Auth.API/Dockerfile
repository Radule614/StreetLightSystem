FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 12000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "StreetLightSystem.sln" "StreetLightSystem.sln"

COPY "ApiGateway/ApiGateway/ApiGateway.csproj" "ApiGateway/ApiGateway/ApiGateway.csproj"
COPY "BuildingBlocks/Common/Common.csproj" "BuildingBlocks/Common/Common.csproj"
COPY "Services/MqttSimulator/MqttSimulator/MqttSimulator.csproj" "Services/MqttSimulator/MqttSimulator/MqttSimulator.csproj"
COPY "Services/MqttClient/MqttClient/MqttClient.csproj" "Services/MqttClient/MqttClient/MqttClient.csproj"
#Auth Microservice
COPY "Services/Auth/Auth.API/Auth.API.csproj" "Services/Auth/Auth.API/Auth.API.csproj"
COPY "Services/Auth/Auth.UnitTests/Auth.UnitTests.csproj" "Services/Auth/Auth.UnitTests/Auth.UnitTests.csproj"
#Notification Microservice
COPY "Services/Notification/Notification.API/Notification.API.csproj" "Services/Notification/Notification.API/Notification.API.csproj"
COPY "Services/Notification/Notification.UnitTests/Notification.UnitTests.csproj" "Services/Notification/Notification.UnitTests/Notification.UnitTests.csproj"
#Pole Microservice
COPY "Services/Pole/Pole.API/Pole.API.csproj" "Services/Pole/Pole.API/Pole.API.csproj"
COPY "Services/Pole/Pole.UnitTests/Pole.UnitTests.csproj" "Services/Pole/Pole.UnitTests/Pole.UnitTests.csproj"
#Repair Microservice
COPY "Services/Repair/Repair.API/Repair.API.csproj" "Services/Repair/Repair.API/Repair.API.csproj"
COPY "Services/Repair/Repair.UnitTests/Repair.UnitTests.csproj" "Services/Repair/Repair.UnitTests/Repair.UnitTests.csproj"
#Team Microservice
COPY "Services/Team/Team.API/Team.API.csproj" "Services/Team/Team.API/Team.API.csproj"
COPY "Services/Team/Team.UnitTests/Team.UnitTests.csproj" "Services/Team/Team.UnitTests/Team.UnitTests.csproj"
#User Microservice
COPY "Services/User/User.API/User.API.csproj" "Services/User/User.API/User.API.csproj"
COPY "Services/User/User.UnitTests/User.UnitTests.csproj" "Services/User/User.UnitTests/User.UnitTests.csproj"

RUN dotnet restore "StreetLightSystem.sln"

COPY . .
WORKDIR /src/Services/Auth/Auth.API
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENV ASPNETCORE_URLS http://*:12000
ENTRYPOINT ["dotnet", "Auth.API.dll"]