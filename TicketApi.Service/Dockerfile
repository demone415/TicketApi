FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Копируем csproj первыми, чтобы использовать кэш докера и не ресторить пакеты каждый раз когда поменялся код
COPY TicketApi/TicketApi.csproj TicketApi/
COPY TicketApi.Classifier/TicketApi.Classifier.csproj TicketApi.Classifier/
COPY TicketApi.Service/TicketApi.Service.csproj TicketApi.Service/

# Копируем остальной репозиторий, тестируем и паблишим сервис в /app
COPY . .

#RUN dotnet test TicketApi.Tests/TicketApi.Tests.csproj -c Release --filter "Category != LocalOnly" -l "console;verbosity=normal"

RUN dotnet publish TicketApi.Service/TicketApi.Service.csproj -c Release -o /app

FROM base AS final

WORKDIR /app

# Копируем только собранный сервис из предыдущего образа
COPY --from=build /app .

# Точка входа при запуске контейнера
ENTRYPOINT ["dotnet", "TicketApi.Service.dll"]

