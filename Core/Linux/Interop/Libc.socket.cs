using System.Net.Sockets;
using System.Runtime.InteropServices;

#pragma warning disable CS8981 // CS8981:The type name only contains lower-cased ascii characters.

internal static partial class Libc
{
    public const int NETLINK_SOCK_DIAG = 4; /* socket monitoring */

    public const int SOCK_DIAG_BY_FAMILY = 20;

    /* Supported address families. */
    public enum AF
    {
        AF_UNSPEC = 0,
        AF_UNIX = 1,    /* Unix domain sockets 		*/
        AF_LOCAL = 1,   /* POSIX name for AF_UNIX	*/
        AF_INET = 2,    /* Internet IP Protocol 	*/
        AF_AX25 = 3,    /* Amateur Radio AX.25 		*/
        AF_IPX = 4, /* Novell IPX 			*/
        AF_APPLETALK = 5,   /* AppleTalk DDP 		*/
        AF_NETROM = 6,  /* Amateur Radio NET/ROM 	*/
        AF_BRIDGE = 7,  /* Multiprotocol bridge 	*/
        AF_ATMPVC = 8,  /* ATM PVCs			*/
        AF_X25 = 9, /* Reserved for X.25 project 	*/
        AF_INET6 = 10,  /* IP version 6			*/
        AF_ROSE = 11,   /* Amateur Radio X.25 PLP	*/
        AF_DECnet = 12, /* Reserved for DECnet project	*/
        AF_NETBEUI = 13,    /* Reserved for 802.2LLC project*/
        AF_SECURITY = 14,   /* Security callback pseudo AF */
        AF_KEY = 15,      /* PF_KEY key management API */
        AF_NETLINK = 16,
        AF_ROUTE = AF_NETLINK, /* Alias to emulate 4.4BSD */
        AF_PACKET = 17, /* Packet family		*/
        AF_ASH = 18,    /* Ash				*/
        AF_ECONET = 19, /* Acorn Econet			*/
        AF_ATMSVC = 20, /* ATM SVCs			*/
        AF_RDS = 21,    /* RDS sockets 			*/
        AF_SNA = 22,    /* Linux SNA Project (nutters!) */
        AF_IRDA = 23,   /* IRDA sockets			*/
        AF_PPPOX = 24,  /* PPPoX sockets		*/
        AF_WANPIPE = 25,    /* Wanpipe API Sockets */
        AF_LLC = 26,    /* Linux LLC			*/
        AF_IB = 27, /* Native InfiniBand address	*/
        AF_MPLS = 28,   /* MPLS */
        AF_CAN = 29,    /* Controller Area Network      */
        AF_TIPC = 30,   /* TIPC sockets			*/
        AF_BLUETOOTH = 31,  /* Bluetooth sockets 		*/
        AF_IUCV = 32,   /* IUCV sockets			*/
        AF_RXRPC = 33,  /* RxRPC sockets 		*/
        AF_ISDN = 34,   /* mISDN sockets 		*/
        AF_PHONET = 35, /* Phonet sockets		*/
        AF_IEEE802154 = 36, /* IEEE802154 sockets		*/
        AF_CAIF = 37,   /* CAIF sockets			*/
        AF_ALG = 38,    /* Algorithm sockets		*/
        AF_NFC = 39,    /* NFC sockets			*/
        AF_VSOCK = 40,  /* vSockets			*/
        AF_KCM = 41,    /* Kernel Connection Multiplexor*/
        AF_QIPCRTR = 42,    /* Qualcomm IPC Router          */
        AF_SMC = 43,    /* smc sockets: reserve number for
		 * = PF_SMC, protocol family that
		 * = reuses, AF_INET address family
		 */
        AF_XDP = 44,    /* XDP sockets			*/
        AF_MCTP = 45,   /* Management component
		 * = transport, protocol
		 */
        AF_MAX = 46,	/* For now.. */
    }

