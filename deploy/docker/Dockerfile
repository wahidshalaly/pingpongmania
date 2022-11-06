#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG NET_IMAGE=6.0-bullseye-slim
ARG ASSEMBLY_NAME
FROM mcr.microsoft.com/dotnet/aspnet:${NET_IMAGE} AS base
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

WORKDIR /app
COPY ./ ./
ENTRYPOINT ["dotnet", "${ASSEMBLY_NAME}"]