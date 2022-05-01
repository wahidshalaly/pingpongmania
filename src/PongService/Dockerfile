#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG NET_IMAGE=6.0-bullseye-slim
FROM mcr.microsoft.com/dotnet/aspnet:${NET_IMAGE} AS base
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:${NET_IMAGE} AS build
WORKDIR /src
COPY ["PongService/PongService.csproj", "PongService/PongService.csproj"]
RUN dotnet restore "PongService/PongService.csproj"
COPY PongService/ PongService/

FROM build AS publish
WORKDIR /src/PongService
RUN dotnet publish --no-restore "PongService.csproj" -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=publish /out /app
ENTRYPOINT ["dotnet", "PongService.dll"]
