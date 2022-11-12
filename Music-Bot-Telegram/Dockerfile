# docker build -t musicbot:1.0 -f .\Music-Bot-Telegram\Dockerfile .
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["../Music-Bot-Telegram/Music-Bot-Telegram.csproj", "Music-Bot-Telegram/"]
COPY ["../Music-Bot-Telegram.Data/Music-Bot-Telegram.Data.csproj", "Music-Bot-Telegram.Data/"]
COPY ["../Music-Bot-Telegram.Services/Music-Bot-Telegram.Services.csproj", "Music-Bot-Telegram.Services/"]
RUN dotnet restore "Music-Bot-Telegram/Music-Bot-Telegram.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Music-Bot-Telegram/Music-Bot-Telegram.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Music-Bot-Telegram/Music-Bot-Telegram.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Music-Bot-Telegram.dll"]
