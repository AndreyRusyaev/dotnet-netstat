using System.Net;

var netstat = new Netstat();

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

foreach (var tcpConnectionInfo in netstat.GetTcpV4Connections())
{
    string shortenedModuleName = tcpConnectionInfo.OwnerModuleName ?? "<Unknown>";
    if (shortenedModuleName.Length > 24)
    {
        shortenedModuleName = shortenedModuleName.Substring(0, 22) + "..";
    }

    string local_ip = tcpConnectionInfo.LocalHost.ToString();
    if (tcpConnectionInfo.LocalHost.Equals(IPAddress.Any))
    {
        local_ip = "<any>";
    }

    string remote_ip = tcpConnectionInfo.RemoteHost.ToString();
    if (tcpConnectionInfo.RemoteHost.Equals(IPAddress.Any))
    {
        remote_ip = "<any>";
    }

    Console.WriteLine(
        "[{0,-5}] {1,-24} {2,-16} {3,-5} <-> {4,-16} {5,-5} {6,-11} {7}",
        tcpConnectionInfo.OwnerPid,
        shortenedModuleName,
        local_ip,
        tcpConnectionInfo.LocalPort,
        remote_ip,
        tcpConnectionInfo.RemotePort,
        tcpConnectionInfo.TcpState,
        tcpConnectionInfo.Created);
}
