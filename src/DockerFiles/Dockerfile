FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source
EXPOSE 80
EXPOSE 443

# copy csproj and restore as distinct layers
COPY *.sln .
COPY OkToWake/*.csproj ./OkToWake/
RUN dotnet restore

# copy everything else and build app
COPY OkToWake/. ./OkToWake/
WORKDIR /source/OkToWake
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "OkToWake.dll"]