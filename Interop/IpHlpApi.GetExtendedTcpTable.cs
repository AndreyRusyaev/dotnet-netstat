using System.Runtime.InteropServices;

internal static partial class IpHlpApi
{
    public const int AF_INET = 2;

    public const int AF_INET6 = 23;

    public enum TCP_TABLE_CLASS
    {
        TCP_TABLE_BASIC_LISTENER,
        TCP_TABLE_BASIC_CONNECTIONS,
        TCP_TABLE_BASIC_ALL,
        TCP_TABLE_OWNER_PID_LISTENER,
        TCP_TABLE_OWNER_PID_CONNECTIONS,
        TCP_TABLE_OWNER_PID_ALL,
        TCP_TABLE_OWNER_MODULE_LISTENER,
        TCP_TABLE_OWNER_MODULE_CONNECTIONS,
        TCP_TABLE_OWNER_MODULE_ALL
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPTABLE_OWNER_MODULE
    {
        public MIB_TCPTABLE_OWNER_MODULE()
        {            
        }

        public int dwNumEntries;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public MIB_TCPROW_OWNER_MODULE[] table = new MIB_TCPROW_OWNER_MODULE[1];

        public IntPtr[] GetTableEntries(IntPtr tablePointer)
        {
            var tableEntries = new IntPtr[dwNumEntries];
            IntPtr itemPtr = tablePointer + Marshal.OffsetOf<MIB_TCPTABLE_OWNER_MODULE>(nameof(table));
            for (int ii = 0; ii < dwNumEntries; ii += 1)
            {
                tableEntries[ii] = itemPtr;
                itemPtr += Marshal.SizeOf<MIB_TCPROW_OWNER_MODULE>();
            }
            return tableEntries;
        }

        public MIB_TCPROW_OWNER_MODULE GetTableEntry(IntPtr itemPtr)
        {
            return Marshal.PtrToStructure<MIB_TCPROW_OWNER_MODULE>(itemPtr);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPROW_OWNER_MODULE 
    {
        public MIB_TCPROW_OWNER_MODULE()
        {            
        }

        public MIB_TCP_STATE dwState;
        public int dwLocalAddr;
        public int dwLocalPort;
        public int dwRemoteAddr;
        public int dwRemotePort;
        public int dwOwningPid;
        public long liCreateTimestamp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public long[] OwningModuleInfo = new long[16];
    }

    [DllImport("iphlpapi.dll")]
    public static extern int GetExtendedTcpTable(IntPtr pTcpTable, ref int pdwSize, bool bOrder, int ulAf, TCP_TABLE_CLASS TableClass, int Reserved);
}