FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Identity.Servier/Identity.Servier.csproj", "Identity.Servier/"]
RUN dotnet restore "Identity.Servier/Identity.Servier.csproj"
COPY . .
WORKDIR "/src/Identity.Servier"
RUN dotnet build "Identity.Servier.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Servier.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Servier.dll"]