    public enum IPPROTO
    {
        IPPROTO_IP = 0,     /* Dummy protocol for TCP		*/
        IPPROTO_ICMP = 1,       /* Internet Control Message Protocol	*/
        IPPROTO_IGMP = 2,       /* Internet Group Management Protocol	*/
        IPPROTO_IPIP = 4,       /* IPIP tunnels (older KA9Q tunnels use 94) */
        IPPROTO_TCP = 6,        /* Transmission Control Protocol	*/
        IPPROTO_EGP = 8,        /* Exterior Gateway Protocol		*/
        IPPROTO_PUP = 12,       /* PUP protocol				*/
        IPPROTO_UDP = 17,       /* User Datagram Protocol		*/
        IPPROTO_IDP = 22,       /* XNS IDP protocol			*/
        IPPROTO_TP = 29,        /* SO Transport Protocol Class 4	*/
        IPPROTO_DCCP = 33,      /* Datagram Congestion Control Protocol */
        IPPROTO_IPV6 = 41,      /* IPv6-in-IPv4 tunnelling		*/
        IPPROTO_RSVP = 46,      /* RSVP Protocol			*/
        IPPROTO_GRE = 47,       /* Cisco GRE tunnels (rfc 1701,1702)	*/
        IPPROTO_ESP = 50,       /* Encapsulation Security Payload protocol */
        IPPROTO_AH = 51,        /* Authentication Header protocol	*/
        IPPROTO_MTP = 92,       /* Multicast Transport Protocol		*/
        IPPROTO_BEETPH = 94,        /* IP option pseudo header for BEET	*/
        IPPROTO_ENCAP = 98,     /* Encapsulation Header			*/
        IPPROTO_PIM = 103,      /* Protocol Independent Multicast	*/
        IPPROTO_COMP = 108,     /* Compression Header Protocol		*/
        IPPROTO_L2TP = 115,     /* Layer 2 Tunnelling Protocol		*/
        IPPROTO_SCTP = 132,     /* Stream Control Transport Protocol	*/
        IPPROTO_UDPLITE = 136,  /* UDP-Lite (RFC 3828)			*/
        IPPROTO_MPLS = 137,     /* MPLS in IP (RFC 4023)		*/
        IPPROTO_ETHERNET = 143, /* Ethernet-within-IPv6 Encapsulation	*/
        IPPROTO_AGGFRAG = 144,  /* AGGFRAG in ESP (RFC 9347)		*/
        IPPROTO_RAW = 255,      /* Raw IP packets			*/
        IPPROTO_SMC = 256,      /* Shared Memory Communications		*/
        IPPROTO_MPTCP = 262,        /* Multipath TCP connection		*/
        IPPROTO_MAX
    }

    public enum SOCK_TYPE
    {
        SOCK_STREAM = 1,
        SOCK_DGRAM = 2,
        SOCK_RAW = 3,
        SOCK_RDM = 4,
        SOCK_SEQPACKET = 5,
        SOCK_DCCP = 6,
        SOCK_PACKET = 10,
    }

    public enum TCP_STATES 
    {
        TCP_ESTABLISHED = 1,
        TCP_SYN_SENT,
        TCP_SYN_RECV,
        TCP_FIN_WAIT1,
        TCP_FIN_WAIT2,
        TCP_TIME_WAIT,
        TCP_CLOSE,
        TCP_CLOSE_WAIT,
        TCP_LAST_ACK,
        TCP_LISTEN,
        TCP_CLOSING,	/* Now a valid state */
        TCP_NEW_SYN_RECV,
        TCP_BOUND_INACTIVE, /* Pseudo-state for inet_diag */

        TCP_MAX_STATES	/* Leave at the end! */
    };

    [Flags]
    public enum NLM_FLAGS : short
    {
        NLM_F_REQUEST = 0x01,	/* It is request message. 	*/
        NLM_F_MULTI = 0x02,	/* Multipart message, terminated by NLMSG_DONE */
        NLM_F_ACK = 0x04,	/* Reply with ack, with zero or error code */
        NLM_F_ECHO = 0x08,	/* Receive resulting notifications */
        NLM_F_DUMP_INTR = 0x10,	/* Dump was inconsistent due to sequence change */
        NLM_F_DUMP_FILTERED = 0x20, /* Dump was filtered as requested */

        /* Modifiers to GET request */
        NLM_F_ROOT = 0x100, /* specify tree	root	*/
        NLM_F_MATCH = 0x200,    /* return all matching	*/
        NLM_F_ATOMIC = 0x400,   /* atomic GET		*/
        NLM_F_DUMP = (NLM_F_ROOT | NLM_F_MATCH),

        /* Modifiers to NEW request */
        NLM_F_REPLACE = 0x100,  /* Override existing		*/
        NLM_F_EXCL = 0x200, /* Do not touch, if it exists	*/
        NLM_F_CREATE = 0x400,   /* Create, if it does not exist	*/
        NLM_F_APPEND = 0x800,   /* Add to end of list		*/

        /* Modifiers to DELETE request */
        NLM_F_NONREC = 0x100,   /* Do not delete recursively	*/
        NLM_F_BULK = 0x200, /* Delete multiple objects	*/

