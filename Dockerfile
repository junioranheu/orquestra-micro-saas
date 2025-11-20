# ============================
# 1) Build stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia a solução e arquivos de projeto
COPY Orquestra.sln ./
COPY Orquestra.API/Orquestra.API.csproj Orquestra.API/
COPY Orquestra.Application/Orquestra.Application.csproj Orquestra.Application/
COPY Orquestra.Domain/Orquestra.Domain.csproj Orquestra.Domain/
COPY Orquestra.Infrastructure/Orquestra.Infrastructure.csproj Orquestra.Infrastructure/
COPY Orquestra.Utils/Orquestra.Utils.csproj Orquestra.Utils/
COPY Orquestra.UnitTests/Orquestra.UnitTests.csproj Orquestra.UnitTests/
COPY Orquestra.IntegrationTests/Orquestra.IntegrationTests.csproj Orquestra.IntegrationTests/

# Restaura dependências
RUN dotnet restore Orquestra.sln

# Copia o resto
COPY . .

# Build + publish
RUN dotnet publish Orquestra.API/Orquestra.API.csproj -c Release -o /app/publish

# ============================
# 2) Runtime stage
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://0.0.0.0:5000

ENTRYPOINT ["dotnet", "Orquestra.API.dll"]