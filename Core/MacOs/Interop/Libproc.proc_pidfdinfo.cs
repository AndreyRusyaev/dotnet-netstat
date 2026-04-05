using System.Runtime.InteropServices;

internal static partial class Libproc
{
    public const int PROC_PIDFDSOCKETINFO = 3;

    public const int AF_INET = 2;

    public const int AF_INET6 = 30;

    public enum SOCKINFO_TYPE
    {
        SOCKINFO_GENERIC = 0,
        SOCKINFO_IN = 1,
        SOCKINFO_TCP = 2,
        SOCKINFO_UN = 3,
        SOCKINFO_NDRV = 4,
        SOCKINFO_KERN_EVENT = 5,
        SOCKINFO_KERN_CTL = 6,
        SOCKINFO_VSOCK = 7,
    }

    [StructLayout(LayoutKind.Sequential, Size = 792)]
    public struct socket_fdinfo
    {
        public proc_fileinfo pfi;
        public socket_info psi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct proc_fileinfo
    {
        public int fi_openflags;
        public int fi_status;
        public long fi_offset;
        public int fi_type;
        public int fi_guardflags;
    }

    [StructLayout(LayoutKind.Sequential, Size = 768)]
    public struct socket_info
    {
        public vinfo_stat soi_stat;
        public long soi_so;         /* opaque handle of socket */
        public long soi_pcb;        /* opaque handle of protocol control block */
        public int soi_type;
        public int soi_protocol;
        public int soi_family;
        public short soi_options;
        public short soi_linger;
        public short soi_state;
        public short soi_qlen;
        public short soi_incqlen;
        public short soi_qlimit;
        public short soi_timeo;
        public short soi_error;
        public int soi_oobmark;
        public sockbuf_info soi_rcv;
        public sockbuf_info soi_snd;
        public int soi_kind;
        public int rfu_1;          /* reserved */
        public proto_info soi_proto;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct vinfo_stat
    {
        public int vst_dev;        /* [XSI] ID of device containing file */
        public short vst_mode;       /* [XSI] Mode of file (see below) */
        public short vst_nlink;      /* [XSI] Number of hard links */
        public long vst_ino;        /* [XSI] File serial number */
        public int vst_uid;        /* [XSI] User ID of the file */
        public int vst_gid;        /* [XSI] Group ID of the file */
        public long vst_atime;      /* [XSI] Time of last access */
        public long vst_atimensec;  /* nsec of last access */
        public long vst_mtime;      /* [XSI] Last data modification time */
        public long vst_mtimensec;  /* last data modification nsec */
        public long vst_ctime;      /* [XSI] Time of last status change */
        public long vst_ctimensec;  /* nsec of last status change */
        public long vst_birthtime;  /*  File creation time(birth)  */
        public long vst_birthtimensec;      /* nsec of File creation time */
        public long vst_size;       /* [XSI] file size, in bytes */
        public long vst_blocks;     /* [XSI] blocks allocated for file */
        public int vst_blksize;    /* [XSI] optimal blocksize for I/O */
        public int vst_flags;      /* user defined flags for file */
        public int vst_gen;        /* file generation number */
        public int vst_rdev;       /* [XSI] Device ID */
        public fixed long vst_qspare[2];  /* RESERVED: DO NOT USE! */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct sockbuf_info
    {
        public int sbi_cc;
        public int sbi_hiwat;                      /* SO_RCVBUF, SO_SNDBUF */
        public int sbi_mbcnt;
        public int sbi_mbmax;
        public int sbi_lowat;
        public short sbi_flags;
        public short sbi_timeo;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct proto_info
    {
        [FieldOffset(0)]
        public in_sockinfo pri_in;                 /* SOCKINFO_IN */

        [FieldOffset(0)]
        public tcp_sockinfo pri_tcp;                /* SOCKINFO_TCP */
        // public  un_sockinfo      pri_un;                 /* SOCKINFO_UN */
        // public  ndrv_info        pri_ndrv;               /* SOCKINFO_NDRV */
        // public  kern_event_info  pri_kern_event;         /* SOCKINFO_KERN_EVENT */
        // public  kern_ctl_info    pri_kern_ctl;           /* SOCKINFO_KERN_CTL */
        // public  vsock_sockinfo   pri_vsock;              /* SOCKINFO_VSOCK */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct in_sockinfo
    {
        public int insi_fport;             /* foreign port */
        public int insi_lport;             /* local port */
        public long insi_gencnt;            /* generation count of this instance */
        public int insi_flags;             /* generic IP/datagram flags */
        public int insi_flow;

        public byte insi_vflag;             /* ini_IPV4 or ini_IPV6 */
        public byte insi_ip_ttl;            /* time to live proto */
        public int rfu_1;                  /* reserved */
        /* protocol dependent part */
        public in_addr_type insi_faddr;             /* foreign host table entry */
        public in_addr_type insi_laddr;             /* local host table entry */
        public insi_v4_type insi_v4;
        public insi_v6_type insi_v6;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct tcp_sockinfo
    {
        public in_sockinfo tcpsi_ini;
        public int tcpsi_state;
        public fixed int tcpsi_timer[4];
        public int tcpsi_mss;
        public int tcpsi_flags;
        public int rfu_1;          /* reserved */
        public long tcpsi_tp;       /* opaque handle of TCP protocol control block */
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct in_addr_type
    {
        [FieldOffset(0)]
        public in4in6_addr ina_46;
        [FieldOffset(0)]
        public in6_addr ina_6;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct in4in6_addr
    {
        public fixed int i46a_pad32[3];
        public int i46a_addr4;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct in6_addr
    {
        [FieldOffset(0)]
        public fixed byte __u6_addr8[16];

        [FieldOffset(0)]
        public fixed short __u6_addr16[8];

        [FieldOffset(0)]
        public fixed int __u6_addr32[4];

        public byte[] ManagedBytes
        {
            get
            {
                byte[] address = new byte[16];
                fixed (byte* array = __u6_addr8)
                {
                    Marshal.Copy(new IntPtr(array), address, 0, address.Length);
                }
                return address;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct insi_v4_type
    {
        public byte in4_tos;                        /* type of service */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct insi_v6_type
    {
        public byte in6_hlim;
        public int in6_cksum;
        public short in6_ifindex;
        public short in6_hops;
    }

    [DllImport("libproc")]
    public static extern int proc_pidfdinfo(int pid, int fd, int flavor, IntPtr bufferPtr, int buffersize);

    [DllImport("libproc")]
    public static extern int proc_pidfdinfo(int pid, int fd, int flavor, ref socket_fdinfo fdinfo, int buffersize);
}