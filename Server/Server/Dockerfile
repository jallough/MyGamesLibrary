FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy entire project first to avoid missing imports/props during restore
COPY . ./

# Restore and publish
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
RUN dotnet restore "Server.csproj"
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "Server.dll"]
