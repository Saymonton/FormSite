# ====== BUILD ======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copia tudo do projeto (coloque este Dockerfile na pasta do .csproj)
COPY . .
# Restaura e publica
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# ====== RUNTIME ======
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
# O Render/Compose vão mapear essa porta; mantenha 8080 pra evitar conflito
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FormSite.dll"]
