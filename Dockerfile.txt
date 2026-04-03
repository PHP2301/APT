FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APT.sln", "./"]
COPY ["APT/APT.csproj", "APT/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/APT"
RUN dotnet publish "APT.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "APT.dll"]