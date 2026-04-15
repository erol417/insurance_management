# build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# copy csproj and restore
COPY ["InsuranceManagement.Web/InsuranceManagement.Web.csproj", "InsuranceManagement.Web/"]
RUN dotnet restore "InsuranceManagement.Web/InsuranceManagement.Web.csproj"

# copy everything else and build
COPY . .
WORKDIR "/src/InsuranceManagement.Web"
RUN dotnet build "InsuranceManagement.Web.csproj" -c Release -o /app/build

# publish stage
FROM build AS publish
RUN dotnet publish "InsuranceManagement.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .

# Environment variables for Docker
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "InsuranceManagement.Web.dll"]
