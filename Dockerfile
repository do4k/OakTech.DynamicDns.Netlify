FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OakTech.DynamicDns.Netlify/OakTech.DynamicDns.Netlify.csproj", "OakTech.DynamicDns.Netlify/"]
RUN dotnet restore "OakTech.DynamicDns.Netlify/OakTech.DynamicDns.Netlify.csproj"
COPY . .
WORKDIR "/src/OakTech.DynamicDns.Netlify"
RUN dotnet build "OakTech.DynamicDns.Netlify.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OakTech.DynamicDns.Netlify.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OakTech.DynamicDns.Netlify.dll"]
