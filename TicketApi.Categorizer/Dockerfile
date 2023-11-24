FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "TicketApi.Categorizer/TicketApi.Categorizer.csproj"
RUN dotnet build "TicketApi.Categorizer/TicketApi.Categorizer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TicketApi.Categorizer/TicketApi.Categorizer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicketApi.Categorizer.dll"]
