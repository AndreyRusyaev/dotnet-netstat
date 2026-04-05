using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

internal sealed class WinNetstat
{
    public IEnumerable<TcpConnectionInfo> GetTcpConnections()
    {
        foreach (var connectionInfo in GetTcpV4Connections())
        {
            yield return connectionInfo;
        }

        foreach (var connectionInfo in GetTcpV6Connections())
        {
            yield return connectionInfo;
        }
    }

    public IEnumerable<UdpConnectionInfo> GetUdpConnections()
    {
        foreach (var connectionInfo in GetUdpV4Connections())
        {
            yield return connectionInfo;
        }

        foreach (var connectionInfo in GetUdpV6Connections())
        {
            yield return connectionInfo;
        }
    }

    public IEnumerable<TcpConnectionInfo> GetTcpV4Connections()
    {
        int tcp4TableSize = 0;

        int result = IpHlpApi.GetExtendedTcpTable(IntPtr.Zero, ref tcp4TableSize, true, IpHlpApi.AF_INET, IpHlpApi.TCP_TABLE_CLASS.TCP_TABLE_OWNER_MODULE_ALL, 0);
        if (result != WinErrorCodes.ERROR_INSUFFICIENT_BUFFER)
        {
            throw new Win32Exception(result);
        }

        var tcp4TablePtr = Marshal.AllocHGlobal(tcp4TableSize);
        try
        {
            result = IpHlpApi.GetExtendedTcpTable(tcp4TablePtr, ref tcp4TableSize, true, IpHlpApi.AF_INET, IpHlpApi.TCP_TABLE_CLASS.TCP_TABLE_OWNER_MODULE_ALL, 0);
            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            IpHlpApi.MIB_TCPTABLE_OWNER_MODULE tcp4Table = Marshal.PtrToStructure<IpHlpApi.MIB_TCPTABLE_OWNER_MODULE>(tcp4TablePtr);

            foreach (IntPtr tcp4TableEntryPtr in tcp4Table.GetTableEntries(tcp4TablePtr))
            {
                IpHlpApi.MIB_TCPROW_OWNER_MODULE tcpTableEntry = tcp4Table.GetTableEntry(tcp4TableEntryPtr);
                IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO? ownerModuleBasicInfo = IpHlpApi.GetOwnerModuleBasicInfoForTcp4Entry(ref tcpTableEntry);

                yield return new TcpConnectionInfo(
                    tcpTableEntry.dwOwningPid,
                    ownerModuleBasicInfo != null ? ownerModuleBasicInfo.Value.pModuleName : null,
                    ownerModuleBasicInfo != null ? ownerModuleBasicInfo.Value.pModulePath : null,
                    tcpTableEntry.liCreateTimestamp > 0 ? DateTime.FromFileTime(tcpTableEntry.liCreateTimestamp) : null,
                    GetTcpState(tcpTableEntry.dwState),
                    GetIpV4Endpoint(tcpTableEntry.dwLocalAddr, tcpTableEntry.dwLocalPort),
                    GetIpV4Endpoint(tcpTableEntry.dwRemoteAddr, tcpTableEntry.dwRemotePort));
            }
        }
        finally
        {
            Marshal.FreeHGlobal(tcp4TablePtr);
        }
    }

    public IEnumerable<TcpConnectionInfo> GetTcpV6Connections()
    {
        int tcp6TableSize = 0;

        int result = IpHlpApi.GetExtendedTcpTable(IntPtr.Zero, ref tcp6TableSize, true, IpHlpApi.AF_INET6, IpHlpApi.TCP_TABLE_CLASS.TCP_TABLE_OWNER_MODULE_ALL, 0);
        if (result != WinErrorCodes.ERROR_INSUFFICIENT_BUFFER)
        {
            throw new Win32Exception(result);
        }

        var tcp6TablePtr = Marshal.AllocHGlobal(tcp6TableSize);
        try
        {
            result = IpHlpApi.GetExtendedTcpTable(tcp6TablePtr, ref tcp6TableSize, true, IpHlpApi.AF_INET6, IpHlpApi.TCP_TABLE_CLASS.TCP_TABLE_OWNER_MODULE_ALL, 0);
            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            IpHlpApi.MIB_TCP6TABLE_OWNER_MODULE tcp6Table = Marshal.PtrToStructure<IpHlpApi.MIB_TCP6TABLE_OWNER_MODULE>(tcp6TablePtr);

            foreach (IntPtr tcp6TableEntryPtr in tcp6Table.GetTableEntries(tcp6TablePtr))
            {
                IpHlpApi.MIB_TCP6ROW_OWNER_MODULE tcp6TableEntry = tcp6Table.GetTableEntry(tcp6TableEntryPtr);
                IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO? ownerModuleBasicInfo = IpHlpApi.GetOwnerModuleBasicInfoForTcp6Entry(ref tcp6TableEntry);

                yield return new TcpConnectionInfo(
                    tcp6TableEntry.dwOwningPid,
                    ownerModuleBasicInfo != null ? ownerModuleBasicInfo.Value.pModuleName : null,
                    ownerModuleBasicInfo != null ? ownerModuleBasicInfo.Value.pModulePath : null,
                    tcp6TableEntry.liCreateTimestamp > 0 ? DateTime.FromFileTime(tcp6TableEntry.liCreateTimestamp) : null,
                    GetTcpState(tcp6TableEntry.dwState),
                    GetIpV6Endpoint(tcp6TableEntry.ucLocalAddr, tcp6TableEntry.dwLocalScopeId, tcp6TableEntry.dwLocalPort),
                    GetIpV6Endpoint(tcp6TableEntry.ucRemoteAddr, tcp6TableEntry.dwRemoteScopeId, tcp6TableEntry.dwRemotePort));

            }
        }
        finally
        {
            Marshal.FreeHGlobal(tcp6TablePtr);
        }
    }

