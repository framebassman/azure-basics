FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-alpine3.19-amd64 AS base
USER $APP_UID
WORKDIR /app
ENV HTTP_PORTS 80
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0.203-alpine3.19-amd64 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProjectTasks.Api/ProjectTasks.Api.csproj", "ProjectTasks.Api.csproj"]
RUN dotnet restore "ProjectTasks.Api.csproj"
COPY ./ProjectTasks.Api .
WORKDIR "/src"
RUN dotnet build "ProjectTasks.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjectTasks.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectTasks.Api.dll"]
