# ---
# First stage (build)
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# Copy solution as distinct layer
COPY VoucherGenerationService.sln .
COPY VoucherGenerationService/VoucherGenerationService.csproj ./VoucherGenerationService/
COPY VoucherGenerationService/VoucherGenerationService.csproj ./VoucherGenerationService/
RUN dotnet restore

# Copy everything else and build
COPY VoucherGenerationService/. ./VoucherGenerationService/
WORKDIR /app/VoucherGenerationService
RUN dotnet publish -c Release -o out

# ---
# Second stage (execution)
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime

WORKDIR /app
COPY --from=build /app/VoucherGenerationService/out ./

# Run on localhost:8779
ENV ASPNETCORE_URLS http://+:8779
EXPOSE 8779

# Drop privileges
USER 1000

ENTRYPOINT ["dotnet", "WomPlatform.Web.Generator.dll"]
