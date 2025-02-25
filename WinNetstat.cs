using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

internal sealed class WinNetstat
{
    const int ERROR_ACCESS_DENIED = 5;

    const int ERROR_INSUFFICIENT_BUFFER = 122;

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
        IntPtr tcpTablePtr = IntPtr.Zero;
        int tcpTableSize = 0;

        try
        {
            int result = IpHlpApi.GetExtendedTcpTable(tcpTablePtr, ref tcpTableSize, true, IpHlpApi.AF_INET, IpHlpApi.TCP_TABLE_CLASS.TCP_TABLE_OWNER_MODULE_ALL, 0);
            if (result != ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception(result);
            }

            tcpTablePtr = Marshal.AllocHGlobal(tcpTableSize);

            result = IpHlpApi.GetExtendedTcpTable(tcpTablePtr, ref tcpTableSize, true, IpHlpApi.AF_INET, IpHlpApi.TCP_TABLE_CLASS.TCP_TABLE_OWNER_MODULE_ALL, 0);
            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            IpHlpApi.MIB_TCPTABLE_OWNER_MODULE tcpTable = Marshal.PtrToStructure<IpHlpApi.MIB_TCPTABLE_OWNER_MODULE>(tcpTablePtr);

            foreach (IntPtr tcpTableEntryPtr in tcpTable.GetTableEntries(tcpTablePtr))
            {
                IpHlpApi.MIB_TCPROW_OWNER_MODULE tableEntry = tcpTable.GetTableEntry(tcpTableEntryPtr);
                IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO? moduleBasicInfo = null;

                IntPtr moduleInfoPtr = IntPtr.Zero;
                int moduleInfoSize = 0;
                try
                {
                    result = IpHlpApi.GetOwnerModuleFromTcpEntry(
                        tcpTableEntryPtr,
                        IpHlpApi.TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC,
                        moduleInfoPtr,
                        ref moduleInfoSize);
                    if (result == ERROR_INSUFFICIENT_BUFFER)
                    {
                        moduleInfoPtr = Marshal.AllocHGlobal(moduleInfoSize);

                        result = IpHlpApi.GetOwnerModuleFromTcpEntry(
                            tcpTableEntryPtr,
                            IpHlpApi.TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC,
                            moduleInfoPtr,
                            ref moduleInfoSize);
                        if (result != 0)
                        {
                            throw new Win32Exception(result);
                        }

                        moduleBasicInfo = Marshal.PtrToStructure<IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO>(moduleInfoPtr);
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
                    moduleBasicInfo != null ? moduleBasicInfo.Value.pModuleName : null,
                    moduleBasicInfo != null ? moduleBasicInfo.Value.pModulePath : null,
                    tableEntry.liCreateTimestamp > 0 ? DateTime.FromFileTime(tableEntry.liCreateTimestamp) : null,
                    GetTcpState(tableEntry.dwState),
                    GetIpV4Endpoint(tableEntry.dwLocalAddr, tableEntry.dwLocalPort),
                    GetIpV4Endpoint(tableEntry.dwRemoteAddr, tableEntry.dwRemotePort));
            }
        }
        finally
        {
            if (tcpTablePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(tcpTablePtr);
            }
        }
    }

