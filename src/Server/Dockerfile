# escape=`

FROM mcr.microsoft.com/powershell:nanoserver-1903 AS downloadnodejs
SHELL ["pwsh", "-Command", "$ErrorActionPreference = 'Stop';$ProgressPreference='silentlyContinue';"]
RUN Invoke-WebRequest -OutFile nodejs.zip -UseBasicParsing "https://nodejs.org/dist/v10.16.3/node-v10.16.3-win-x64.zip"; `
Expand-Archive nodejs.zip -DestinationPath C:\; `
Rename-Item "C:\node-v10.16.3-win-x64" c:\nodejs

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
COPY --from=downloadnodejs C:\nodejs\ C:\Windows\system32\

FROM microsoft/dotnet:2.2-sdk AS build
COPY --from=downloadnodejs C:\nodejs\ C:\Windows\system32\
WORKDIR /src
COPY . .
RUN dotnet restore "./Coderr.Server.Web/Coderr.Server.Web.csproj"
WORKDIR "/src/Coderr.Server.Web"
RUN dotnet build "Coderr.Server.Web.csproj" -c Release -o /app

FROM build AS publish
WORKDIR "/src/Coderr.Server.Web"
RUN dotnet publish "Coderr.Server.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Coderr.Server.Web.dll"]
