# Dockerfile for ASP.NET Core (.NET 8) Web API

# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["WebApiStudents.csproj", "./"]
RUN dotnet restore "WebApiStudents.csproj"

# Copy the rest of the source code and publish
COPY . .
RUN dotnet publish "WebApiStudents.csproj" -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Expose port 80 and configure entrypoint
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "WebApiStudents.dll"] 