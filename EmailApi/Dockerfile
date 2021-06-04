#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["tgs-email.csproj", "."]
RUN dotnet restore "./tgs-email.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "tgs-email.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "tgs-email.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "tgs-email.dll"]