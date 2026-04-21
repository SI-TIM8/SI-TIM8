# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LABsistem.Presentation/LABsistem.Presentation.csproj", "LABsistem.Presentation/"]
# Ovdje moraš dodati SVE projekte iz tvoje 4-slojne arhitekture
COPY ["LABsistem.Presentation/LABsistem.Presentation.csproj", "LABsistem.Presentation/"]
COPY ["LABsistem.Api/LABsistem.Api.csproj", "LABsistem.Api/"]
COPY ["LABsistem.Dal/LABsistem.Dal.csproj", "LABsistem.Dal/"]
COPY ["LABsistem.Domain/LABsistem.Domain.csproj", "LABsistem.Domain/"]

# Onda uradiš restore za glavni projekat (on će povući i ostale)
RUN dotnet restore "LABsistem.Presentation/LABsistem.Presentation.csproj"
RUN dotnet restore "./LABsistem.Presentation/LABsistem.Presentation.csproj"
COPY . .
WORKDIR "/src/LABsistem.Presentation"
RUN dotnet build "./LABsistem.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LABsistem.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LABsistem.Presentation.dll"]