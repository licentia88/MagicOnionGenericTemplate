FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
 

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["MagicT.Server/MagicT.Server.csproj", "MagicT.Server/"]
RUN dotnet restore "MagicT.Server/MagicT.Server.csproj"
COPY . .
WORKDIR "/src/MagicT.Server"
RUN dotnet build "MagicT.Server.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "MagicT.Server.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
 # Additional lines for installing icu-libs
#RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagicT.Server.dll"]