    public IEnumerable<UdpConnectionInfo> GetUdpV4Connections()
    {
        int udp4TableSize = 0;

        int result = IpHlpApi.GetExtendedUdpTable(IntPtr.Zero, ref udp4TableSize, true, IpHlpApi.AF_INET, IpHlpApi.UDP_TABLE_CLASS.UDP_TABLE_OWNER_MODULE, 0);
        if (result != WinErrorCodes.ERROR_INSUFFICIENT_BUFFER)
        {
            throw new Win32Exception(result);
        }

        IntPtr udp4TablePtr = Marshal.AllocHGlobal(udp4TableSize);
        try
        {
            result = IpHlpApi.GetExtendedUdpTable(udp4TablePtr, ref udp4TableSize, true, IpHlpApi.AF_INET, IpHlpApi.UDP_TABLE_CLASS.UDP_TABLE_OWNER_MODULE, 0);
            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            IpHlpApi.MIB_UDPTABLE_OWNER_MODULE udpTable = Marshal.PtrToStructure<IpHlpApi.MIB_UDPTABLE_OWNER_MODULE>(udp4TablePtr);

            foreach (IntPtr udp4TableEntryPtr in udpTable.GetTableEntries(udp4TablePtr))
            {
                IpHlpApi.MIB_UDPROW_OWNER_MODULE udpTableEntry = udpTable.GetTableEntry(udp4TableEntryPtr);
                IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO? ownerModuleBasicInfo = IpHlpApi.GetOwnerModuleBasicInfoForUdp4Entry(ref udpTableEntry);

                yield return new UdpConnectionInfo(
                    udpTableEntry.dwOwningPid,
                    ownerModuleBasicInfo != null ? ownerModuleBasicInfo.Value.pModuleName : null,
                    ownerModuleBasicInfo != null ? ownerModuleBasicInfo.Value.pModulePath : null,
                    udpTableEntry.liCreateTimestamp > 0 ? DateTime.FromFileTime(udpTableEntry.liCreateTimestamp) : null,
                    GetIpV4Endpoint(udpTableEntry.dwLocalAddr, udpTableEntry.dwLocalPort));
            }
        }
        finally
        {
            Marshal.FreeHGlobal(udp4TablePtr);
        }
    }

    public IEnumerable<UdpConnectionInfo> GetUdpV6Connections()
    {
        int udp6TableSize = 0;

        int result = IpHlpApi.GetExtendedUdpTable(IntPtr.Zero, ref udp6TableSize, true, IpHlpApi.AF_INET6, IpHlpApi.UDP_TABLE_CLASS.UDP_TABLE_OWNER_MODULE, 0);
        if (result != WinErrorCodes.ERROR_INSUFFICIENT_BUFFER)
        {
            throw new Win32Exception(result);
        }

        IntPtr udp6TablePtr = Marshal.AllocHGlobal(udp6TableSize);
        try
        {
            result = IpHlpApi.GetExtendedUdpTable(udp6TablePtr, ref udp6TableSize, true, IpHlpApi.AF_INET6, IpHlpApi.UDP_TABLE_CLASS.UDP_TABLE_OWNER_MODULE, 0);
            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            IpHlpApi.MIB_UDP6TABLE_OWNER_MODULE udp6Table = Marshal.PtrToStructure<IpHlpApi.MIB_UDP6TABLE_OWNER_MODULE>(udp6TablePtr);

            foreach (IntPtr udp6TableEntryPtr in udp6Table.GetTableEntries(udp6TablePtr))
            {
                IpHlpApi.MIB_UDP6ROW_OWNER_MODULE udp6TableEntry = udp6Table.GetTableEntry(udp6TableEntryPtr);
                IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO? moduleBasicInfo = IpHlpApi.GetOwnerModuleBasicInfoForUdp6Entry(ref udp6TableEntry);

                yield return new UdpConnectionInfo(
                    udp6TableEntry.dwOwningPid,
                    moduleBasicInfo != null ? moduleBasicInfo.Value.pModuleName : null,
                    moduleBasicInfo != null ? moduleBasicInfo.Value.pModulePath : null,
                    udp6TableEntry.liCreateTimestamp > 0 ? DateTime.FromFileTime(udp6TableEntry.liCreateTimestamp) : null,
                    GetIpV6Endpoint(udp6TableEntry.ucLocalAddr, udp6TableEntry.dwLocalScopeId, udp6TableEntry.dwLocalPort));
            }
        }
        finally
        {
            Marshal.FreeHGlobal(udp6TablePtr);
        }
    }

    IPEndPoint GetIpV4Endpoint(int ip, int port)
    {
        return new IPEndPoint((uint)ip, (ushort)IPAddress.NetworkToHostOrder((short)port));
    }

    IPEndPoint GetIpV6Endpoint(byte[] address, int scope, int port)
    {
        return new IPEndPoint(new IPAddress(address, scope), (ushort)IPAddress.NetworkToHostOrder((short)port));
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
}