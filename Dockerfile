FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Identity.Server/Identity.Server.csproj", "Identity.Server/"]
RUN dotnet restore "Identity.Server/Identity.Server.csproj"
COPY . .
WORKDIR "/src/Identity.Server"
RUN dotnet build "Identity.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Server.dll"]