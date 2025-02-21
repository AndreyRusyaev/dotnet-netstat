# dotnet-netstat

Show active tcp connections with module name.

## How to run it

Ensure that .Net 8 or later is installed
```
dotnet --version
```

Clone repository and use `dotnet run` command:
``` shell
git clone https://github.com/AndreyRusyaev/dotnet-netstat
cd dotnet-netstat
dotnet run
```

Output:
```shell
[pid  ] module                   local_ip         lport <-> remote_ip        rport tcp_state   creation_date
...
[13444] firefox.exe              127.0.0.1        49726 <-> 34.107.243.93    443   Established 21.02.2025 14:18:50
[13444] firefox.exe              127.0.0.1        49758 <-> 23.88.75.177     443   Established 21.02.2025 14:18:52
[13444] firefox.exe              127.0.0.1        49829 <-> 13.107.42.14     443   Established 21.02.2025 14:18:59
...
```

# Prerequisites
.Net 8.0 or higher.

# Usage

```
dotnet run
```

# Examples

## Show active tcp connections (user mode)
``` shell
git clone https://github.com/AndreyRusyaev/dotnet-netstat
cd dotnet-netstat
dotnet run
```