        /* Flags for ACK message */
        NLM_F_CAPPED = 0x100,   /* request was capped */
        NLM_F_ACK_TLVS = 0x200,	/* extended ACK TVLs were included */
    }

    public enum NLMSG_TYPE
    {
        NLMSG_NOOP = 0x1,   /* Nothing.	*/
        NLMSG_ERROR = 0x2,  /* Error */
        NLMSG_DONE = 0x3,   /* End of a dump */
        NLMSG_OVERRUN = 0x4,	/* Data lost */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NL_REQUEST
    {
        public nlmsghdr nlh;
        public inet_diag_req_v2 req;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct sockaddr_nl
    {
        public short nl_family; /* AF_NETLINK	*/
        public short nl_pad;        /* zero		*/
        public int nl_pid;      /* port ID	*/
        public int nl_groups;   /* multicast groups mask */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct nlrequest
    {
        public nlmsghdr nlh;
        public inet_diag_req_v2 req;
    }

    /**
     * struct nlmsghdr - fixed format metadata header of Netlink messages
     * @nlmsg_len:   Length of message including header
     * @nlmsg_type:  Message content type
     * @nlmsg_flags: Additional flags
     * @nlmsg_seq:   Sequence number
     * @nlmsg_pid:   Sending process port ID
     */
    [StructLayout(LayoutKind.Sequential)]
    public struct nlmsghdr
    {
        public int nlmsg_len;
        public short nlmsg_type;
        public NLM_FLAGS nlmsg_flags;
        public int nlmsg_seq;
        public int nlmsg_pid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct inet_diag_req_v2
    {
        public byte sdiag_family;
        public byte sdiag_protocol;
        public byte idiag_ext;
        public byte pad;
        public int idiag_states;
        public inet_diag_sockid id;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct inet_diag_sockid
    {
        // __be16  
        public short idiag_sport;
        // __be16  
        public short idiag_dport;
        // __be32
        public fixed int idiag_src[4];
        // __be32
        public fixed int idiag_dst[4];
        public int idiag_if;
        public fixed int idiag_cookie[2];

        public int[] ManagedSrc
        {
            get
            {
                int[] result = new int[4];
                fixed (int* ptr = idiag_src)
                {
                    Marshal.Copy(new IntPtr(ptr), result, 0, result.Length);
                }
                return result;
            }
        }

        public byte[] ManagedSrcBytes
        {
            get
            {
                byte[] result = new byte[16];
                fixed (int* ptr = idiag_src)
                {
                    Marshal.Copy(new IntPtr(ptr), result, 0, result.Length);
                }
                return result;
            }
        }

        public int[] ManagedDst
        {
            get
            {
                int[] result = new int[4];
                fixed (int* ptr = idiag_dst)
                {
                    Marshal.Copy(new IntPtr(ptr), result, 0, result.Length);
                }
                return result;
            }
        }

        public byte[] ManagedDstBytes
        {
            get
            {
                byte[] result = new byte[16];
                fixed (int* ptr = idiag_dst)
                {
                    Marshal.Copy(new IntPtr(ptr), result, 0, result.Length);
                }
                return result;
            }
        }
    }

    /* Base info structure. It contains socket identity (addrs/ports/cookie)
     * and, alas, the information shown by netstat. 
     */
    [StructLayout(LayoutKind.Sequential)]
    public struct inet_diag_msg
    {
        public byte idiag_family;
        public byte idiag_state;
        public byte idiag_timer;
        public byte idiag_retrans;

        public inet_diag_sockid id;

        public int idiag_expires;
        public int idiag_rqueue;
        public int idiag_wqueue;
        public int idiag_uid;
        public int idiag_inode;
    }

    public static int NLMSG_ALIGNTO(int len)
    {
        const uint NLMSG_ALIGNTO = 4;
        return (int)((len + (NLMSG_ALIGNTO - 1)) & ~(NLMSG_ALIGNTO - 1));
    }

    [DllImport("libc", SetLastError = true)]
    public static extern SafeSocketHandle socket(int domain, SOCK_TYPE type, int protocol);

    [DllImport("libc", SetLastError = true)]
    public static extern int sendto(SafeSocketHandle sockfd, ref nlrequest buf, int len, int flags, ref sockaddr_nl dest_addr, int addr_len);

    [DllImport("libc", SetLastError = true)]
    public static extern int recv(SafeSocketHandle sockfd, IntPtr buf, int len, int flags);
}