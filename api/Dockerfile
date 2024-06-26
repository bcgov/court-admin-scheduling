FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV DOTNET_gcServer=1
ARG VERSION
ENV VERSION=$VERSION

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# install diagnostics tools
RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-counters
RUN dotnet tool install --tool-path /tools dotnet-dump

WORKDIR /

COPY ["api/api.csproj", "api/"]
COPY ["db/db.csproj", "db/"]
COPY ["jc-interface-client/jc-interface-client.csproj", "jc-interface-client/"]

RUN dotnet restore ./api/api.csproj
#RUN dotnet restore ./db/db.csproj
COPY . .

# Install Node.js and npm 
RUN apt-get update
RUN apt-get install -y nodejs npm 
RUN npm install -g autorest

# run unit tests
#RUN dotnet test -c Release

# build
FROM build AS publish
RUN dotnet publish "api/api.csproj" -c Release -o /app/publish 
# --runtime linux-musl-x64 --no-self-contained

FROM base AS final
# copy diagnostics tools
WORKDIR /tools
COPY --from=build /tools .
# copy app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api.dll"]
