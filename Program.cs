var netstat = new Netstat();

foreach(var tcpConnectionInfo in netstat.GetTcpConnections())
{
    Console.WriteLine(
        "{0,-16}:{1,-5} <-> {2,-16}:{3,-5} {4,-11} [{5,-5}] {6,-20} {7}", 
        tcpConnectionInfo.LocalHost, 
        tcpConnectionInfo.LocalPort,
        tcpConnectionInfo.RemoteHost,
        tcpConnectionInfo.RemotePort,
        tcpConnectionInfo.TcpState,
        tcpConnectionInfo.OwnerPid, 
        tcpConnectionInfo.OwnerModuleName ?? "<Unknown>", 
        tcpConnectionInfo.Created);
}
