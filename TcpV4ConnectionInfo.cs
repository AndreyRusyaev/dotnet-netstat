using System.Net;

public record TcpV4ConnectionInfo(
    int OwnerPid, 
    string? OwnerModuleName, 
    string? OwnerModulePath, 
    DateTime Created, 
    TcpState TcpState, 
    IPAddress LocalHost, 
    int LocalPort, 
    IPAddress RemoteHost, 
    int RemotePort);