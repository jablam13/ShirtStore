FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["ShirtStoreService/ShirtStoreService.csproj", "ShirtStoreService/"]
RUN dotnet restore "ShirtStoreService/ShirtStoreService.csproj"
COPY . .
WORKDIR "/src/ShirtStoreService"
RUN dotnet build "ShirtStoreService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ShirtStoreService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ShirtStoreService.dll"]