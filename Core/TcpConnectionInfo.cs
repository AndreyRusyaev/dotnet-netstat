using System.Net;

internal record TcpConnectionInfo(
    int OwnerPid, 
    string? OwnerModuleName, 
    string? OwnerModulePath, 
    DateTimeOffset? Created, 
    TcpState TcpState, 
    IPEndPoint Local, 
    IPEndPoint Remote);
