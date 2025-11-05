# Stage 1: Build Angular client
FROM node:20-alpine AS client-build
WORKDIR /app/client
COPY Client/package*.json ./
RUN npm ci --silent
RUN npm install 
COPY Client/ .
RUN npm run build

# Stage 2: Build .NET server
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS server-build
WORKDIR /app/server
COPY Server/Server/Server.csproj ./
RUN dotnet restore
COPY Server/Server/. .
# Build and publish with explicit runtime
RUN dotnet publish -c Release -o out --runtime linux-x64 --self-contained false

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=client-build /app/client/dist/AngularApp/browser/ ./wwwroot/
COPY --from=server-build /app/server/out ./

EXPOSE 80

# Add environment variable to disable HTTPS redirection in production
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Server.dll"]
