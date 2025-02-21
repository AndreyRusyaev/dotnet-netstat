using System.Net;

Console.WriteLine("UDP:");

Console.WriteLine(
    "[{0,-5}] {1,-24} {2,-16} {3,-5} {4}",
    "pid",
    "module",
    "ip",
    "port",
    "active_time");

var netstat = new Netstat();

foreach (var udpConnectionInfo in netstat.GetUdpConnections())
{
    string shortenedModuleName = udpConnectionInfo.OwnerModuleName ?? "<Unknown>";
    if (shortenedModuleName.Length > 24)
    {
        shortenedModuleName = shortenedModuleName.Substring(0, 22) + "..";
    }

    string local_ip = udpConnectionInfo.Local.Address.ToString();
    if (udpConnectionInfo.Local.Address.Equals(IPAddress.Any))
    {
        local_ip = "0.0.0.0";
    }
    else if (udpConnectionInfo.Local.Address.Equals(IPAddress.IPv6Any))
    {
        local_ip = "[::]";
    }

    Console.WriteLine(
        "[{0,-5}] {1,-24} {2,-16} {3,-5} {4,4}",
        udpConnectionInfo.OwnerPid,
        shortenedModuleName,
        local_ip,
        udpConnectionInfo.Local.Port,
        GetActiveTime(udpConnectionInfo.Created));
}

Console.WriteLine();

Console.WriteLine("TCP:");

Console.WriteLine(
    "[{0,-5}] {1,-24} {2,-16} {3,-5} <-> {4,-16} {5,-5} {6,-11} {7}",
    "pid",
    "module",
    "local_ip",
    "lport",
    "remote_ip",
    "rport",
    "tcp_state",
    "active_time");

foreach (var tcpConnectionInfo in netstat.GetTcpConnections())
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
    else if (tcpConnectionInfo.Local.Address.Equals(IPAddress.IPv6Any))
    {
        local_ip = "[::]";
    }

    string remote_ip = tcpConnectionInfo.Remote.Address.ToString();
    if (tcpConnectionInfo.Remote.Address.Equals(IPAddress.Any))
    {
        remote_ip = "0.0.0.0";
    }
    else if (tcpConnectionInfo.Remote.Address.Equals(IPAddress.IPv6Any))
    {
        remote_ip = "[::]";
    }

    Console.WriteLine(
        "[{0,-5}] {1,-24} {2,-16} {3,-5} <-> {4,-16} {5,-5} {6,-11} {7,4}",
        tcpConnectionInfo.OwnerPid,
        shortenedModuleName,
        local_ip,
        tcpConnectionInfo.Local.Port,
        remote_ip,
        tcpConnectionInfo.Remote.Port,
        tcpConnectionInfo.TcpState,
        GetActiveTime(tcpConnectionInfo.Created));
}

string GetActiveTime(DateTime? created)
{
    if (created == null)
    {
        return "";
    }

    TimeSpan activeTime = DateTime.Now - created.Value;

    if (activeTime.TotalSeconds < 60)
    {
        return $"{(int)activeTime.TotalSeconds}s";
    }

    if (activeTime.TotalMinutes < 60)
    {
        return $"{(int)activeTime.TotalMinutes}m";    
    }

    var totalHours = (int)activeTime.TotalMinutes / 60;
    var minutes = (int)activeTime.TotalMinutes % 60;

    return $"{totalHours}h{minutes}m";
}