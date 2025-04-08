FROM mcr.microsoft.com/dotnet/sdk:8.0-noble

# RUN apt update && apt install -y iproute2 net-tools lsof netcat-traditional

WORKDIR /app

# copy and run
COPY . .
ENTRYPOINT [ "dotnet", "run" ]