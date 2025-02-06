FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app
EXPOSE 80

# copy all .csproj files and restore as distincts layers. Use of the same COPY command
# for every dockerfile in the project to take advantage of docker caching
COPY Teamo_Backend.sln Teamo_Backend.sln
COPY TeamoWeb.API/TeamoWeb.API.csproj TeamoWeb.API/TeamoWeb.API.csproj
COPY Teamo.Core/Teamo.Core.csproj Teamo.Core/Teamo.Core.csproj
COPY Teamo.Infrastructure/Teamo.Infrastructure.csproj Teamo.Infrastructure/Teamo.Infrastructure.csproj

# Restore package deps
RUN dotnet restore Teamo_Backend.sln

# Copy the app folders over
COPY TeamoWeb.API TeamoWeb.API
COPY Teamo.Core Teamo.Core
COPY Teamo.Infrastructure Teamo.Infrastructure
WORKDIR /app/TeamoWeb.API
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT [ "dotnet", "TeamoWeb.API.dll" ]
