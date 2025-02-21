using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

class Netstat
{
    const int ERROR_ACCESS_DENIED = 5;

    const int ERROR_INSUFFICIENT_BUFFER = 122;

    public IEnumerable<TcpConnectionInfo> GetTcpConnections()
    {
        IntPtr tablePointer = IntPtr.Zero;
        int tableSize = 0;

        try
        {
            int result = IpHlpApi.GetExtendedTcpTable(tablePointer, ref tableSize, true, IpHlpApi.AF_INET, IpHlpApi.TCP_TABLE_CLASS.TCP_TABLE_OWNER_MODULE_ALL, 0);
            if (result != ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception(result);
            }

            tablePointer = Marshal.AllocHGlobal(tableSize);

            result = IpHlpApi.GetExtendedTcpTable(tablePointer, ref tableSize, true, IpHlpApi.AF_INET, IpHlpApi.TCP_TABLE_CLASS.TCP_TABLE_OWNER_MODULE_ALL, 0);
            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            IpHlpApi.MIB_TCPTABLE_OWNER_MODULE table = Marshal.PtrToStructure<IpHlpApi.MIB_TCPTABLE_OWNER_MODULE>(tablePointer);

            foreach (IntPtr tableEntryPtr in table.GetTableEntries(tablePointer))
            {
                IpHlpApi.MIB_TCPROW_OWNER_MODULE tableEntry = table.GetTableEntry(tableEntryPtr);
                string? moduleName = null;
                string? modulePath = null;

                IntPtr moduleInfoPtr = IntPtr.Zero;
                int moduleInfoSize = 0;
                try
                {
                    result = IpHlpApi.GetOwnerModuleFromTcpEntry(
                        tableEntryPtr, 
                        IpHlpApi.TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC, 
                        moduleInfoPtr, 
                        ref moduleInfoSize);
                    if (result == ERROR_INSUFFICIENT_BUFFER)
                    {
                        moduleInfoPtr = Marshal.AllocHGlobal(moduleInfoSize);

                        result = IpHlpApi.GetOwnerModuleFromTcpEntry(
                            tableEntryPtr,
                            IpHlpApi.TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC,
                            moduleInfoPtr,
                            ref moduleInfoSize);
                        if (result != 0)
                        {
                            throw new Win32Exception(result);
                        }

                        IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO moduleInfo = Marshal.PtrToStructure<IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO>(moduleInfoPtr);
                        moduleName = moduleInfo.pModuleName;
                        modulePath = moduleInfo.pModulePath;
                    }
                    else
                    {
                        // Failed to retrieve module info (Access denied, etc.)
                    }
                }
                finally
                {
                    if (moduleInfoPtr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(moduleInfoPtr);
                    }
                }

                yield return new TcpConnectionInfo(
                    tableEntry.dwOwningPid,
                    moduleName,
                    modulePath,
                    DateTime.FromFileTime(tableEntry.liCreateTimestamp),
                    GetTcpState(tableEntry.dwState),
                    GetIpHostName(tableEntry.dwLocalAddr),
                    GetPort(tableEntry.dwLocalPort),
                    GetIpHostName(tableEntry.dwRemoteAddr),
                    GetPort(tableEntry.dwRemotePort));

            }
        }
        finally
        {
            if (tablePointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(tablePointer);
            }
        }
    }

    IPAddress GetIpHostName(int ip)
    {
        return new IPAddress((uint)ip);
    }

    int GetPort(int port)
    {
        return (ushort)IPAddress.NetworkToHostOrder((short)port);
    }

    TcpState GetTcpState(IpHlpApi.MIB_TCP_STATE state)
    {
        switch (state)
        {
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_CLOSED:
                return TcpState.Closed;
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_LISTEN:
                return TcpState.Listening;
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_SYN_SENT:
                return TcpState.SynSent;
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_SYN_RCVD:
                return TcpState.SynReceived;
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_ESTAB:
                return TcpState.Established;
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_FIN_WAIT1:
                return TcpState.FinWait;
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_FIN_WAIT2:
                return TcpState.FinWait2;
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_CLOSE_WAIT:
                return TcpState.CloseWait;
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_CLOSING:
                return TcpState.Closing;
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_LAST_ACK:
                return TcpState.LastAck;
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_TIME_WAIT:
                return TcpState.TimeWait;
            case IpHlpApi.MIB_TCP_STATE.MIB_TCP_STATE_DELETE_TCB:
                return TcpState.DeleteTcb;
        }

        return TcpState.Unknown;
    }

    public record TcpConnectionInfo(int OwnerPid, string? OwnerModuleName, string? OwnerModulePath, DateTime Created, TcpState TcpState, IPAddress LocalHost, int LocalPort, IPAddress RemoteHost, int RemotePort);

    public enum TcpState
    {
        Unknown,
        Closed,
        Listening,
        SynSent,
        SynReceived,
        Established,
        FinWait,
        FinWait2,
        CloseWait,
        Closing,
        LastAck,
        TimeWait,
        DeleteTcb
    }
}