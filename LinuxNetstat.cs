using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

internal sealed class LinuxNetstat
{
    private static readonly TcpState[] tcpStates = [
        0,
        TcpState.Established, TcpState.SynSent, TcpState.SynReceived, TcpState.FinWait, TcpState.FinWait2,
        TcpState.TimeWait, TcpState.Closed, TcpState.CloseWait, TcpState.LastAck, TcpState.Listening, TcpState.Closing,
    ];

    public IEnumerable<TcpConnectionInfo> GetTcpConnections()
    {
        var inodeTable = BuildINodeSearchTable();

        foreach (var msg in EnumDiagMessages(Libc.AF.AF_INET, Libc.IPPROTO.IPPROTO_TCP))
        {
            SocketINodeInfo? inodeInfo = inodeTable.GetValueOrDefault(msg.idiag_inode);

            yield return new TcpConnectionInfo(
                inodeInfo != null ? inodeInfo.pid : 0,
                inodeInfo?.procName,
                inodeInfo?.procName,
                inodeInfo?.fdCreated,
                GetTcpState(msg.idiag_state),
                GetIpV4Endpoint(msg.id.ManagedSrc[0], msg.id.idiag_sport),
                GetIpV4Endpoint(msg.id.ManagedDst[0], msg.id.idiag_dport)
            );
        }

        foreach (var msg in EnumDiagMessages(Libc.AF.AF_INET6, Libc.IPPROTO.IPPROTO_TCP))
        {
            SocketINodeInfo? inodeInfo = inodeTable.GetValueOrDefault(msg.idiag_inode);

            yield return new TcpConnectionInfo(
                inodeInfo != null ? inodeInfo.pid : 0,
                inodeInfo?.procName,
                inodeInfo?.procName,
                inodeInfo?.fdCreated,
                GetTcpState(msg.idiag_state),
                GetIpV6Endpoint(msg.id.ManagedSrcBytes, 0, msg.id.idiag_sport),
                GetIpV6Endpoint(msg.id.ManagedDstBytes, 0, msg.id.idiag_dport)
            );
        }
    }

    public IEnumerable<UdpConnectionInfo> GetUdpConnections()
    {
        var inodeTable = BuildINodeSearchTable();

        foreach (var msg in EnumDiagMessages(Libc.AF.AF_INET, Libc.IPPROTO.IPPROTO_UDP))
        {
            SocketINodeInfo? inodeInfo = inodeTable.GetValueOrDefault(msg.idiag_inode);

            yield return new UdpConnectionInfo(
                inodeInfo != null ? inodeInfo.pid : 0,
                inodeInfo?.procName,
                inodeInfo?.procName,
                inodeInfo?.fdCreated,
                GetIpV4Endpoint(msg.id.ManagedSrc[0], msg.id.idiag_sport)
            );
        }

        foreach (var msg in EnumDiagMessages(Libc.AF.AF_INET6, Libc.IPPROTO.IPPROTO_UDP))
        {
            SocketINodeInfo? inodeInfo = inodeTable.GetValueOrDefault(msg.idiag_inode);

            yield return new UdpConnectionInfo(
                inodeInfo != null ? inodeInfo.pid : 0,
                inodeInfo?.procName,
                inodeInfo?.procName,
                inodeInfo?.fdCreated,
                GetIpV6Endpoint(msg.id.ManagedSrcBytes, 0, msg.id.idiag_sport)
            );
        }
    }

