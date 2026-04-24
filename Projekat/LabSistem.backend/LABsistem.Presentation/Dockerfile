FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Kopiranje fajlova projekata (PAZI NA VELIKA SLOVA - LABSistem)
COPY ["LABsistem.sln", "./"]
COPY ["LABsistem.Presentation/LABsistem.Presentation.csproj", "LABsistem.Presentation/"]
COPY ["LABsistem.Api/LABsistem.Api.csproj", "LABsistem.Api/"]
COPY ["LABsistem.Dal/LABsistem.Dal.csproj", "LABsistem.Dal/"]
COPY ["LABsistem.Domain/LABsistem.Domain.csproj", "LABsistem.Domain/"]

# 2. Restore za cijeli solution (ovo rješava zavisnosti između projekata)
RUN dotnet restore "LABsistem.sln"

# 3. Kopiranje cijelog koda
COPY . .

# 4. Build Presentation projekta
WORKDIR "/src/LABsistem.Presentation"
RUN dotnet build "LABsistem.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "LABsistem.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LABsistem.Presentation.dll"]