using System.Net;

internal record UdpConnectionInfo(
    int OwnerPid, 
    string? OwnerModuleName, 
    string? OwnerModulePath, 
    DateTimeOffset? Created, 
    IPEndPoint Local);