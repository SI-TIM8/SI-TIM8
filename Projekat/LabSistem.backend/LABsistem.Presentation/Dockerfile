FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["LABsistem.Presentation/LABsistem.Presentation.csproj", "LABsistem.Presentation/"]
COPY ["LABsistem.Api/LABsistem.Api.csproj", "LABsistem.Api/"]
COPY ["LabSistem.Dal/LabSistem.Dal.csproj", "LabSistem.Dal/"]
COPY ["LabSistem.Domain/LABsistem.Domain.csproj", "LabSistem.Domain/"]

RUN dotnet restore "LABsistem.Presentation/LABsistem.Presentation.csproj"

COPY . .

WORKDIR "/src/LABsistem.Presentation"
RUN dotnet build "LABsistem.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "LABsistem.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LABsistem.Presentation.dll"]
