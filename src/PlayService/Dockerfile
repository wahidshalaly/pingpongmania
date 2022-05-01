#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG NET_IMAGE=6.0-bullseye-slim
FROM mcr.microsoft.com/dotnet/aspnet:${NET_IMAGE} AS base
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:${NET_IMAGE} AS build
WORKDIR /src
COPY ["PlayService/PlayService.csproj", "PlayService/PlayService.csproj"]
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/SharedLibrary.csproj"]
RUN dotnet restore "PlayService/PlayService.csproj"
COPY PlayService/ PlayService/
COPY SharedLibrary/ SharedLibrary/

FROM build AS publish
WORKDIR /src/PlayService
RUN dotnet publish --no-restore "PlayService.csproj" -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=publish /out /app
ENTRYPOINT ["dotnet", "PlayService.dll"]
