FROM mcr.microsoft.com/dotnet/sdk:8.0-noble

WORKDIR /app

# copy and run
COPY . .
ENTRYPOINT [ "dotnet", "run" ]