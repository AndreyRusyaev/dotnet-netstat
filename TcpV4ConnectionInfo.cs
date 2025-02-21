using System.Net;

internal record TcpV4ConnectionInfo(
    int OwnerPid, 
    string? OwnerModuleName, 
    string? OwnerModulePath, 
    DateTime? Created, 
    TcpState TcpState, 
    IPEndPoint Local, 
    IPEndPoint Remote);
