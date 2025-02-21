using System.Runtime.InteropServices;

internal static partial class IpHlpApi
{
    public enum TCP_CONNECTION_OFFLOAD_STATE 
    {
        TcpConnectionOffloadStateInHost,
        TcpConnectionOffloadStateOffloading,
        TcpConnectionOffloadStateOffloaded,
        TcpConnectionOffloadStateUploading,
        TcpConnectionOffloadStateMax
    }

    public enum MIB_TCP_STATE
    {
        MIB_TCP_STATE_CLOSED     =  1,
        MIB_TCP_STATE_LISTEN     =  2,
        MIB_TCP_STATE_SYN_SENT   =  3,
        MIB_TCP_STATE_SYN_RCVD   =  4,
        MIB_TCP_STATE_ESTAB      =  5,
        MIB_TCP_STATE_FIN_WAIT1  =  6,
        MIB_TCP_STATE_FIN_WAIT2  =  7,
        MIB_TCP_STATE_CLOSE_WAIT =  8,
        MIB_TCP_STATE_CLOSING    =  9,
        MIB_TCP_STATE_LAST_ACK   = 10,
        MIB_TCP_STATE_TIME_WAIT  = 11,
        MIB_TCP_STATE_DELETE_TCB = 12,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MIB_TCPROW2
    {
        public MIB_TCP_STATE dwState;
        public int dwLocalAddr;
        public int dwLocalPort;
        public int dwRemoteAddr;
        public int dwRemotePort;
        public int dwOwningPid;
        public TCP_CONNECTION_OFFLOAD_STATE dwOffloadState;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MIB_TCPTABLE2
    {
        public MIB_TCPTABLE2()
        {            
        }

        public int dwNumEntries;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public MIB_TCPROW2[] table = new MIB_TCPROW2[1];

        public MIB_TCPROW2[] GetTableEntries(IntPtr tablePointer)
        {
            var tableEntries = new MIB_TCPROW2[dwNumEntries];
            IntPtr itemPtr = tablePointer + Marshal.OffsetOf<MIB_TCPTABLE2>(nameof(table));
            for (int ii = 0; ii < dwNumEntries; ii += 1)
            {
                tableEntries[ii] = Marshal.PtrToStructure<MIB_TCPROW2>(itemPtr);
                itemPtr += Marshal.SizeOf<MIB_TCPROW2>();
            }
            return tableEntries;
        }
    }

    [DllImport("iphlpapi.dll")]
    public static extern int GetTcpTable2([In, Out] IntPtr tcpTable, ref int sizePointer, bool order);
}