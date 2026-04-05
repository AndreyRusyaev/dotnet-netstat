using System.Net;

Console.WriteLine("UDP:");

Console.WriteLine(
    "[{0,-5}] {1,-24} {2,-30} {3,-5} {4,4}",
    "pid",
    "module",
    "local_ip",
    "lport",
    "active_time");

var netstat = new Netstat();

foreach (var udpConnectionInfo in netstat.GetUdpConnections().OrderBy(x => x.OwnerPid).ThenBy(x => x.Local.Address.ToString()))
{
    string shortenedModuleName = Formatting.FormatModuleName(udpConnectionInfo.OwnerModuleName);
    string local_ip = Formatting.FormatIpAddress(udpConnectionInfo.Local.Address);
    string activeTime = Formatting.FormatActiveTime(DateTimeOffset.Now - udpConnectionInfo.Created);

    Console.WriteLine(
        "[{0,-5}] {1,-24} {2,-30} {3,-5} {4,4}",
        udpConnectionInfo.OwnerPid,
        shortenedModuleName,
        local_ip,
        udpConnectionInfo.Local.Port,
        activeTime);
}

Console.WriteLine();

Console.WriteLine("TCP:");

Console.WriteLine(
    "[{0,-5}] {1,-24} {2,-30} {3,-5} <-> {4,-30} {5,-5} {6,-12} {7,4}",
    "pid",
    "module",
    "local_ip",
    "lport",
    "remote_ip",
    "rport",
    "tcp_state",
    "active_time");

foreach (var tcpConnectionInfo in netstat.GetTcpConnections().OrderBy(x => x.OwnerPid).ThenBy(x => x.Local.Address.ToString()))
{
    string shortenedModuleName = Formatting.FormatModuleName(tcpConnectionInfo.OwnerModuleName);

    string local_ip = Formatting.FormatIpAddress(tcpConnectionInfo.Local.Address);
    string remote_ip = Formatting.FormatIpAddress(tcpConnectionInfo.Remote.Address);
    string activeTime = Formatting.FormatActiveTime(DateTimeOffset.Now - tcpConnectionInfo.Created);

    Console.WriteLine(
        "[{0,-5}] {1,-24} {2,-30} {3,-5} <-> {4,-30} {5,-5} {6,-12} {7,4}",
        tcpConnectionInfo.OwnerPid,
        shortenedModuleName,
        local_ip,
        tcpConnectionInfo.Local.Port,
        remote_ip,
        tcpConnectionInfo.Remote.Port,
        tcpConnectionInfo.TcpState,
        activeTime);
}