using System.Net;

Console.WriteLine(
    "[{0,-5}] {1,-24} {2,-16} {3,-5} <-> {4,-16} {5,-5} {6,-11} {7}",
    "pid",
    "module",
    "local_ip",
    "lport",
    "remote_ip",
    "rport",
    "tcp_state",
    "creation_date");

var netstat = new Netstat();

foreach (var tcpConnectionInfo in netstat.GetTcpV4Connections())
{
    string shortenedModuleName = tcpConnectionInfo.OwnerModuleName ?? "<Unknown>";
    if (shortenedModuleName.Length > 24)
    {
        shortenedModuleName = shortenedModuleName.Substring(0, 22) + "..";
    }

    string local_ip = tcpConnectionInfo.Local.Address.ToString();
    if (tcpConnectionInfo.Local.Address.Equals(IPAddress.Any))
    {
        local_ip = "0.0.0.0";
    }

    string remote_ip = tcpConnectionInfo.Remote.Address.ToString();
    if (tcpConnectionInfo.Remote.Address.Equals(IPAddress.Any))
    {
        remote_ip = "0.0.0.0";
    }

    Console.WriteLine(
        "[{0,-5}] {1,-24} {2,-16} {3,-5} <-> {4,-16} {5,-5} {6,-11} {7}",
        tcpConnectionInfo.OwnerPid,
        shortenedModuleName,
        local_ip,
        tcpConnectionInfo.Local.Port,
        remote_ip,
        tcpConnectionInfo.Remote.Port,
        tcpConnectionInfo.TcpState,
        tcpConnectionInfo.Created);
}

foreach (var tcpConnectionInfo in netstat.GetTcpV6Connections())
{
    string shortenedModuleName = tcpConnectionInfo.OwnerModuleName ?? "<Unknown>";
    if (shortenedModuleName.Length > 24)
    {
        shortenedModuleName = shortenedModuleName.Substring(0, 22) + "..";
    }

    string local_ip = tcpConnectionInfo.Local.Address.ToString();
    if (tcpConnectionInfo.Local.Address.Equals(IPAddress.IPv6Any))
    {
        local_ip = "[::]";
    }

    string remote_ip = tcpConnectionInfo.Remote.Address.ToString();
    if (tcpConnectionInfo.Remote.Address.Equals(IPAddress.IPv6Any))
    {
        remote_ip = "[::]";
    }

    Console.WriteLine(
        "[{0,-5}] {1,-24} {2,-16} {3,-5} <-> {4,-16} {5,-5} {6,-11} {7}",
        tcpConnectionInfo.OwnerPid,
        shortenedModuleName,
        local_ip,
        tcpConnectionInfo.Local.Port,
        remote_ip,
        tcpConnectionInfo.Remote.Port,
        tcpConnectionInfo.TcpState,
        tcpConnectionInfo.Created);
}
