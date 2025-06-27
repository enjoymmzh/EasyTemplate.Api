#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ShareManagement.Entry/ShareManagement.Entry.csproj", "ShareManagement.Entry/"]
COPY ["ShareManagement.Service/ShareManagement.Service.csproj", "ShareManagement.Service/"]
COPY ["ShareManagement.Tool/ShareManagement.Tool.csproj", "ShareManagement.Tool/"]
RUN dotnet restore "./ShareManagement.Entry/ShareManagement.Entry.csproj"
COPY . .
WORKDIR "/src/ShareManagement.Entry"
RUN dotnet build "./ShareManagement.Entry.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShareManagement.Entry.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShareManagement.Entry.dll"]