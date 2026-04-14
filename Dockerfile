FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy project files first to cache the restore layer
COPY AIDocumentAnalyzer/AIDocumentAnalyzer.csproj AIDocumentAnalyzer/
COPY Library/Library.csproj Library/
RUN dotnet restore AIDocumentAnalyzer/AIDocumentAnalyzer.csproj
# Copy remaining source and publish
COPY . .
RUN dotnet publish AIDocumentAnalyzer/AIDocumentAnalyzer.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
# Run as non-root user for container security
RUN adduser --disabled-password --no-create-home appuser
COPY --from=build /app/publish .
USER appuser
ENTRYPOINT ["dotnet", "AIDocumentAnalyzer.dll"]
