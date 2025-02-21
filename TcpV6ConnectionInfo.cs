using System.Net;

internal record TcpV6ConnectionInfo(
    int OwnerPid, 
    string? OwnerModuleName, 
    string? OwnerModulePath, 
    DateTime? Created, 
    TcpState TcpState, 
    IPEndPoint Local, 
    IPEndPoint Remote);