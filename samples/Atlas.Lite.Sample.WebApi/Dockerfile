FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Atlas.Lite.sln
RUN dotnet publish samples/Atlas.Lite.Sample.WebApi/Atlas.Lite.Sample.WebApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Atlas.Lite.Sample.WebApi.dll"]