    public IEnumerable<TcpConnectionInfo> GetTcpV6Connections()
    {
        IntPtr tcpTablePtr = IntPtr.Zero;
        int tcpTableSize = 0;

        try
        {
            int result = IpHlpApi.GetExtendedTcpTable(tcpTablePtr, ref tcpTableSize, true, IpHlpApi.AF_INET6, IpHlpApi.TCP_TABLE_CLASS.TCP_TABLE_OWNER_MODULE_ALL, 0);
            if (result != ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception(result);
            }

            tcpTablePtr = Marshal.AllocHGlobal(tcpTableSize);

            result = IpHlpApi.GetExtendedTcpTable(tcpTablePtr, ref tcpTableSize, true, IpHlpApi.AF_INET6, IpHlpApi.TCP_TABLE_CLASS.TCP_TABLE_OWNER_MODULE_ALL, 0);
            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            IpHlpApi.MIB_TCP6TABLE_OWNER_MODULE tcpTable = Marshal.PtrToStructure<IpHlpApi.MIB_TCP6TABLE_OWNER_MODULE>(tcpTablePtr);

            foreach (IntPtr tcpTableEntryPtr in tcpTable.GetTableEntries(tcpTablePtr))
            {
                IpHlpApi.MIB_TCP6ROW_OWNER_MODULE tableEntry = tcpTable.GetTableEntry(tcpTableEntryPtr);
                IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO? moduleBasicInfo = null;

                IntPtr moduleInfoPtr = IntPtr.Zero;
                int moduleInfoSize = 0;
                try
                {
                    result = IpHlpApi.GetOwnerModuleFromTcp6Entry(
                        tcpTableEntryPtr,
                        IpHlpApi.TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC,
                        moduleInfoPtr,
                        ref moduleInfoSize);
                    if (result == ERROR_INSUFFICIENT_BUFFER)
                    {
                        moduleInfoPtr = Marshal.AllocHGlobal(moduleInfoSize);

                        result = IpHlpApi.GetOwnerModuleFromTcp6Entry(
                            tcpTableEntryPtr,
                            IpHlpApi.TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC,
                            moduleInfoPtr,
                            ref moduleInfoSize);
                        if (result != 0)
                        {
                            throw new Win32Exception(result);
                        }

                        moduleBasicInfo = Marshal.PtrToStructure<IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO>(moduleInfoPtr);
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
                    moduleBasicInfo != null ? moduleBasicInfo.Value.pModuleName : null,
                    moduleBasicInfo != null ? moduleBasicInfo.Value.pModulePath : null,
                    tableEntry.liCreateTimestamp > 0 ? DateTime.FromFileTime(tableEntry.liCreateTimestamp) : null,
                    GetTcpState(tableEntry.dwState),
                    GetIpV6Endpoint(tableEntry.ucLocalAddr, tableEntry.dwLocalScopeId, tableEntry.dwLocalPort),
                    GetIpV6Endpoint(tableEntry.ucRemoteAddr, tableEntry.dwRemoteScopeId, tableEntry.dwRemotePort));

            }
        }
        finally
        {
            if (tcpTablePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(tcpTablePtr);
            }
        }
    }

    public IEnumerable<UdpConnectionInfo> GetUdpV4Connections()
    {
        IntPtr udpTablePtr = IntPtr.Zero;
        int udpTableSize = 0;

        try
        {
            int result = IpHlpApi.GetExtendedUdpTable(udpTablePtr, ref udpTableSize, true, IpHlpApi.AF_INET, IpHlpApi.UDP_TABLE_CLASS.UDP_TABLE_OWNER_MODULE, 0);
            if (result != ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception(result);
            }

            udpTablePtr = Marshal.AllocHGlobal(udpTableSize);

            result = IpHlpApi.GetExtendedUdpTable(udpTablePtr, ref udpTableSize, true, IpHlpApi.AF_INET, IpHlpApi.UDP_TABLE_CLASS.UDP_TABLE_OWNER_MODULE, 0);
            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            IpHlpApi.MIB_UDPTABLE_OWNER_MODULE tcpTable = Marshal.PtrToStructure<IpHlpApi.MIB_UDPTABLE_OWNER_MODULE>(udpTablePtr);

            foreach (IntPtr tcpTableEntryPtr in tcpTable.GetTableEntries(udpTablePtr))
            {
                IpHlpApi.MIB_UDPROW_OWNER_MODULE tableEntry = tcpTable.GetTableEntry(tcpTableEntryPtr);
                IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO? moduleBasicInfo = null;

                IntPtr moduleInfoPtr = IntPtr.Zero;
                int moduleInfoSize = 0;
                try
                {
                    result = IpHlpApi.GetOwnerModuleFromUdpEntry(
                        tcpTableEntryPtr,
                        IpHlpApi.TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC,
                        moduleInfoPtr,
                        ref moduleInfoSize);
                    if (result == ERROR_INSUFFICIENT_BUFFER)
                    {
                        moduleInfoPtr = Marshal.AllocHGlobal(moduleInfoSize);

                        result = IpHlpApi.GetOwnerModuleFromUdpEntry(
                            tcpTableEntryPtr,
                            IpHlpApi.TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC,
                            moduleInfoPtr,
                            ref moduleInfoSize);
                        if (result != 0)
                        {
                            throw new Win32Exception(result);
                        }

                        moduleBasicInfo = Marshal.PtrToStructure<IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO>(moduleInfoPtr);
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

                yield return new UdpConnectionInfo(
                    tableEntry.dwOwningPid,
                    moduleBasicInfo != null ? moduleBasicInfo.Value.pModuleName : null,
                    moduleBasicInfo != null ? moduleBasicInfo.Value.pModulePath : null,
                    tableEntry.liCreateTimestamp > 0 ? DateTime.FromFileTime(tableEntry.liCreateTimestamp) : null,
                    GetIpV4Endpoint(tableEntry.dwLocalAddr, tableEntry.dwLocalPort));
            }
        }
        finally
        {
            if (udpTablePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(udpTablePtr);
            }
        }
    }

    public IEnumerable<UdpConnectionInfo> GetUdpV6Connections()
    {
        IntPtr udpTablePtr = IntPtr.Zero;
        int udpTableSize = 0;

        try
        {
            int result = IpHlpApi.GetExtendedUdpTable(udpTablePtr, ref udpTableSize, true, IpHlpApi.AF_INET6, IpHlpApi.UDP_TABLE_CLASS.UDP_TABLE_OWNER_MODULE, 0);
            if (result != ERROR_INSUFFICIENT_BUFFER)
            {
                throw new Win32Exception(result);
            }

            udpTablePtr = Marshal.AllocHGlobal(udpTableSize);

            result = IpHlpApi.GetExtendedUdpTable(udpTablePtr, ref udpTableSize, true, IpHlpApi.AF_INET6, IpHlpApi.UDP_TABLE_CLASS.UDP_TABLE_OWNER_MODULE, 0);
            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            IpHlpApi.MIB_UDP6TABLE_OWNER_MODULE tcpTable = Marshal.PtrToStructure<IpHlpApi.MIB_UDP6TABLE_OWNER_MODULE>(udpTablePtr);

            foreach (IntPtr tcpTableEntryPtr in tcpTable.GetTableEntries(udpTablePtr))
            {
                IpHlpApi.MIB_UDP6ROW_OWNER_MODULE tableEntry = tcpTable.GetTableEntry(tcpTableEntryPtr);
                IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO? moduleBasicInfo = null;

                IntPtr moduleInfoPtr = IntPtr.Zero;
                int moduleInfoSize = 0;
                try
                {
                    result = IpHlpApi.GetOwnerModuleFromUdp6Entry(
                        tcpTableEntryPtr,
                        IpHlpApi.TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC,
                        moduleInfoPtr,
                        ref moduleInfoSize);
                    if (result == ERROR_INSUFFICIENT_BUFFER)
                    {
                        moduleInfoPtr = Marshal.AllocHGlobal(moduleInfoSize);

                        result = IpHlpApi.GetOwnerModuleFromUdp6Entry(
                            tcpTableEntryPtr,
                            IpHlpApi.TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC,
                            moduleInfoPtr,
                            ref moduleInfoSize);
                        if (result != 0)
                        {
                            throw new Win32Exception(result);
                        }

                        moduleBasicInfo = Marshal.PtrToStructure<IpHlpApi.TCPIP_OWNER_MODULE_BASIC_INFO>(moduleInfoPtr);
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

                yield return new UdpConnectionInfo(
                    tableEntry.dwOwningPid,
                    moduleBasicInfo != null ? moduleBasicInfo.Value.pModuleName : null,
                    moduleBasicInfo != null ? moduleBasicInfo.Value.pModulePath : null,
                    tableEntry.liCreateTimestamp > 0 ? DateTime.FromFileTime(tableEntry.liCreateTimestamp) : null,
                    GetIpV6Endpoint(tableEntry.ucLocalAddr, tableEntry.dwLocalScopeId, tableEntry.dwLocalPort));
            }
        }
        finally
        {
            if (udpTablePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(udpTablePtr);
            }
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