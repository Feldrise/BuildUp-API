FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY BuildUp.API/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY BuildUp.API/. ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
COPY BuildUp.API/BuildUp.API.xml ./
COPY BuildUp.API/PDF ./PDF
COPY BuildUp.API/Emails/html ./Emails/html

VOLUME [ "/app/Emails", "/app/wwwroot/pdf" ]

ENTRYPOINT ["dotnet", "BuildUp.API.dll"]