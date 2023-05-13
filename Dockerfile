FROM mcr.microsoft.com/devcontainers/universal:2 AS build
WORKDIR /app
COPY ./src ./src
COPY ./Identity.sln ./Identity.sln
WORKDIR /app/src/Identity
RUN npm i
WORKDIR /app
RUN dotnet restore && dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT [ "dotnet", "Identity.dll" ]