using System.Runtime.InteropServices;

internal sealed class Netstat
{
    public IEnumerable<TcpConnectionInfo> GetTcpConnections()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WinNetstat().GetTcpConnections();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new MacNetstat().GetTcpConnections();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new LinuxNetstat().GetTcpConnections();
        }
        else 
        {
            throw new NotSupportedException($"Platform '{RuntimeInformation.OSDescription}' is not supported.");
        }
    }

    public IEnumerable<UdpConnectionInfo> GetUdpConnections()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new WinNetstat().GetUdpConnections();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new MacNetstat().GetUdpConnections();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new LinuxNetstat().GetUdpConnections();
        }
        else 
        {
            throw new NotSupportedException($"Platform '{RuntimeInformation.OSDescription}' is not supported.");
        }
    }
}