    IEnumerable<Libc.inet_diag_msg> EnumDiagMessages(Libc.AF addressFamily, Libc.IPPROTO protocol)
    {
        var socketHandle = Libc.socket((int)Libc.AF.AF_NETLINK, Libc.SOCK_TYPE.SOCK_RAW, Libc.NETLINK_SOCK_DIAG);
        if (socketHandle.IsInvalid)
        {
            var errorCode = Marshal.GetLastSystemError();
            throw new Exception($"socket failed. ErrorCode = '{errorCode}'.");
        }

        using (var socket = new Socket(socketHandle))
        {
            Libc.sockaddr_nl socket_addr = new Libc.sockaddr_nl();
            socket_addr.nl_family = (int)Libc.AF.AF_NETLINK;

            var request = new Libc.nlrequest();

            request.nlh.nlmsg_len = Marshal.SizeOf<Libc.nlrequest>();
            request.nlh.nlmsg_type = Libc.SOCK_DIAG_BY_FAMILY;
            request.nlh.nlmsg_flags = Libc.NLM_FLAGS.NLM_F_REQUEST | Libc.NLM_FLAGS.NLM_F_DUMP;

            request.req.sdiag_family = (byte)addressFamily;
            request.req.sdiag_protocol = (byte)protocol;
            request.req.idiag_states = ~0; // Get all states
            request.req.idiag_ext = 0;

            int sendToResult = Libc.sendto(socket.SafeHandle, ref request, Marshal.SizeOf(request), 0, ref socket_addr, Marshal.SizeOf(socket_addr));
            if (sendToResult == -1)
            {
                var errorCode = Marshal.GetLastSystemError();
                throw new Exception($"sendto failed. ErrorCode = '{errorCode}'.");                
            }

            IntPtr buf = Marshal.AllocHGlobal(8132);
            while (true)
            {
                int len = Libc.recv(socket.SafeHandle, buf, 8132, 0);
                if (len == 0)
                {
                    break;
                }

                while (true)
                {
                    if (len < Marshal.SizeOf<Libc.nlmsghdr>())
                    {
                        break;
                    }

                    var nlh = Marshal.PtrToStructure<Libc.nlmsghdr>(buf);

                    if (nlh.nlmsg_len < Marshal.SizeOf<Libc.nlmsghdr>() || nlh.nlmsg_len > len)
                    {
                        break;
                    }

                    if (nlh.nlmsg_type == (short)Libc.NLMSG_TYPE.NLMSG_DONE)
                    {
                        yield break;
                    }

                    if (nlh.nlmsg_type == (short)Libc.NLMSG_TYPE.NLMSG_ERROR)
                    {
                        Console.WriteLine("ERROR");
                        yield break;
                    }

                    yield return Marshal.PtrToStructure<Libc.inet_diag_msg>(buf + Libc.NLMSG_ALIGNTO(Marshal.SizeOf<Libc.nlmsghdr>()));

                    buf += Libc.NLMSG_ALIGNTO(nlh.nlmsg_len);
                    len -= Libc.NLMSG_ALIGNTO(nlh.nlmsg_len);
                }
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

    TcpState GetTcpState(int linuxTcpState)
    {
        switch ((Libc.TCP_STATES)linuxTcpState)
        {
            case Libc.TCP_STATES.TCP_ESTABLISHED:
                return TcpState.Established;
            case Libc.TCP_STATES.TCP_SYN_SENT:
                return TcpState.SynSent;
            case Libc.TCP_STATES.TCP_SYN_RECV:
                return TcpState.SynReceived;
            case Libc.TCP_STATES.TCP_FIN_WAIT1:
                return TcpState.FinWait;
            case Libc.TCP_STATES.TCP_FIN_WAIT2:
                return TcpState.FinWait2;
            case Libc.TCP_STATES.TCP_TIME_WAIT:
                return TcpState.TimeWait;
            case Libc.TCP_STATES.TCP_CLOSE:
                return TcpState.Closed;
            case Libc.TCP_STATES.TCP_CLOSE_WAIT:
                return TcpState.CloseWait;
            case Libc.TCP_STATES.TCP_LAST_ACK:
                return TcpState.LastAck;
            case Libc.TCP_STATES.TCP_LISTEN:
                return TcpState.Listening;
            case Libc.TCP_STATES.TCP_CLOSING:
                return TcpState.Closing;
        }

        return TcpState.Unknown;
    }

    private IReadOnlyDictionary<long, SocketINodeInfo> BuildINodeSearchTable()
    {
        Dictionary<long, SocketINodeInfo> dict = new Dictionary<long, SocketINodeInfo>();

        foreach (var procInfo in LinuxProc.GetProcInfos())
        {
            foreach (var fdInfo in procInfo.Fds)
            {
                dict[fdInfo.EntryINode] = new SocketINodeInfo(fdInfo.EntryINode, fdInfo.Id, fdInfo.EntryCreated, procInfo.Id, procInfo.ExePath != null ? Path.GetFileName(procInfo.ExePath) : null);
            }
        }

        return dict;
    }

    record SocketINodeInfo(long socketIno, int fd, DateTimeOffset fdCreated, int pid, string? procName);
}