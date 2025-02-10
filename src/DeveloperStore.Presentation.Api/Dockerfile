FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/DeveloperStore.Presentation.Api/DeveloperStore.Presentation.Api.csproj", "src/DeveloperStore.Presentation.Api/"]
COPY ["src/DeveloperStore.Presentation.Endpoints/DeveloperStore.Presentation.Endpoints.csproj", "src/DeveloperStore.Presentation.Endpoints/"]
COPY ["src/DeveloperStore.Application/DeveloperStore.Application.csproj", "src/DeveloperStore.Application/"]
COPY ["src/DeveloperStore.Domain/DeveloperStore.Domain.csproj", "src/DeveloperStore.Domain/"]
COPY ["src/DeveloperStore.Infra.Data/DeveloperStore.Infra.Data.csproj", "src/DeveloperStore.Infra.Data/"]
RUN dotnet restore "src/DeveloperStore.Presentation.Api/DeveloperStore.Presentation.Api.csproj"
COPY . .
WORKDIR "/src/src/DeveloperStore.Presentation.Api"
RUN dotnet build "DeveloperStore.Presentation.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeveloperStore.Presentation.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeveloperStore.Presentation.Api.dll"] 