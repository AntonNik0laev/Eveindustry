FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

WORKDIR /app
COPY eveindustry.sln .

COPY Eveindustry.API/Eveindustry.API.csproj Eveindustry.API/
COPY Eveindustry.CLI/*.csproj Eveindustry.CLI/
COPY Eveindustry.Core/*.csproj Eveindustry.Core/
COPY Eveindustry.Sde/*.csproj Eveindustry.Sde/
COPY Eveindustry.Shared/*.csproj Eveindustry.Shared/
COPY Eveindustry.Tests/*.csproj Eveindustry.Tests/
COPY EveIndustry.UpdateData/*.csproj EveIndustry.UpdateData/
COPY EveIndustry.Web/*.csproj EveIndustry.Web/

RUN dotnet restore

COPY . .
 
WORKDIR /app/EveIndustry.Web
RUN dotnet publish -c Release -o publish_web

WORKDIR /app/Eveindustry.API
RUN dotnet publish -c Release -o publish_api

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as webapp
WORKDIR /app
COPY --from=build /app/Eveindustry.API/publish_api .
COPY --from=build /app/EveIndustry.Web/publish_web .

EXPOSE 80

ENTRYPOINT ["dotnet", "Eveindustry.API.dll"]