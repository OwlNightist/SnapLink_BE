# ====== Stage 1: Build ======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY SnapLink_BE.sln .
COPY SnapLink_API/SnapLink_API.csproj SnapLink_API/
COPY SnapLink_Model/SnapLink_Model.csproj SnapLink_Model/
COPY SnapLink_Repository/SnapLink_Repository.csproj SnapLink_Repository/
COPY SnapLink_Service/SnapLink_Service.csproj SnapLink_Service/
COPY Tool/Tool.csproj Tool/

# Restore packages
RUN dotnet restore "SnapLink_BE.sln"

# Copy rest of code and publish
COPY . .
RUN dotnet publish "SnapLink_API/SnapLink_API.csproj" -c Release -o /app/publish

# ====== Stage 2: Runtime ======
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Chạy HTTP trên cổng 80
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "SnapLink_API.dll"]
