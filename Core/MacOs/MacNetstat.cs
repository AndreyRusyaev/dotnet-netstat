using System.Net;
using System.Runtime.InteropServices;
using System.Text;

internal sealed class MacNetstat
{
    private static readonly TcpState[] tcpStates = [
        TcpState.Closed, TcpState.Listening, TcpState.SynSent, TcpState.SynReceived,
        TcpState.Established, TcpState.CloseWait, TcpState.FinWait, TcpState.Closing,
        TcpState.LastAck, TcpState.FinWait2, TcpState.TimeWait
    ];

    public IEnumerable<TcpConnectionInfo> GetTcpConnections()
    {
        int bufferSize = Libproc.proc_listpids(Libproc.PROC_ALL_PIDS, 0, IntPtr.Zero, 0);
        IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
        try
        {
            int pidListCountInBytes = Libproc.proc_listpids(Libproc.PROC_ALL_PIDS, 0, buffer, bufferSize);
            int[] pids = new int[pidListCountInBytes / sizeof(int)];
            Marshal.Copy(buffer, pids, 0, pids.Length);

            foreach (var pid in pids)
            {
                StringBuilder procNameBuilder = new StringBuilder(4 * 1024);
                Libproc.proc_name(pid, procNameBuilder, procNameBuilder.Capacity);

                string procName = procNameBuilder.ToString();

                StringBuilder procPathBuilder = new StringBuilder(4 * 1024);
                Libproc.proc_pidpath(pid, procPathBuilder, procPathBuilder.Capacity);

                string procPath = procPathBuilder.ToString();

                foreach (var connectionInfo in GetTcpConnectionsForPid(pid, procName, procPath))
                {
                    yield return connectionInfo;
                }
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public IEnumerable<UdpConnectionInfo> GetUdpConnections()
    {
        int bufferSize = Libproc.proc_listpids(Libproc.PROC_ALL_PIDS, 0, IntPtr.Zero, 0);
        IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
        try
        {
            int pidListCountInBytes = Libproc.proc_listpids(Libproc.PROC_ALL_PIDS, 0, buffer, bufferSize);
            int[] pids = new int[pidListCountInBytes / sizeof(int)];
            Marshal.Copy(buffer, pids, 0, pids.Length);

            foreach (var pid in pids)
            {
                StringBuilder procNameBuilder = new StringBuilder(4 * 1024);
                Libproc.proc_name(pid, procNameBuilder, procNameBuilder.Capacity);

                string procName = procNameBuilder.ToString();

                StringBuilder procPathBuilder = new StringBuilder(4 * 1024);
                Libproc.proc_pidpath(pid, procPathBuilder, procPathBuilder.Capacity);

                string procPath = procPathBuilder.ToString();

                foreach (var connectionInfo in GetUdpConnectionsForPid(pid, procName, procPath))
                {
                    yield return connectionInfo;
                }
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private IEnumerable<TcpConnectionInfo> GetTcpConnectionsForPid(int pid, string processName, string processPath)
    {
        int bufferSize = Libproc.proc_pidinfo(pid, Libproc.PROC_PIDLISTFDS, 0, IntPtr.Zero, 0);
        IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
        try
        {
            int fdListCountInBytes = Libproc.proc_pidinfo(pid, Libproc.PROC_PIDLISTFDS, 0, buffer, bufferSize);
            if (fdListCountInBytes < 0)
            {
                // Process exited
                yield break;
            }

            if (fdListCountInBytes > bufferSize)
            {
                // Race condition: new fds were created. Ignore them for now.
                fdListCountInBytes = bufferSize;
            }

            Libproc.proc_fdinfo[] fdList = new Libproc.proc_fdinfo[fdListCountInBytes / Marshal.SizeOf<Libproc.proc_fdinfo>()];

            int fdIndex = 0;
            for (IntPtr ptr = buffer; ptr < buffer + fdListCountInBytes; ptr += Marshal.SizeOf<Libproc.proc_fdinfo>())
            {
                fdList[fdIndex] = Marshal.PtrToStructure<Libproc.proc_fdinfo>(ptr);
                fdIndex += 1;
            }

            foreach (var fileInfo in fdList)
            {
                if (fileInfo.proc_fdtype != Libproc.PROX_FDTYPE_SOCKET)
                {
                    // Skip non-sockets
                    continue;
                }

                Libproc.socket_fdinfo socket_fdinfo = new Libproc.socket_fdinfo();
                var error = Libproc.proc_pidfdinfo(pid, fileInfo.proc_fd, Libproc.PROC_PIDFDSOCKETINFO, ref socket_fdinfo, Marshal.SizeOf<Libproc.socket_fdinfo>());
                if (error < 0)
                {
                    // process exited or socket released
                    continue;
                }

                if (socket_fdinfo.psi.soi_kind != (int)Libproc.SOCKINFO_TYPE.SOCKINFO_TCP)
                {
                    continue;
                }

                var tcp_sockinfo = socket_fdinfo.psi.soi_proto.pri_tcp;

                if (socket_fdinfo.psi.soi_family == Libproc.AF_INET)
                {
                    yield return new TcpConnectionInfo(
                        pid,
                        processName,
                        processPath,
                        null,
                        tcpStates[tcp_sockinfo.tcpsi_state],
                        GetIpV4Endpoint(tcp_sockinfo.tcpsi_ini.insi_laddr.ina_46.i46a_addr4, tcp_sockinfo.tcpsi_ini.insi_lport),
                        GetIpV4Endpoint(tcp_sockinfo.tcpsi_ini.insi_faddr.ina_46.i46a_addr4, tcp_sockinfo.tcpsi_ini.insi_fport));
                }
                else if (socket_fdinfo.psi.soi_family == Libproc.AF_INET6)
                {
                    yield return new TcpConnectionInfo(
                        pid,
                        processName,
                        processPath,
                        null,
                        tcpStates[tcp_sockinfo.tcpsi_state],
                        GetIpV6Endpoint(tcp_sockinfo.tcpsi_ini.insi_laddr.ina_6.ManagedBytes, 0, tcp_sockinfo.tcpsi_ini.insi_lport),
                        GetIpV6Endpoint(tcp_sockinfo.tcpsi_ini.insi_faddr.ina_6.ManagedBytes, 0, tcp_sockinfo.tcpsi_ini.insi_fport));
                }
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private IEnumerable<UdpConnectionInfo> GetUdpConnectionsForPid(int pid, string processName, string processPath)
    {
        int bufferSize = Libproc.proc_pidinfo(pid, Libproc.PROC_PIDLISTFDS, 0, IntPtr.Zero, 0);
        IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
        try
        {
            int fdListCountInBytes = Libproc.proc_pidinfo(pid, Libproc.PROC_PIDLISTFDS, 0, buffer, bufferSize);
            if (fdListCountInBytes < 0)
            {
                // Process exited
                yield break;
            }

            if (fdListCountInBytes > bufferSize)
            {
                // Race condition: new fds were created. Ignore them for now.
                fdListCountInBytes = bufferSize;
            }

            Libproc.proc_fdinfo[] fdList = new Libproc.proc_fdinfo[fdListCountInBytes / Marshal.SizeOf<Libproc.proc_fdinfo>()];

            int fdIndex = 0;
            for (IntPtr ptr = buffer; ptr < buffer + fdListCountInBytes; ptr += Marshal.SizeOf<Libproc.proc_fdinfo>())
            {
                fdList[fdIndex] = Marshal.PtrToStructure<Libproc.proc_fdinfo>(ptr);
                fdIndex += 1;
            }

            foreach (var fileInfo in fdList)
            {
                if (fileInfo.proc_fdtype != Libproc.PROX_FDTYPE_SOCKET)
                {
                    continue;
                }

                Libproc.socket_fdinfo socket_fdinfo = new Libproc.socket_fdinfo();
                var error = Libproc.proc_pidfdinfo(pid, fileInfo.proc_fd, Libproc.PROC_PIDFDSOCKETINFO, ref socket_fdinfo, Marshal.SizeOf<Libproc.socket_fdinfo>());
                if (error < 0)
                {
                    // process exited or socket released
                    continue;
                }

                if (socket_fdinfo.psi.soi_kind != (int)Libproc.SOCKINFO_TYPE.SOCKINFO_IN)
                {
                    continue;
                }

                Libproc.in_sockinfo in_sockinfo = socket_fdinfo.psi.soi_proto.pri_in;
                if (socket_fdinfo.psi.soi_family == Libproc.AF_INET)
                {
                    yield return new UdpConnectionInfo(
                            pid,
                            processName,
                            processPath,
                            null,
                            GetIpV4Endpoint(in_sockinfo.insi_laddr.ina_46.i46a_addr4, in_sockinfo.insi_lport)
                            );
                }
                else if (socket_fdinfo.psi.soi_family == Libproc.AF_INET6)
                {
                    yield return new UdpConnectionInfo(
                        pid,
                        processName,
                        processPath,
                        null,
                        GetIpV6Endpoint(in_sockinfo.insi_laddr.ina_6.ManagedBytes, 0, in_sockinfo.insi_lport)
                        );
                }
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
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
}