FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WebApiStudents/WebApiStudents.csproj", "WebApiStudents/"]
RUN dotnet restore "WebApiStudents/WebApiStudents.csproj"
COPY . .
WORKDIR "/src/WebApiStudents"
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet build "WebApiStudents.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiStudents.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "WebApiStudents.dll"] 